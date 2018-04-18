using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PerilUI.Models;

namespace PerilUI.Controllers
{
	public class GamePlayController : Controller
	{
		public ActionResult PerilGamePlay(int id)
		{
			return View(id);
		}
	}
}