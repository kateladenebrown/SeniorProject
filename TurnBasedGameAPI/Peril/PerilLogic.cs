using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedGameAPI;
using System.Web;
using Peril.Models;

namespace Peril
{
	public class PerilLogic : IGameLogic
	{
        int MinPowerGain = 0;


		////Implemented by Michael Case, 02-22-2018
		////Edited by Todd Clark, 02-24-2018. Changed implementation to work with JSON.
		///// <summary>
		///// 
		///// </summary>
		///// <param name="outputGameState"></param>
		///// <param name="currentGameState"></param>
		///// <param name="gameId"></param>
		///// <param name="callingUsername"></param>
		///// <param name="requestedTurn"></param>
		///// <returns></returns>
		//public int TryTakeTurn(ref string outputGameState, string currentGameState, int gameId, string callingUsername, string requestedTurn)
		//{
		//	PerilGameState tempGameState = JsonConvert.DeserializeObject<PerilGameState>(currentGameState);

		//	// Return if it's not the user's turn
		//	if (tempGameState.CurrentTurnUser != callingUsername)
		//	{
		//		return 0;
		//	}

		//	// If the spot is empty, proceed to set in JSON
		//	// Output = Original + new move and set current turn to next
		//	// in turn order
		//	int requestedGridPosition;
		//	if (!int.TryParse(requestedTurn, out requestedGridPosition))
		//	{
		//		return 0;
		//	}

		//	if (string.IsNullOrWhiteSpace(tempGameState.Grid[requestedGridPosition]))
		//	{
		//		tempGameState.Grid[requestedGridPosition] = tempGameState.CurrentTurn;
		//		int gameStatus = GetGameStatus(ref tempGameState);
		//		tempGameState.AdvanceTurnOrder();
		//		outputGameState = JsonConvert.SerializeObject(tempGameState);

		//		return gameStatus;
		//	}
		//	else
		//	{
		//		return 0; //Error, requested grid position is full.
		//	}
		//}

		//// Michael Case
		//// 02-22-2018
		////Edited by Todd Clark 02-24-18. Changed to work with game object instead of string. 
		////This now works with our JSON implementation.
		///// <summary>
		///// Checks the game board for victory or draw condition
		///// </summary>
		///// <param name="gameState">Current state of the game</param>
		///// <returns>3 if game is over, 1 if game continues</returns>
		//private int GetGameStatus(ref PerilGameState gameState)
		//{
		//	// I'm not sure how we are implenting the grid yet
		//	// 1 or 2 dimensional array?
		//	if (//row 1
		//		gameState.Grid[0] == gameState.CurrentTurn
		//		&& gameState.Grid[1] == gameState.CurrentTurn
		//		&& gameState.Grid[2] == gameState.CurrentTurn
		//		||
		//		// row 2
		//		gameState.Grid[3] == gameState.CurrentTurn
		//		&& gameState.Grid[4] == gameState.CurrentTurn
		//		&& gameState.Grid[5] == gameState.CurrentTurn
		//		||
		//		// row 3
		//		gameState.Grid[6] == gameState.CurrentTurn
		//		&& gameState.Grid[7] == gameState.CurrentTurn
		//		&& gameState.Grid[8] == gameState.CurrentTurn
		//		||
		//		// column 1
		//		gameState.Grid[0] == gameState.CurrentTurn
		//		&& gameState.Grid[3] == gameState.CurrentTurn
		//		&& gameState.Grid[6] == gameState.CurrentTurn
		//		||
		//		// column 2
		//		gameState.Grid[1] == gameState.CurrentTurn
		//		&& gameState.Grid[4] == gameState.CurrentTurn
		//		&& gameState.Grid[7] == gameState.CurrentTurn
		//		||
		//		// column 3
		//		gameState.Grid[2] == gameState.CurrentTurn
		//		&& gameState.Grid[5] == gameState.CurrentTurn
		//		&& gameState.Grid[8] == gameState.CurrentTurn
		//		||
		//		// diagonal 1
		//		gameState.Grid[0] == gameState.CurrentTurn
		//		&& gameState.Grid[4] == gameState.CurrentTurn
		//		&& gameState.Grid[8] == gameState.CurrentTurn
		//		||
		//		// diagonal 2
		//		gameState.Grid[2] == gameState.CurrentTurn
		//		&& gameState.Grid[4] == gameState.CurrentTurn
		//		&& gameState.Grid[6] == gameState.CurrentTurn
		//		)
		//	{
  //              gameState.Victor = gameState.CurrentTurnUser;
  //              gameState.CurrentTurn = "";
  //              return 3;
		//	}

		//	bool boardHasFreeSpace = gameState.Grid.Any(x => string.IsNullOrEmpty(x));

		//	// Grid full, end game
		//	if (!boardHasFreeSpace)
		//	{
  //              gameState.CurrentTurn = "";
  //              gameState.Victor = "draw";
  //              return 3;
		//	}
		//	else
		//	{
		//		return 1; // continue game
		//	}
		//}

		////Todd Clark
		////02-21-2018
		///// <summary>
		///// Creates initial game state.
		///// </summary>
		//private void CreateGame(ref string outputGameState, List<Tuple<string, int>> usernameStatusList)
		//{
		//	Dictionary<string, string> turnOrder = new Dictionary<string, string> {
		//		{ "x", usernameStatusList[0].Item1 },
		//		{ "o", usernameStatusList[1].Item1 }
		//	};
		//	PerilGameState initialGameState = new PerilGameState(turnOrder);

		//	outputGameState = JsonConvert.SerializeObject(initialGameState);
		//}

        //Tyler Lancaster (framework by Todd Clark)
        /// <summary>
        /// Attempts to update a users current status in a game. 
        /// </summary>
        /// <param name="outputGameState"></param>
        /// <param name="currentGameState"></param>
        /// <param name="gameId"></param>
        /// <param name="usernameStatusList"></param>
        /// <param name="callingUsername"></param>
        /// <param name="requestedStatus"></param>
        /// <returns>
        /// 0: Invalid status change.
        /// 1: Valid status change.
        /// 2: Valid status change, set game to active.
        /// 3: Valid status change, set game to inactive.
        /// </returns>
        public int TryUpdateUserStatus(ref string outputGameState, string currentGameState, int gameId, List<Tuple<string, int>> usernameStatusList, string callingUsername, int requestedStatus)
        {
            int currentUserStatus = usernameStatusList.Single(x => x.Item1 == callingUsername).Item2;
            bool otherPendingUsers = usernameStatusList.Where(x => x.Item1 != callingUsername && x.Item2 == 1).Any();

            if (currentUserStatus == 1 && requestedStatus == 2)
            {
                if (otherPendingUsers)
                {
                    return 1;
                }
                else
                {
                    //CreateGame(ref outputGameState, usernameStatusList);

                    return 2;
                }
            }
            else if (currentUserStatus == 1 && requestedStatus == 3)
            {
                //rejected invite
                return 3;
            }
            else if (currentUserStatus == 2 && requestedStatus == 3)
            {
                if (usernameStatusList.Where(x => x.Item1 != callingUsername && x.Item2 == 2).Any()) //any remaining active players
                {
                    //callingUsername.troops.owner = Neutral;

                    PerilGameState tempGameState = JsonConvert.DeserializeObject<PerilGameState>(currentGameState);
                    outputGameState = JsonConvert.SerializeObject(tempGameState);

                    return 1;
                }
                else
                {
                    PerilGameState tempGameState = JsonConvert.DeserializeObject<PerilGameState>(currentGameState);
                    tempGameState.Victor = usernameStatusList.Single(x => x.Item1 != callingUsername && x.Item2 == 2).Item1; // last remaining active player wins
                    outputGameState = JsonConvert.SerializeObject(tempGameState);

                    return 3;
                }

            }
            else
            {
                return 0;
            }
        }

        ///// <summary>
        ///// Sets initial gameState 
        ///// </summary>
        ///// Coded by Stephen 
        ///// <param name="playerNames"></param>
        //public string CreateGame(ref string outputGameState, List<Tuple<string, int>> usernameStatusList)
        //{
        //    string retString = ""; // for return JSON
        //    PerilGameState thisGame = new PerilGameState();
        //    // set up territories
        //    // MakeMap();

        //    int i = 0;
        //    foreach (string name in usernameStatusList)
        //    { // set up for each player
        //        Types.Player p = new Types.Player();
        //        p.Name = name;
        //        p.TurnPosition = i;
        //        i++;
        //        thisGame.Players.Add(p);
        //    }
        //    thisGame.Phase = 0;
        //    retString = JsonConvert.SerializeObject(thisGame);
        //    return retString;
        //}

        public int TryTakeTurn(ref string OutPutGameState, string CurrentGameState, int GameID, string UserName, string RequestedTurn)
        {
            Models.MoveModel move = JsonConvert.DeserializeObject<Models.MoveModel>(RequestedTurn);
            Models.PerilGameStateModel CurGameState = JsonConvert.DeserializeObject<Models.PerilGameStateModel>(CurrentGameState);
            int retInt = 0; // zero is invalid request code, will return this unless move works
            if(UserName == CurGameState.TurnOrder[CurGameState.TurnPosition])
            {
                switch (CurGameState.Phase)
                {
                    case 0: // case of initial setup phase. placement only of necromancer
                        if (move.howMany == -1 && move.To.Owner == "Neutral" && CurGameState.Players.Single(x=> x.LeaderLocation == null))  
                        { // -1 to mean move player && To must be Neutral && must not have leader location already
                            // call to chk if last player to make this move,true: advance to phase 1 turn order 0
                            if(CurGameState.Players.Select(x=> x.LeaderLocation != null).Sum() != CurGameState.Players.Sum() +1  )
                            { // case of not last player to place leader
                                CurGameState.Territories.Single(x=> x.Name == move.To.Name).Owner = UserName; // find territory matching that name, change owner to username
                                CurGameState.TurnPosition++; // this advances the turn order
                                retInt = 1;
                            }
                            else 
                            { // case of last player to place leader
                                CurGameState.Territories.Single(x=> x.Name == move.To.Name).Owner = UserName; // find territory matching that name, change owner to username
                                CurGameState.TurnPosition = 0 ; // change to phase 1, first players turn
                                CurGameState.Phase = 1; 
                                CurGameState = PrepareTurn(CurGameState, CurGameState.TurnPosition);
                                retInt = 1;
                            }
                                                   
                        }

                        break; // end case 0

                    case 1: // case of allocation
                        // need to add from off board to unallocated purchasing options here
                        if (CurGameState.TurnOrder[CurGameState.TurnPosition] == UserName && move.To.Owner == UserName)
                        { // must be your turn and moving to your territory
                            if (move.howMany <= CurGameState.Players.Single(x => x.Name == UserName).Unallocated)
                            { // howMany must be less then or equal to the number of that players unallocated
                                Types.Territory t = move.To;
                                t.ForceCount += move.howMany;
                                CurGameState.Players.Single(x => x.Name == UserName).Unallocated += -move.howMany;
                                retInt = 1;
                            }
                        }
                        break; // end case 1
                    
                    // Needs work
                    case 2: // case of attack
                        if (CurGameState.TurnOrder[CurGameState.TurnPosition] == UserName && CurGameState.ActiveBattle != null) { } // show battle screen
                                                                                                                                  // if its your turn and activeBattle is not null. show battle
                        if ( move.From.Owner == UserName && move.To.Owner != UserName)
                        { // must be moving from your territory to a where u arent the owner
                            if (move.howMany < move.From.ForceCount)
                            { // howMany must have less then number in territory
                                Types.Battle thisBattle = new Types.Battle(RequestedTurn);
                                CurGameState.ActiveBattle = thisBattle;
                                // show Battle screen
                                // update final battle results

                                retInt = 1; 
                            }
                        }

                        break; // end case 2
                    
                    // Should be complete
                    case 3: // case of movement
                        if (move.To.Owner == UserName  )
                        { // must be moving to an owned territory
                            if (move.howMany == -1 && !CurGameState.Players.Single(x => x.Name == UserName).leaderMoved )
                            {   // case of leader movement                                                         
                                CurGameState.Players.Single(x => x.Name == UserName).LeaderLocation = move.To.TerritoryNumber;
                                CurGameState.Players.Single(x => x.Name == UserName).leaderMoved = true;
                                retInt = 1;
                            }
                            else if (move.From.Moveable >= move.howMany && move.howMany != -1)
                            { // case of moving troops 
                                if(move.howMany < CurGameState.Territories.Single(x=> x.Name == move.From.Name ).ForceCount )
                                { // must have fewer troops moving than are in From Territory
                                    CurGameState.Territories.Single(x => x.Name == move.To.Name).ForceCount += move.howMany;
                                    CurGameState.Territories.Single(x=> x.Name == move.From.Name).ForceCount += -move.howMany;                               
                                    retInt = 1;
                                }
                            }
                        }
                        break; // end case 3
                }
            }
            OutPutGameState = CurGameState;
            return retInt;
        } // end try take turn

        /// <summary>
        /// Sets up all player's turns.
        /// </summary>
        /// <param name="CurGamState"></param>
        /// <param name="whomsTurn"></param>
        /// <returns>JSON string GameState.</returns>
        private Models.PerilGameStateModel PrepareTurn(Models.PerilGameStateModel CurGamState, int whomsTurn)
        {
            string whom = CurGamState.Players.Single(x => x.TurnPosition == whomsTurn).Name; // get name of current player
            if(CurGamState.TurnOrder[CurGamState.TurnPosition] == whom ) // if its the sending players turn
            { // this chk to prevent call going through for wrong player, or Web request attempting to bypass normal chks                
                List<Types.Territory> holder = CurGamState.Territories.Where(x => x.Owner == whom).ToList(); // gets list of territories owned by player
                int value = 0; // holder for powergain
                foreach (Types.Territory t in holder)
                { value += t.PowerValue; } // FE add PowerValue of each Territory
                value = value * CurGamState.PowerRate; // Rate * totalPowerValue = totalPowerGain. overlapped storage
                if (value < MinPowerGain) { value = MinPowerGain; } // if you gain less then Min, change to min            
                CurGamState.Players.Single(x => x.Name == whom).PowerTotal += value; // add power to Power Total
            } // else return no changes
            return CurGamState;
        }
    }
    
}