﻿using GameEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using GameEF;
//using System.Web.Mvc;

namespace TurnBasedGameAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Game")]
    public class GameController : ApiController
    {
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
                        return Content(System.Net.HttpStatusCode.NotFound, "One or more of the game participants were not found in the database.");
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
                return Content(System.Net.HttpStatusCode.InternalServerError, "The database encountered an error while attempting to retrieve information about the participants.");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error and was unable to create the game. Please inform the development team.");
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
                return Content(System.Net.HttpStatusCode.NotFound, "The user who made the call could not be found in the database.");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error retrieving the list of games. Please inform the development team.");
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
                return Content(System.Net.HttpStatusCode.NotFound, "No game with the ID specified was found in the database.");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the game history. Please inform the development team.");
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
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the latest game state. Please inform the development team.");
            }
        }

        // POST: api/Game/UpdateGameUserStatus ???
        // Written by James, 2/13/18
        /// <summary>
        /// Determines if status change is valid. If change is valid update game status then handle change accordingly
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="gID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateGameUserStatus", Name = "Update Game User Status")] // ???
        public IHttpActionResult UpdateGameUserStatus(int newStatus, int gID)
        {
            try
            {
                using (var db = new GameEntities())
                {
                    List<GameUser> gameUsers = db.Games.Single(g => g.ID == gID).GameUsers.ToList();

                    //Get the list of game states for the id provided and order them by descending.
                    IQueryable<GameState> gameStatesDesc = db.GameStates.Where(x => x.GameID == gID).OrderByDescending(x => x.TimeStamp);

                    //If game states were found, set the latest one to gameState else set to null.
                    GameState gameState;
                    if (gameStatesDesc.Any())
                    {
                        gameState = gameStatesDesc.First();
                    }
                    else
                    {
                        gameState = null;
                    }

                    // Determine if updating game user status is valid
                    int oldStatus = db.GameUsers.Single(gu => gu.AspNetUser.UserName == User.Identity.Name).Status;
                    if(IsValidGUStatusChange(oldStatus, newStatus))
                    {
                        // Handle update or possible update to game here
                        return Ok(); // ???
                    } else
                    {
                        // Correct way to handle this return? NotImplemented status code? something else?
                        return Content(System.Net.HttpStatusCode.PaymentRequired, "An illegal operation was attempted. Game user status was not updated.");
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                return Content(System.Net.HttpStatusCode.NotFound, "The user who made the call could not be found in the database.");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error updating game user status. Please inform the development team.");
            }
        }

        // Written by James, 2/13/18
        /// <summary>
        /// Determines if status codes are valid to change.
        /// Status Codes: 1="Pending", 2="Active", 3="Inactive"
        /// Valid Changes: 1->2 (Accept request), 1->3 (Deny request), 2->3 (Quit game)
        /// (Possible invalid changes) 2->1, 3->1 (Poss. request sent during active game), 3->2 (Poss. allow user to start midgame)
        /// </summary>
        /// <param name="oldStatus"></param>
        /// <param name="newStatus"></param>
        /// <returns>True if valid request, false otherwise</returns>
        private bool IsValidGUStatusChange(int oldStatus, int newStatus)
        {
            switch (oldStatus)
            {
                case 1:
                    return (newStatus == 2 || newStatus == 3);
                case 2:
                    return newStatus == 3;
                default:
                    return false;
            }
        }
    }
}