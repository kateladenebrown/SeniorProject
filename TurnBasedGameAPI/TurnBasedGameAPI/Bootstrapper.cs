using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TurnBasedGameAPI
{
    public static class Bootstrapper
    {

        //Kate Brown
        //02-13-2018
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IGameLogic GetGameLogic()
        {
            //Change this line to point to a different IGameLogic class.
            return new NoLogic();
        }
    }
}