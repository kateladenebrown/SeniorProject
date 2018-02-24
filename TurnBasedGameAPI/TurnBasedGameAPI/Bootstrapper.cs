using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace TurnBasedGameAPI
{
    public static class Bootstrapper
    {
        //Kate Brown
        //02-13-2018
        /// <summary>
        /// Instantiates the desired game logic class specified in the web.config file.
        /// </summary>
        /// <returns>The newly instantiated game logic as an IGameLogic object.</returns>
        public static IGameLogic GetGameLogic()
        {
            string gameLogicAssembly = WebConfigurationManager.AppSettings["gameLogicAssembly"];
            string gameLogicClass = WebConfigurationManager.AppSettings["gameLogicClass"];

            string assemblyPath = $"{Environment.CurrentDirectory}\\{gameLogicAssembly}";

            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Type type = assembly.GetType(gameLogicClass);

            return Activator.CreateInstance(type) as IGameLogic;
        }
    }
}