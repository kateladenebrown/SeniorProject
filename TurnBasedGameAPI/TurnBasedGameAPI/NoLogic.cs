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
        public int TryTakeTurn(ref string outputGameState, string currentGameState, int gameId, string callingUsername, string requestedTurn)
        {
            throw new NotImplementedException();
        }

        public int TryUpdateUserStatus(ref string outputGameState, string currentGameState, int gameId, List<Tuple<string, int>> usernameStatusList, string callingUsername, int requestedStatus)
        {
            throw new NotImplementedException();
        }
    }
}