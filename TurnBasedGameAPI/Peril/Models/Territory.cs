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

namespace Peril.Models
{
    public class Territory
    {

        /// <summary>
        /// The territory ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The title of the territory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The username for the owner of the territory.
        /// </summary>
        public string Owner { get; set; } 

        /// <summary>
        /// The number of units on the territory.
        /// </summary>
        public int ForceCount { get; set; } 

        /// <summary>
        /// The value of owning the territory.
        /// </summary>
        public int PowerValue { get; set; } 

        /// <summary>
        /// The list of territories (by ID) that are connected to this one.
        /// </summary>
        public List<int> Connections { get; set; } 

        /// <summary>
        /// The number of movable troops on this territory.
        /// </summary>
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
            Connections = Tconnections;
            Owner = _owner;
            PowerValue = _powerValue;
        }
    }
}
