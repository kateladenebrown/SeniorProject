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
        // GET: api/Game
        public IHttpActionResult Index()
        {
            return Ok("");
        }

        // GET: api/Game
        public IHttpActionResult GetGameHistory(GameID id)
        {
            return Ok("Game Controller GetGameHistory API Call");
        }
    }
}