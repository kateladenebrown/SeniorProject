using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedGameAPI
{
    //Kate Brown
    //02-13-2018
    /// <summary>
    /// 
    /// </summary>
    public interface IGameLogic
    {

        //All
        //3-17-2018
        /// <summary>
        /// Determines whether the conditions to create a game have been met.
        /// </summary>
        /// <param name="responseMessage">A location to store a response message for the user.</param>
        /// <param name="players">The list of all players, including those invited to the new game and the initiator.</param>
        /// <returns>A boolean value indicating the validity.</returns>
        bool TryCreateGame(ref string responseMessage, List<string> players);

        //Kate Brown
        //02-13-2018
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputGameState"></param>
        /// <param name="currentGameState"></param>
        /// <param name="gameId"></param>
        /// <param name="callingUsername"></param>
        /// <param name="requestedTurn"></param>
        /// <returns></returns>
        int TryTakeTurn(ref string outputGameState, string currentGameState, int gameId, string callingUsername, string requestedTurn );

        //Kate Brown
        //02-13-2018
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputGameState"></param>
        /// <param name="currentGameState"></param>
        /// <param name="gameId"></param>
        /// <param name="usernameStatusList"></param>
        /// <param name="callingUsername"></param>
        /// <param name="requestedStatus"></param>
        /// <returns></returns>
        int TryUpdateUserStatus(ref string outputGameState, string currentGameState, int gameId, List<Tuple<string, int>> usernameStatusList, string callingUsername, int requestedStatus);
    }
}
