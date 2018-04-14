using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peril.Models;
using Peril.Types;

namespace Peril.Models
{
    class PerilGameState
    {
        public Dictionary<string, Player> Players { get; set; }

        public Battle ActiveBattle { get; set; }

        public BattleResult BattleResult { get; set; }

        /// <summary>
        /// The probability of a troop being revived.
        /// </summary>
        public int ReviveRate { get; set; } = 25;

        public int Phase { get; set; }

        public Dictionary<int, Territory> Territories { get; set; }

        public List<int> TurnOrder { get; set; }

        public int PowerRate { get; set; } = 7;

        public int CurrentTurn { get; set; }

        public string Victor { get; set; };

        public PerilGameState()
        {

        }
    }
}
