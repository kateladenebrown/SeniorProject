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

namespace Peril.Types
{
    public class Player
    {
        public string Name { get; set; }                // player username

        public List<int> TerritoryList { get; set; }    // list of territory numbers belonging to player

        public int PowerTotal { get; set; }             // unused power total

        public int LeaderLocation { get; set; }         // denotes which territory the leader is in

        public bool leaderMoved { get; set; }           // denotes if leader has already moved this Movement phase

        public int Unallocated { get; set; }            // troops to be dropped onto field

        public int TurnPosition { get; set; }           // Where in turn order they are

        public string hexColor { get; set; }            // player color

    }
}
