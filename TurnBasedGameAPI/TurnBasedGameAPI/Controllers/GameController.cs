﻿using GameEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net;
using System.Data.Entity;
using System.Web.Http.Cors;
using TurnBasedGameAPI.ViewModels;

namespace TurnBasedGameAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Game")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GameController : ApiController
    {
        // James, 2/17/18
        // Enum for updating game user status
        /// <summary>
        /// Expected response codes from IGameLogic calls.
        /// </summary>
        enum GameLogicResponseCodes {
            /// <summary>
            /// The change submitted to the game logic was not valid.
            /// </summary>
            Invalid,
            /// <summary>
            /// The change submitted to the game logic was valid but there is no resulting game status change.
            /// </summary>
            Valid,
            /// <summary>
            /// The change submitted to the game logic was valid and the game status should change to Active.
            /// </summary>
            GameActive,
            /// <summary>
            /// The change submitted to the game logic was valid and the game status should change to Inctive.
            /// </summary>
            GameInactive
        };

        private IGameLogic logic;
        //Kate Brown
        //02-13-2018
        /// <summary>
        /// No-arg constructor for the game controller - also instantiates the game logic specified in the web.config file.
        /// </summary>
        public GameController()
        {
            this.logic = Bootstrapper.GetGameLogic();
        }

        // POST: api/Game/Create
        // Coded by Stephen 2/7/18
        /// <summary>
        /// Starts a new game instance with the caller and listed users as players.
        /// </summary>
        /// <param name="players">A list of usernames specifying who should be a player.</param>
        /// <returns>Returns the newly created game's ID if the game was created successfully, or an error otherwise.</returns>
        [HttpPost]
        [Route("Create", Name = "Create New Game")]
        public IHttpActionResult CreateGame([FromBody] List<string> players)
        {
            try
            {
                using (var db = new GameEntities())
                {
                    //Add the user who made the call to the list of game participants.
                    if (!players.Contains(User.Identity.Name))
                    {
                        players.Add(User.Identity.Name);
                    }

                    //Get the ID and username for each participant.
                    var participants = db.AspNetUsers.Where(x => players.Contains(x.UserName)).Select(x => new { x.Id, x.UserName }).ToList();

                    //Create a list of GameUser objects using the participants. The user who made the call will have a status of 2 (active), while all others will be 1 (pending).
                    var newGameUsers = participants.Select(x => new GameUser
                    {
                        UserID = x.Id,
                        Status = (x.UserName == User.Identity.Name) ? 2 : 1
                    }).ToList();

                    //Check that all game participants have accounts (and were found) in the database. If not, return an error.
                    if (newGameUsers.Count() != players.Count())
                    {
                        return Content(HttpStatusCode.NotFound, "One or more of the game participants were not found in the database.");
                    }

                    string createGameMessage = null;
                    if (!logic.TryCreateGame(ref createGameMessage, participants.Select(x => x.UserName).ToList()))
                    {
                        return Content(HttpStatusCode.BadRequest, createGameMessage);
                    }

                    Game g = new Game()
                    {
                        Start = DateTime.Now,
                        GameUsers = newGameUsers
                    };

                    db.Games.Add(g);
                    db.SaveChanges();

                    return Ok(g.ID);
                }
            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.InternalServerError, "The database encountered an error while attempting to retrieve information about the participants.");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, "The server encountered an error and was unable to create the game. Please inform the development team.");
            }
        }

        /* testing notes for GameCreate: 
         * Method requires a List<string>. 
         * Send an empty list or one that has a 0 count.
         * Send List<string>  
         * Send List<string> where one userId is invalid
         */

        // GET: api/Game/MyGames
        // -Written by Garrick 1/23/18
        /// <summary>
        /// Retrieves a list of games the user is or was a player in.
        /// </summary>
        /// <param name="gameStatus">The ID of the status to filter by. (If no ID is provided, all records are returned.)</param>
        /// <returns>A list of Game objects.</returns>
        [HttpGet]
        [Route("MyGames", Name = "Get My Games")]
        public IHttpActionResult GetMyGames(int gameStatus = -1) // string? Gamestatus to check for active vs inactive games
        {
            try
            {
                using (var db = new GameEntities())
                {
                    IQueryable<Game> gameQueryable = db.GameUsers
                        .Where(gu => gu.AspNetUser.UserName == User.Identity.Name)
                        .Select(g => g.Game)
                        .Include(x => x.GameUsers.Select(y => y.AspNetUser));

                    if (gameStatus != -1)
                    {
                        gameQueryable = gameQueryable.Where(x => x.Status == gameStatus);
                    }

                    List<GameDetails> gameList = gameQueryable.ToList().Select(x => new GameDetails(x)).ToList();

                    return Ok(gameList);
                }
            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.NotFound, "The user who made the call could not be found in the database.");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, "The server encountered an error retrieving the list of games. Please inform the development team.");
            }
        }

        // GET: api/Game/GameHistory
        // @Michael Case, 1/23/18
        /// <summary>
        /// Retrieves all game records related to a specific game.
        /// </summary>
        /// <param name="id">The ID of the game whose history should be returned.</param>
        /// <returns>A list of GameState objects.</returns>
        [HttpGet]
        [Route("GameHistory", Name = "Get Game History")]
        public IHttpActionResult GetGameHistory(int id)
        {
            try
            {
                using (var db = new GameEntities())
                {
                    List<GameState> myGames = db.Games.Single(g => g.ID == id).GameStates.ToList();

                    return Ok(myGames);
                }
            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.NotFound, "No game with the ID specified was found in the database.");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the game history. Please inform the development team.");
            }
        }

        // GET: api/Game
        // >Tyler Lancaster, 1/25/18
        /// <summary>
        /// Retrieves the latest game state for the game whose ID was provided.
        /// </summary>
        /// <param name="id">The ID of the game whose latest state should be returned.</param>
        /// <returns>The latest GameState object for the specified game. If no game states exist, the response will be empty.</returns>
        public IHttpActionResult GetGame(int id)
        {
            try
            {
                using (var db = new GameEntities())
                {
                    //Get the list of game states for the id provided and order them by descending.
                    IQueryable<GameState> gameStatesDesc = db.GameStates.Where(x => x.GameID == id).OrderByDescending(x => x.TimeStamp);

                    //If game states were found, return the latest one. Otherwise, return an empty list.
                    if (gameStatesDesc.Any())
                    {
                        return Ok(gameStatesDesc.First());
                    }
                    else
                    {
                        return Ok();
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.NotFound, "Requested data not found.");
            }
            catch (InvalidOperationException e)
            {
                return Content(HttpStatusCode.BadRequest, "Unabble to process request.");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the game history. Please inform the development team.");
            }

        }

        // POST: api/Game/Update
        //Kate Brown 2/13/18
        /// <summary>
        /// Verifies a player's turn with the game logic implementing IGameLogic.
        /// </summary>
        /// <param name="gameId">The ID of the game to update.</param>
        /// <param name="requestedTurn">JSON representing a player's turn.</param>
        /// <returns>JSON that represents the latest gamestate, assuming the game changed after a player's turn.</returns>
        [HttpPost]
        [Route("Update", Name = "Update Game")]
        public IHttpActionResult Update(int gameId, [FromBody] string requestedTurn)
        {
            try
            {
                string outputGameState = null;

                using (GameEntities db = new GameEntities())
                {
                    //Get the latest gamestate for the requested game
                    GameState currentGameState;
                    Game currentGame = db.Games.Include(x => x.GameStates).First(g => g.ID == gameId);

                    // Checks if game is already over
                    if (currentGame.Status == 3)
                    {
                        return Content(HttpStatusCode.BadRequest, "Game is inactive. No more status changes allowed.");
                    }

                    IEnumerable<GameState> gameStatesDesc = currentGame.GameStates.Where(gs => gs.GameID == gameId).OrderByDescending(x => x.TimeStamp);

                    //As long as at least one game state exists, process player's turn
                    if (gameStatesDesc.Any())
                    {
                        currentGameState = gameStatesDesc.First();
                        string callingUsername = User.Identity.Name;
                        //Validate player's turn with the implementation of IGameLogic, returns GameLogicResponseCode
                        int tryTurnResult = logic.TryTakeTurn(ref outputGameState, currentGameState.GameState1, gameId, callingUsername, requestedTurn);

                        // Process response code
                        switch (tryTurnResult)
                        {
                            case (int)GameLogicResponseCodes.Invalid: //Invalid player turn, no change to game status
                                return BadRequest();
                            case (int)GameLogicResponseCodes.Valid: //Valid player turn, no change to game status
                                break;
                            case (int)GameLogicResponseCodes.GameActive: //Valid player turn, game status changes to active
                                currentGame.Status = 2;
                                break;
                            case (int)GameLogicResponseCodes.GameInactive: //Valid player turn, game status changes to inactive
                                currentGame.Status = 3;
                                foreach (var gu in db.GameUsers.Where(x => x.GameID == gameId))
                                {
                                    gu.Status = 3;
                                }
                                currentGame.End = DateTime.Now;
                                break;
                            default:
                                //If we receive a status > 3, the game logic class is bad.
                                return Content(HttpStatusCode.InternalServerError, "The server encountered an issue while processing the request. Please inform the development team.");
                        }

                        //If TryTakeTurn returned a new gamestate, save it in the database
                        if (!string.IsNullOrWhiteSpace(outputGameState))
                        {
                            GameState gameState = new GameState
                            {
                                GameID = currentGameState.GameID,
                                GameState1 = outputGameState,
                                TimeStamp = DateTime.Now
                            };
                            currentGame.GameStates.Add(gameState);
                        }
                    }
                    //No gamestates exist for requested game, turn not allowed
                    else
                    {
                        return Content(HttpStatusCode.NotFound, "The game " + gameId + " has no gamestates.");
                    }
                    db.SaveChanges();
                }

                //If TryTakeTurn returned a new gamestate, return it to the calling user
                if (!string.IsNullOrWhiteSpace(outputGameState))
                {
                    return Ok(outputGameState);
                }

                //Otherwise, return an empty OK response.
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return Content(HttpStatusCode.NotFound, "Could not find a game with an ID of " + gameId);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.InternalServerError, "The server encountered an error when attempting to update the game state.");
            }
        }

        // POST: api/Game/UpdateUserStatus
        // James 2/13/18
        // Determines if status change is valid. 
        // If valid, update game user status, update game state, and update game status as needed.
        // Status codes: 1="Pending", 2="Active", 3="Inactive"
        /// <summary>
        /// Changes the current user's status for the specified game.
        /// </summary>
        /// <param name="newStatus">The status the user would like to change to.</param>
        /// <param name="gID">The ID of the game for which the user would liek to change their status.</param>
        /// <returns>A success status code with an empty body, or an error otherwise.</returns>
        [HttpPost]
        [Route("UpdateUserStatus", Name = "Update Game User Status")]
        public IHttpActionResult UpdateUserStatus(int newStatus, int gID)
        {
            try
            {
                using (var db = new GameEntities())
                {
                    // Create game object from game ID, gID, to minimize db calls
                    Game game = db.Games.Single(x => x.ID == gID);

                    // Checks if game is already over
                    if (game.Status == 3)
                    {
                        return Content(HttpStatusCode.BadRequest, "Game is inactive. No more status changes allowed.");
                    }

                    // Creates tuple list of userName and status
                    var userNameStatusList = db.GameUsers.Where(x => x.GameID == gID).Include(x => x.AspNetUser).ToList()
                        .Select(x => new Tuple<string, int>(x.AspNetUser.UserName, x.Status)).ToList();

                    //Get the list of game states for the id provided and order them by descending.
                    IQueryable<GameState> gameStatesDesc = db.GameStates.Where(x => x.GameID == gID).OrderByDescending(x => x.TimeStamp);

                    //If game states were found, set the latest one to gameState.
                    string newGameState = "";
                    string gameState = "";
                    if (gameStatesDesc.Any())
                    {
                        gameState = gameStatesDesc.First().GameState1;
                    }

                    int statusCode = logic.TryUpdateUserStatus(ref newGameState, gameState, gID, userNameStatusList, User.Identity.Name, newStatus);
                    if (statusCode > 0)
                    {
                        // Update game user's status in db
                        db.GameUsers.Single(x => x.GameID == gID && x.AspNetUser.UserName == User.Identity.Name).Status = newStatus;

                        // Update game state in db
                        if (!String.IsNullOrEmpty(newGameState))
                        {
                            GameState gameSt = new GameState
                            {
                                GameState1 = newGameState,
                                TimeStamp = DateTime.Now,
                                GameID = gID
                            };
                            game.GameStates.Add(gameSt);
                        }

                        // Update game status (active/inactive) if needed
                        switch (statusCode)
                        {
                            case (int)GameLogicResponseCodes.Valid:
                                break;
                            case (int)GameLogicResponseCodes.GameActive:
                                game.Status = 2;
                                break;
                            case (int)GameLogicResponseCodes.GameInactive:
                                game.Status = 3;
                                foreach (var gu in db.GameUsers.Where(x => x.GameID == gID))
                                {
                                    gu.Status = 3;
                                }
                                game.End = DateTime.Now;
                                break;
                            default:
                                //If we receive a status > 3, the game logic class is bad.
                                return Content(HttpStatusCode.InternalServerError, "The server encountered an issue while processing the request. Please inform the development team.");
                        }
                        db.SaveChanges();

                        //If TryUpdateUserStatus returned a new gamestate, return it to the calling user.
                        if (!string.IsNullOrWhiteSpace(newGameState))
                        {
                            return Ok(newGameState);
                        }

                        //Otherwise, return an empty OK response.
                        return Ok();
                    }
                    else
                    {
                        return Content(System.Net.HttpStatusCode.BadRequest, "Invalid status change request.");
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return Content(System.Net.HttpStatusCode.NotFound, "Cound not find game with an ID of " + gID);
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error updating game user status. Please inform the development team.");
            }
        }
    }
}