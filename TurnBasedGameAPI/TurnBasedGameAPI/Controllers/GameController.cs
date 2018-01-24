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
        public IHttpActionResult GetGameHistory(GameID id)
        {
            return Ok("Game Controller GetGameHistory API Call");
        }
    }
}