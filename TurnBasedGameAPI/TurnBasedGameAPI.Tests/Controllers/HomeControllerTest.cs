using System.Web.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnBasedGameAPI;
using TurnBasedGameAPI.Controllers;

namespace TurnBasedGameAPI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }

        // Written by Michael Case, Feb. 1, 2018
        [TestMethod]
        public void TestGameControllers()
        {
            // Arrange
            GameController controller = new GameController();

            DateTime currentDate = DateTime.Today;

            TestGame game1 = new TestGame(currentDate, "A game", "start");

            TestGameState gameState1 = new TestGameState("start", 1, 1, DateTime.Today);
            TestGameState gameState2 = new TestGameState("made a move", 2, 1, DateTime.Today);

            // Act
            game1.addState(gameState1);
            game1.addState(gameState2);

            // Assert
            


        }
    }
}
