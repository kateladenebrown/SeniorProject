using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicTacToe
{
	public class PerilGameState
	{
		#region properties
		public string CurrentTurn { get; set; }
		public Dictionary<string, string> TurnOrder { get; set; }
		public string[] Grid { get; set; }
		public string Victor { get; set; }
		#endregion

		public string CurrentTurnUser
		{
			get
			{
				return TurnOrder[CurrentTurn];
			}
		}

		public PerilGameState(Dictionary<string, string> turnOrder)
		{
			CurrentTurn = "x";
			Grid = new string[9];
			TurnOrder = turnOrder;
		}

		public void AdvanceTurnOrder()
		{
			CurrentTurn = (CurrentTurn == "x") ? "o" : "x";
		}
	}
}