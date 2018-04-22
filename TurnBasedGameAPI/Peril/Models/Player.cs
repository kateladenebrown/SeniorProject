/* Player
 * Coded by:
 * Tyler Lancaster
 * Stephen Bailey
 * 
 * Holds data for each player
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peril.Models
{
    public class Player
    {
        /// <summary>
        /// The player's ID
        /// </summary>
        public int ID { get; set; } = -1;

        /// <summary>
        /// The player's username.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of territories owned by the player (by ID).
        /// </summary>
        public List<int> TerritoryList { get; set; }

        /// <summary>
        /// The player's power bank.
        /// </summary>
        public int PowerTotal { get; set; }

        /// <summary>
        /// The ID of the territory the leader is on.
        /// </summary>
        public int LeaderLocation { get; set; } = -1;

        /// <summary>
        /// Whether or not the leader has moved this turn.
        /// </summary>
        public bool LeaderMoved { get; set; } = false;

        public int LeaderCost { get; set; } = 6; 

        /// <summary>
        /// The player's color.
        /// </summary>
        public string HexColor { get; set; }

    }
}
