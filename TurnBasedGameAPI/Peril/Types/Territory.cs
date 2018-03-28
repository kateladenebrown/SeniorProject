/*
 * Territoy
 * Used to hold data about a territory for RiZk.
 * Originally Coded by:
 * Stephen Bailey
 * Tyler Lan(d)caster
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peril.Types
{
    public class Territory
    {
        public string Name { get; set; } // title of territory
        public string Owner { get; set; } // which player owns this
        public int ForceCount { get; set; } // number of units on board
        public int PowerValue { get; set; } // currency value of this territory
        public int TerritoryNumber { get; set; }
        public List<int> TConnections { get; set; } // the title of territories that are connected to this one
        public int Moveable { get; set; }

        /// <summary>
        /// no Arg constructor
        /// </summary>
        public Territory() { }

        /// <summary>
        /// Used to construct each territory.
        /// </summary>
        /// Coded by Stephen
        /// <param name="_name"></param>
        /// <param name="Tconnections"></param>
        /// <param name="force"></param>
        /// <param name="_owner"></param>
        /// <param name="_powerValue"></param>
        public void TerritoryBuild(string _name, List<int> Tconnections, int force, string _owner, int _powerValue)
        {
            Name = _name;
            ForceCount = force;
            TConnections = Tconnections;
            Owner = _owner;
            PowerValue = _powerValue;
        }
    }
}
