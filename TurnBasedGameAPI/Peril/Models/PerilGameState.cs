using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peril.Models;

namespace Peril.Models
{
    class PerilGameState
    {
        public Dictionary<string, Player> Players { get; set; }

        public Battle ActiveBattle { get; set; }

        public BattleResult BattleResult { get; set; }

        // Coded by James, Cameron, Tyler, & Stephen
        /// <summary>
        /// The probability of a troop being revived.
        /// </summary>
        public int ReviveRate
        {
            get
            {
                Random random = new Random();
                return random.Next(1, 34);
            }
        }

        public int Phase { get; set; }

        public Dictionary<int, Territory> Territories { get; set; }

        public List<int> TurnOrder { get; set; }

        public double PowerRate { get; set; } = 1.0;

        public int CurrentTurn { get; set; }

        public string Victor { get; set; }

        public PerilGameState()
        {

        }
    }
}
