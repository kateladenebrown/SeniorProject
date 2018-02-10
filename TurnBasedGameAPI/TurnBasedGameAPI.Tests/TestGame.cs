using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Edited by Michael Case. I took James' code because it was already there. I
// integrated the game states
namespace TurnBasedGameAPI.Tests
{
    class TestGame
    {
        DateTime _startDate, _endDate;
        private string _gameData, _updateGame, _gameStatus;
        private List<TestGameState> _gameStates;

        // Written by James 1/31/18
        /// <summary> 
        /// Default constructor initializes start date to today's date, game data to null, and game status to Pending
        /// </summary>
        public TestGame()
        {
            StartDate = DateTime.Today;
            GameData = null;
            GameStatus = "Pending";
        }

        /// <summary>
        /// Overloaded constructor initializes game data to parameter values
        /// </summary>
        /// <param name="gameData"></param>
        public TestGame(DateTime startDate, string gameData, string gameStatus = "Pending")
        {
            StartDate = startDate;
            GameData = gameData;
            GameStatus = gameStatus;
        }

        // Written by James 2/1/18
        /// <summary>
        /// Property for starting date of game
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        // Written by James 2/1/18
        /// <summary>
        /// Property for ending date of game
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        // Written by James 2/1/18
        /// <summary>
        /// Property for game status (Pending, Active, or Inactive)
        /// </summary>
        public string GameStatus
        {
            get
            {
                return _gameStatus;
            }
            set
            {
                _gameStatus = value;
            }
        }

        // Written by James 1/31/18
        /// <summary>
        /// Property for game data such as (maybe) how many users are playing, turn order for players etc.
        /// </summary>
        public string GameData
        {
            get
            {
                return _gameData;
            }
            set
            {
                _gameData = value;
            }
        }

        // Written by James 2/1/18
        /// <summary>
        /// Property for updating game data after each player turn
        /// </summary>
        public string UpDateGame
        {
            get
            {
                return _updateGame;
            }
            set
            {
                _updateGame += value;
            }
        }

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// GameStates property
        /// </summary>
        public List<TestGameState> GameStates
        {
            get
            {
                return _gameStates;
            }
            set
            {
                _gameStates = value;
            }
        }

        // Michael Case, Feb 1, 2018
        /// <summary>
        /// Add a new GameState to the game
        /// </summary>
        /// <param name="gs"></param>
        public void addState(TestGameState gs)
        {
            _gameStates.Add(gs);
        }
    }
}