using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peril.Models
{
    class PerilGameStateModel
    {

        public List<Types.Player> Players { get; set; } //Do we want the whole object here or just a list of names?

        public Types.Battle ActiveBattle { get; set; }

        public int Phase { get; set; }

        public List<Types.Territory> Territories { get; set; }


        public string[] TurnOrder { get; set; } //Standardize

        public int PowerRate { get; set; }

        public int GameStatus { get; set; } //As with GameID, not sure this is needed here. can be used to note if in battle.

        public int TurnPosition { get; set; }

        public string Victor { get; set; }

        ////This is what actually sets the gamestate
        //// not totally set yet on what needs be set here 
        //// 
        //public PerilGameState()
        //{
        //    // unpack curTurn gameState JSON into holder object
        //}
    }
}
