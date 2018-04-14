using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peril.Models
{
    public class BattleResult
    {
        /// <summary>
        /// Boolean indicating whether or not the attacker won.
        /// </summary>
        public bool AttackerWon { get; set; } 

        /// <summary>
        /// Number of troops revived after the battle.
        /// </summary>
        public int NumRevived { get; set; } 

        /// <summary>
        /// The number of troops the defender lost.
        /// </summary>
        public int DefLost { get; set; } 

        /// <summary>
        /// The number of tropps the attacker lost.
        /// </summary>
        public int AttLost { get; set; } 

        /// <summary>
        /// The username of the defender.
        /// </summary>
        public string Defender { get; set; } 

        /// <summary>
        /// The username of the attacker.
        /// </summary>
        public string Attacker { get; set; } 
        
        
        public BattleResult(int _attLost, int _defLost, bool _attackerWon, int _numRevived, string _defender, string _attacker)
        {
            AttackerWon = _attackerWon;
            AttLost = _attLost;
            DefLost = _defLost;
            NumRevived = _numRevived;
            Defender = _defender;
            Attacker = _attacker;
        }

    }
}
