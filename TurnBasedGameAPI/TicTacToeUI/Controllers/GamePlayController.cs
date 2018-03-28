using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicTacToeUI.Models;

namespace TicTacToeUI.Controllers
{
    public class GamePlayController : Controller
    {
        public ActionResult TTTGamePlay(int gameID)
        {
            return View(gameID);
        }
    }
}
