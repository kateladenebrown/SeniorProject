using GameEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net;

namespace TurnBasedGameAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Game")]
    public class GameController : ApiController
    {
        // James, 2/17/18
        // Enum for updating game user status
        enum GameLogicResponseCodes { Invalid, Valid, GameActive, GameInactive };


        private IGameLogic logic;
        //Kate Brown
        //02-13-2018
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameLogic"></param>
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
        public IHttpActionResult CreateGame(List<string> players)
        {
            try
            {
                using (var db = new GameEntities())
                {
                    //Add the user who made the call to the list of game participants.
                    players.Add(User.Identity.Name);

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
                    IQueryable<Game> myGames = db.GameUsers.Where(gu => gu.AspNetUser.UserName == User.Identity.Name).Select(g => g.Game);

                    if (gameStatus != -1)
                    {
                        myGames = myGames.Where(x => x.Status == gameStatus);
                    }

                    ////May not be neccessary:
                    ////In the case that the user exists but does not have any games, return OK with an empty result.
                    //if (!myGames.Any())
                    //{
                    //    return Ok();
                    //}

                    return Ok(myGames.ToList());
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
                    //Note: The database is ordered by timestampt descending, we can use the first record.

                    //Cameron: ^This is not true. We have an index which makes sorting by timestamp descending extremely fast,
                    //  but that does not mean it is sorting by timestamp descending by default.


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
                return Content(HttpStatusCode.BadRequest, "Issue!");
            }
            catch (InvalidOperationException e)
            {
                return Content(HttpStatusCode.BadRequest, "Can't do that.");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the game history. Please inform the development team.");
            }

        }

        // POST: api/Game/Update
        /// Kate Brown 2/13/18
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="gameId"></param>
        /// <param name="requestedTurn"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Update", Name = "Update Game")]
        public IHttpActionResult Update(int gameId, string requestedTurn)
        {
            try
            {
                using (GameEntities db = new GameEntities())
                {
                    string outputGameState = null;
                    GameState currentGameState = db.GameStates.Where(x => x.GameID == gameId).OrderByDescending(x => x.TimeStamp).First();
                    string callingUsername = User.Identity.Name;
                    int statusChangeResult = logic.TryTakeTurn(ref outputGameState, currentGameState.GameState1, gameId, callingUsername, requestedTurn);

                    switch(statusChangeResult)
                    {
                        case 0:
                            //No change for the game and no change for the player.
                            break;
                        case 1:
                            //Valid status change for the player but no change for the game.
                            break;
                        case 2:
                            //Valid status change for the player and game status goes to active.
                            break;
                        case 3:
                            //Valid status change for the player and game status goes to inactive.
                            break;
                    }

                    if(!string.IsNullOrWhiteSpace(outputGameState))
                    {
                        GameState gameState = new GameState();
                        gameState.GameID = currentGameState.GameID;
                        gameState.Game = currentGameState.Game;
                        gameState.GameState1 = outputGameState;
                        gameState.TimeStamp = DateTime.Now;
                        db.GameStates.Add(gameState);
                        db.SaveChanges();
                    }
                }

                return Ok();
            }
            catch(InvalidOperationException)
            {
                return Content(HttpStatusCode.NotFound, "Could not find a game with an ID of " + gameId);
            }
            catch(Exception)
            {
                return Content(HttpStatusCode.InternalServerError, "The server encountered an error when attempting to update the game state.");
            }
        }

        // POST: api/Game/UpdateUserStatus
        // James 2/13/18
        /// <summary>
        /// Determines if status change is valid. 
        /// If valid, update game user status, update game state, and update game status as needed.
        /// Status codes: 1="Pending", 2="Active", 3="Inactive"
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="gID"></param>
        /// <returns>No message if update was successful, error otherwise</returns>
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

                    // Creates tuple list of userName and status
                    var userNameStatusList = db.GameUsers.Select(x => new Tuple<string, int>(x.AspNetUser.UserName, x.Status)).ToList();

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
                            case (int)GameLogicResponseCodes.GameActive:
                                game.Status = 2;
                                break;
                            case (int)GameLogicResponseCodes.GameInactive:
                                game.Status = 3;
                                game.End = DateTime.Now;
                                break;
                            default:
                                break;
                        }
                        db.SaveChanges();
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
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error updating game user status. Please inform the development team.");
            }
        }
    }
}