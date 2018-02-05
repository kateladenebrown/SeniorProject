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
        // Coded by Stephen 1/24/18
        /// <summary>
        /// Starts a new game instance with the caller and listed users as players.
        /// </summary>
        /// <param name="players">A list of usernames specifying who should be a player.</param>
        /// <returns>A message indicating that the game was created successfully, or an error otherwise.</returns>
        [HttpPost]
        [Route("Create", Name = "Create New Game")]
        public IHttpActionResult gameCreate(List<string> players)
        {
            int GIDHolder;
            if (players.Count == 0)
            {
                using (var db = new GameEntities())
                {
                    try
                    {
                        Game g = new Game(); // create a game
                        g.Start = System.DateTime.Now; // set start time to now
                        g.Status = 1; // set to pending status ( 1 ) 
                        GIDHolder = g.ID; // hold the created game ID

                        foreach ( string name in players)
                        {
                            GameUser usr = new GameUser(); // make a GameUser holder
                            usr.UserID = db.Users.Single(x => x.Username == name).ID  ; // find the single ID, where the UserName is the current name
                            usr.GameID = GIDHolder; // Set gameId to GIDHolders value
                            usr.Status = 1; // set each player to pending status
                            g.GameUsers.Add(usr); // add each GameUser iteration to the game instance
                        } // end foreach

                        try { db.Games.Add(g); } catch (Exception e) { return Content(System.Net.HttpStatusCode.NotModified, "Failure to add Game to Database."); } // end try2
                        db.SaveChanges() ; // save changes to db 
                        return Ok("Game Controller gameCreate API Call");
                    } // end try1
                    catch (Exception e)
                    { return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error and was unable to create the game. Please inform the development team."); } // end catch1
                } // end using
            } // end if players == 0
            else { return Content(System.Net.HttpStatusCode.NotAcceptable, "Failure to create game due to 0 players in list.") ; }
        }

        /* testing notes for GameCreate: 
         * Method requires a List<User>. 
         * Send an empty list or one that has a 0 count.
         * Send List<User> that has more than 1 User in the list. 
         * Send List<User> where one userId is invalid $$ no catch for this specifically yet
         */



        // GET: api/Game/MyGames
        // -Written by Garrick 1/23/18
        /// <summary>
        /// Retrieves a list of games the user is or was a player in.
        /// </summary>
        /// <returns>A list of Game objects.</returns>
        [HttpGet]
        [Route("MyGames", Name = "Get My Games")]
        public IHttpActionResult GetMyGames() //(GameStatus)
        {
            try
            {
                //var myGames = db.games.where(GameStatus == active)
                return Ok("Game Controller GetMyGames API Call");
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
        /// GameID
        /// </summary>
        /// <param name="id">The ID of the game whose history should be returned.</param>
        /// <returns>A list of GameState objects.</returns>
        [HttpGet]
        [Route("GameHistory", Name = "Get Game History")]
        public IHttpActionResult GetGameHistory(int id)
        {
            try
            {
                // Need more details of database implementation, but
                // hopefully this is similar to the actual implementation

                // -Cameron: This looks good. I just commented out pieces for the moment so that the solution would build.

                //using (var db = new Game.ENTITIES())
                //{
                //    var gameHistory = db.games.where(gameHistory => GetMyGames.id);
                //    return Ok("Game Controller GetGameHistory API Call");
                //}

                return Ok("Game Controller GetGameHistory API Call");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the game history. Please inform the development team.");
            }
        }

        // GET: api/Game
        // >Tyler Lancaster, 1/25/18
        /// <summary>
        /// Returns the latest game record for the passed-in GameID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetGame(GameID id)
        {
            try
            {         
                var game = db.games.where(Games.id => id && GameStates.timestamp => mostRecent);

                return Ok("Game Controller GetGame API Call");
            }
            catch (Exception e)
            {
                return Exception("GetGame call failed");
            }
        }
    }
}