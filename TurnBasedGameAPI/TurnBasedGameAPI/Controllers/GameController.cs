using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace TurnBasedGameAPI.Controllers
{
    public class GameController : ApiController
    {


        // GET: api/Game/getPeronalData
        /// <summary>
        /// Returns all personal data about a user
        /// coded by Stephen 1/24/18, last updated 1/24/18
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult getPersonalDetails(UserId id)
        {
            // ask database for personal details from tables matching the user id
            // returns the data in ?? specific format ?? as object ?? 
            try
            {
                // Obj holder = db.User.where(UserId => id) ;

                // format or return 

            }
            catch (Exception e) { return Exception("Failure in getPersonalData"); }

            return Ok("Game Controller getPersonalDetails API Call");
        }

        // GET: api/Game/deleteUser
        /// <summary>
        /// used to mark users as inactive. Toggles boolean "active" to false
        /// coded by Stephen 1/24/18, last updated 1/24/18
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult deleteUser(UserId id)
        {
            // tell database to toggle active to false on user matching 'id'
            try
            {
                // db is the database. varify path to active field

                // db.User.active.where(UserId => id) = false ;
            }
            catch (Exception e) { return Exception("failure to deleteUser"); }

            return Ok("Game Controller deleteUser API Call");
        }

        // GET: api/Game/deleteUser
        /// <summary>
        /// creates an instance of game. Uses list<player>
        /// coded by Stephen 1/24/18, last updated 1/24/18
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public IHttpActionResult gameCreate(List<Player> players)
        {
            /* used to instanciate a game instance
             * args: List of Player, 
             */
            try
            {
                /* not sure what all we are going to need here just yet.
                 */

            }
            catch (Exception e) { return Exception("gameCreate failed to perform as expected. :(   my bad"); }

            return Ok("Game Controller gameCreate API Call");
        }

        // GET: api/Game/GetMyGames
        /// <summary>
        /// Returns all active User games
        /// -Written by Garrick 1/23/18
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetMyGames() //(GameStatus)
        {
            try
            {
                //var myGames = db.games.where(GameStatus == active)
            }
            catch (Exception e)
            {
                throw new Exception("Error in GetMyGames API call");
            }
            return Ok("Game Controller GetMyGames API Call");
        }

        // GET: api/Game
        /// <summary>
        /// Returns all game records for a passed in
        /// GameID
        /// @Michael Case, 1/23/18
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetGameHistory(GameID id)
        {
            try
            {
                // Need more details of database implementation, but
                // hopefully this is similar to the actual implementation
                using (var db = new Game.ENTITIES())
                {
                    var gameHistory = db.games.where(gameHistory => GetMyGames.id);
                    return Ok("Game Controller GetGameHistory API Call");
                }

            }
            catch (Exception e)
            {
                return Exception("GetGameHistory call failed");
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