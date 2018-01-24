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
        // GET: api/Game/GetMyGames
        /// <summary>
        /// Returns all active User games
        /// -Written by Garrick 1/23/18
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetMyGames() //(GameStatus)
        {
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
    }
}