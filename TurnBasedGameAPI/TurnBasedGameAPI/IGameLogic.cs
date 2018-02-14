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
        //Kate Brown
        //02-13-2018
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="playerTurn"></param>
        /// <returns></returns>
        bool TakeTurn(string currentGameState, string playerTurn);

        //Kate Brown
        //02-13-2018
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetGameState();
    }
}
