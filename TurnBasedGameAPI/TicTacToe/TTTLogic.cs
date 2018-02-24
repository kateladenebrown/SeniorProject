using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedGameAPI;
using System.Web;

namespace TicTacToe
{
	public class TTTLogic : IGameLogic
	{
		//Implemented by Michael Case, 02-22-2018
		//Edited by Todd Clark, 02-24-2018. Changed implementation to work with JSON.
		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputGameState"></param>
		/// <param name="currentGameState"></param>
		/// <param name="gameId"></param>
		/// <param name="callingUsername"></param>
		/// <param name="requestedTurn"></param>
		/// <returns></returns>
		public int TryTakeTurn(ref string outputGameState, string currentGameState, int gameId, string callingUsername, string requestedTurn)
		{
			TTTGameState tempGameState = JsonConvert.DeserializeObject<TTTGameState>(currentGameState);

			// Return if it's not the user's turn
			if (tempGameState.CurrentTurnUser != callingUsername)
			{
				return 0;
			}

			// If the spot is empty, proceed to set in JSON
			// Output = Original + new move and set current turn to next
			// in turn order
			int requestedGridPosition;
			if (!int.TryParse(requestedTurn, out requestedGridPosition))
			{
				return 0;
			}

			if (string.IsNullOrWhiteSpace(tempGameState.Grid[requestedGridPosition]))
			{
				tempGameState.Grid[requestedGridPosition] = tempGameState.CurrentTurn;
				int gameStatus = GetGameStatus(ref tempGameState);
				tempGameState.AdvanceTurnOrder();
				outputGameState = JsonConvert.SerializeObject(tempGameState);

				return gameStatus;
			}
			else
			{
				return 0; //Error, requested grid position is full.
			}
		}

		// Michael Case
		// 02-22-2018
		//Edited by Todd Clark 02-24-18. Changed to work with game object instead of string. 
		//This now works with our JSON implementation.
		/// <summary>
		/// Checks the game board for victory or draw condition
		/// </summary>
		/// <param name="gameState">Current state of the game</param>
		/// <returns>3 if game is over, 1 if game continues</returns>
		private int GetGameStatus(ref TTTGameState gameState)
		{
			// I'm not sure how we are implenting the grid yet
			// 1 or 2 dimensional array?
			if (//row 1
				gameState.Grid[0] == gameState.CurrentTurn
				&& gameState.Grid[1] == gameState.CurrentTurn
				&& gameState.Grid[2] == gameState.CurrentTurn
				||
				// row 2
				gameState.Grid[3] == gameState.CurrentTurn
				&& gameState.Grid[4] == gameState.CurrentTurn
				&& gameState.Grid[5] == gameState.CurrentTurn
				||
				// row 3
				gameState.Grid[6] == gameState.CurrentTurn
				&& gameState.Grid[7] == gameState.CurrentTurn
				&& gameState.Grid[8] == gameState.CurrentTurn
				||
				// column 1
				gameState.Grid[0] == gameState.CurrentTurn
				&& gameState.Grid[3] == gameState.CurrentTurn
				&& gameState.Grid[6] == gameState.CurrentTurn
				||
				// column 2
				gameState.Grid[1] == gameState.CurrentTurn
				&& gameState.Grid[4] == gameState.CurrentTurn
				&& gameState.Grid[7] == gameState.CurrentTurn
				||
				// column 3
				gameState.Grid[2] == gameState.CurrentTurn
				&& gameState.Grid[5] == gameState.CurrentTurn
				&& gameState.Grid[8] == gameState.CurrentTurn
				||
				// diagonal 1
				gameState.Grid[0] == gameState.CurrentTurn
				&& gameState.Grid[4] == gameState.CurrentTurn
				&& gameState.Grid[8] == gameState.CurrentTurn
				||
				// diagonal 2
				gameState.Grid[2] == gameState.CurrentTurn
				&& gameState.Grid[4] == gameState.CurrentTurn
				&& gameState.Grid[6] == gameState.CurrentTurn
				)
			{
                gameState.Victor = gameState.CurrentTurnUser;
                gameState.CurrentTurn = "";
                return 3;
			}

			bool boardHasFreeSpace = gameState.Grid.Any(x => string.IsNullOrEmpty(x));

			// Grid full, end game
			if (!boardHasFreeSpace)
			{
                gameState.CurrentTurn = "";
                gameState.Victor = "draw";
                return 3;
			}
			else
			{
				return 1; // continue game
			}
		}

		//Todd Clark
		//02-21-2018
		/// <summary>
		/// Creates initial game state.
		/// </summary>
		private void CreateGame(ref string outputGameState, List<Tuple<string, int>> usernameStatusList)
		{
			Dictionary<string, string> turnOrder = new Dictionary<string, string> {
				{ "x", usernameStatusList[0].Item1 },
				{ "o", usernameStatusList[1].Item1 }
			};
			TTTGameState initialGameState = new TTTGameState(turnOrder);

			outputGameState = JsonConvert.SerializeObject(initialGameState);
		}

		//Todd Clark
		//02-21-2018
		/// <summary>
		/// Attempts to update a users current status in a game. 
		/// Returns the following:
		/// 0: Invalid status change.
		/// 1: Valid status change.
		/// 2: Valid status change, set game to active.
		/// 3: Valid status change, set game to inactive.
		/// </summary>
		/// <param name="outputGameState"></param>
		/// <param name="currentGameState"></param>
		/// <param name="gameId"></param>
		/// <param name="usernameStatusList"></param>
		/// <param name="callingUsername"></param>
		/// <param name="requestedStatus"></param>
		/// <returns></returns>
		public int TryUpdateUserStatus(ref string outputGameState, string currentGameState, int gameId, List<Tuple<string, int>> usernameStatusList, string callingUsername, int requestedStatus)
		{
			int currentUserStatus = usernameStatusList.Single(x => x.Item1 == callingUsername).Item2;
			bool otherPendingUsers = usernameStatusList.Where(x => x.Item1 != callingUsername && x.Item2 == 1).Any();

			if (currentUserStatus == 1 && requestedStatus == 2)
			{
				if (!otherPendingUsers)
				{
					CreateGame(ref outputGameState, usernameStatusList);
					return 2;
				}
				return 1;
			}
			else if (currentUserStatus == 1 && requestedStatus == 3)
			{
				//rejected invite
				return 3;
			}
			else if (currentUserStatus == 2 && requestedStatus == 3)
			{
				TTTGameState tempGameState = JsonConvert.DeserializeObject<TTTGameState>(currentGameState);
				tempGameState.Victor = usernameStatusList.Where(x => x.Item1 != callingUsername).First().Item1;
				outputGameState = JsonConvert.SerializeObject(tempGameState);
				return 3;
			}
			else
			{
				return 0;
			}
		}
	}
}