using GameEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;

namespace TurnBasedGameAPI.Controllers
{
    //[Authorize]
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

        // GET: api/Game/MyGames
        // -Written by Garrick 1/23/18
        /// <summary>
        /// Retrieves a list of games the user is or was a player in.
        /// </summary>
        /// <param name="gameStatus">The value of the gamestatus to filter for, default is -1 for ignore</param>
        /// <returns>A list of Game objects.</returns>
        [HttpGet]
        [Route("MyGames", Name = "Get My Games")]
        public IHttpActionResult GetMyGames(int gameStatus = -1) // string? Gamestatus to check for active vs inactive games
        {
            using (var db = new GameEntities())
            {
                try
                {
                    IQueryable<Game> myGames = db.GameUsers
                        .Where(gu => gu.UserID == User.Identity.Name)
                        .Select(g => g.Game);
                    if (gameStatus != -1)
                    {
                        myGames = myGames.Where(x => x.Status == gameStatus);
                    }

                    return Ok(myGames.ToList());
                }
                catch (InvalidOperationException e)//User does not exist
                {
                    return NotFound();
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