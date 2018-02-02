using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedGameAPI.Tests
{
    class TestGameState
    {
        private string _gameState;
        private int _gameID, _ID;
        private DateTime _timeStamp;

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// Default constructor
        /// </summary>
        public TestGameState()
        {
            GameState = "Initial State";
            GameID = 1;
            ID = 1;
            TimeStamp = DateTime.Now;
        }

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// Overloaded constructor
        /// </summary>
        public TestGameState(string gameState, int id, int gameID, DateTime timeStamp)
        {
            _gameState = gameState;
            _ID = id;
            _gameID = gameID;
            _timeStamp = timeStamp;
        }

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// StartDate property
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
            }
        }

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// StartDate property
        /// </summary>
        public int GameID
        {
            get
            {
                return _gameID;
            }
            set
            {
                _gameID = value;
            }
        }

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// StartDate property
        /// </summary>
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// StartDate property
        /// </summary>
        public string GameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;
            }
        }
    }
}
