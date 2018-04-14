/*
 * RiZk Battle Json Object
 * Used to hold the data needed for the current active battle. 
 * Attack() used to calculate the next round of combat using the built in rules.
 * 
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peril.Models
{
    public class Battle
    {
        /// <summary>
        /// The ID of the territory under attack.
        /// </summary>
        public int ToTerritory { get; set; }

        /// <summary>
        /// The ID of the territory the attacking troops are coming from.
        /// </summary>
        public int FromTerritory { get; set; }

        /// <summary>
        /// How many attackers have been lost in the battle.
        /// </summary>
        public int AttackersLost { get; set; } = 0;

        /// <summary>
        /// How many defenders have been lost in the battle.
        /// </summary>
        public int DefendersLost { get; set; } = 0;

        /// <summary>
        /// The number of troops committed to the battle.
        /// </summary>
        public int Committed { get; set; } = 0;

        /// <summary>
        /// The number of attackers still alive.
        /// </summary>
        public int RemainingAttackers
        {
            get
            {
                return Committed - AttackersLost;
            }
        }

        /// <summary>
        /// The number of defenders still alive.
        /// </summary>
        public int RemainingDefenders
        {
            get
            {
                return Committed - DefendersLost;
            }
        }

        public Battle(MoveModel move)
        {
            FromTerritory = move.From;
            ToTerritory = move.To;
            Committed = move.HowMany;
        }
    }
}