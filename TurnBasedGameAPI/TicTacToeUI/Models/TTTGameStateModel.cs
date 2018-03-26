using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicTacToeUI.Models
{
    public class TTTGameStateModel
    {
        public int GameId { get; set; }
        public string CurrentTurn { get; set; }
        public Dictionary<string, string> TurnOrder { get; set; }
        public string[] Grid { get; set; }
        public string Victor { get; set; }
        public string CurrentTurnUser { get { return TurnOrder[CurrentTurn]; } }
    }
}