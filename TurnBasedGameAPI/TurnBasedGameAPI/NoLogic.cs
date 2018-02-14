using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TurnBasedGameAPI
{
    //Kate Brown
    //02-13-2018
    public class NoLogic : IGameLogic
    {
        public string GetGameState()
        {
            throw new NotImplementedException();
        }

        public bool TakeTurn(string currentGameState, string playerTurn)
        {
            throw new NotImplementedException();
        }
    }
}