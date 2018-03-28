using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Peril.Models;

namespace Peril
{
	public class PerilGameState
	{
		//#region properties
		//public string CurrentTurn { get; set; }
		//public Dictionary<string, string> TurnOrder { get; set; }
		//public string[] Grid { get; set; }
		//public string Victor { get; set; }
		//#endregion

		//public string CurrentTurnUser
		//{
		//	get
		//	{
		//		return TurnOrder[CurrentTurn];
		//	}
		//}

		//public PerilGameState(Dictionary<string, string> turnOrder)
		//{
		//	CurrentTurn = "x";
		//	Grid = new string[9];
		//	TurnOrder = turnOrder;
		//}

		//public void AdvanceTurnOrder()
		//{
		//	CurrentTurn = (CurrentTurn == "x") ? "o" : "x";
		//}

        public List<Types.Player> Players { get; set; } //Do we want the whole object here or just a list of names?

        public Types.Battle ActiveBattle { get; set; }

        public int Phase { get; set; }

        public List<Types.Territory> Territories { get; set; }


        public string[] TurnOrder { get; set; } //Standardize

        public int PowerRate { get; set; }

        public int GameStatus { get; set; } //As with GameID, not sure this is needed here. can be used to note if in battle.

        public int TurnPosition { get; set; }

        public string Victor { get; set; }

        //This is what actually sets the gamestate
        // not totally set yet on what needs be set here 
        // 
        public PerilGameState()
        {
            // unpack curTurn gameState JSON into holder object
        }

        ///// <summary>
        ///// Handles turn order. 
        ///// </summary>
        ///// Coded by Stephen
        ///// <returns>Returns int of player whoms turn it is.</returns>
        //public string turnPosition()
        //{
        //    if (TurnPosition > Players.Count - 1)
        //    {
        //        TurnPosition = 0;
        //        return TurnOrder[TurnPosition];
        //    }
        //    else { return TurnOrder[TurnPosition]; }

        //}

        /// <summary>
        /// Sets initial gameState 
        /// </summary>
        /// Coded by Stephen 
        /// <param name="playerNames"></param>
        public string SetUp(List<string> playerNames)
        {
            string retString = ""; // for return JSON
            PerilGameState thisGame = new PerilGameState();
            // set up territories
            // MakeMap();

            int i = 0;
            foreach (string name in playerNames)
            { // set up for each player
                Types.Player p = new Types.Player();
                p.Name = name;
                p.TurnPosition = i;
                i++;
                thisGame.Players.Add(p);
            }
            thisGame.Phase = 0;
            retString = JsonConvert.SerializeObject(thisGame);
            return retString;
        }


    }
}