using GameEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
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
            try
            {
                /* not sure what all we are going to need here just yet.
                 */

                return Ok("Game Controller gameCreate API Call");
            }
            catch (Exception e)
            {

                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error and was unable to create the game. Please inform the development team.");
            }

        }

        // GET: api/Game/GameID
        // Written by Tyler Lancaster, 1/25/2018
        /// <summary>
        /// Returns the latest game record for the passed-in GameID
        /// </summary>
        /// <param name="id">The ID of the game whose most recent state should be returned.</param>
        /// <returns>Latest gamestate of given gameID.</returns>
        [HttpGet]
        [Route("GameID", Name = "Get Latest Game")]    //What exactly is the name of this? 
        public IHttpActionResult GetGameID(int id)
        {
            try
            {

                using (var db = new GameEntities())
                 {

                    //If this syntax works, it should get all gamestates that match the id, then sort and return the most recent gamestate
                    GameState latestGameState = db.GameStates.Where(gs => gs.GameID == id).OrderByDescending(x => x.TimeStamp).First();


                    return Ok(latestGameState);
                }

            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the game history. Please inform the development team.");
            }
        }

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
            using (var db = new GameEntities())
            {
                try
                {
                    //List<User> myGames = db.GameUsers.Where(u => u.Username == User.Identity.Name).ToList();
                    return Ok("Game Controller GetMyGames API Call");
                }
                catch (Exception e)
                {
                    return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error retrieving the list of games. Please inform the development team.");
                }
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
    }
}