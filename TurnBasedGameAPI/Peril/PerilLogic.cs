using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedGameAPI;
using System.Web;
using Peril.Models;
using Peril.Types;

namespace Peril
{
    public class PerilLogic : IGameLogic
    {
        int MinPowerGain = 0;
        int TroopCost = 0;

        //KateBrown 10 mins
        //4-3-2018
        /// <summary>
        /// Determines whether there are a valid number of players.
        /// </summary>
        /// <param name="players">The list of players.</param>
        /// <returns>A boolean indicating whether or not there are exactly two players.</returns>
        public bool TryCreateGame(ref string responseMessage, List<string> players)
        {
            if (players.Count >= 2 && players.Count <= 8)
            {
                return true;
            }
            else
            {
                responseMessage = "Incorrect number of players. Peril only accepts 2-8 players.";
                return false;
            }
        }


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
                    outputGameState = CreateGame(usernameStatusList);

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

                PerilGameState tempGameState = JsonConvert.DeserializeObject<PerilGameState>(currentGameState);

                if (usernameStatusList.Where(x => x.Item1 != callingUsername && x.Item2 == 2).Any()) //any remaining active players
                {
                    Player currentPlayer = tempGameState.Players.Single(x => x.Name == callingUsername);

                    foreach (int i in currentPlayer.TerritoryList)
                    {
                        tempGameState.Territories[i].Owner = "Neutral";
                    }

                    currentPlayer.hexColor = "#a8a8a8";  //Grey

                    tempGameState.TurnOrder.Remove(currentPlayer.TurnPosition); //removes player from TurnOrder list

                    outputGameState = JsonConvert.SerializeObject(tempGameState);

                    return 1;
                }
                else
                {
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

        /// <summary>
        /// Sets initial gameState 
        /// </summary>
        /// Coded by Stephen 
        /// <param name="playerNames"></param>
        public string CreateGame(List<Tuple<string, int>> usernameStatusList)
        {
            PerilGameState thisGame = new PerilGameState();
            // set up territories
            // MakeMap();

            Random rand = new Random();

            int i, j = 0;
            do
            {
                i = rand.Next(usernameStatusList.Count());
                Tuple<string, int> selectedUser = usernameStatusList[i];
                usernameStatusList.Remove(selectedUser);

                Player p = new Player();
                p.Name = selectedUser.Item1;
                p.TurnPosition = j;
                j++;
                thisGame.Players.Add(p);
            } while (usernameStatusList.Count() > 0);

            return JsonConvert.SerializeObject(thisGame);
        }


        public int TryTakeTurn(ref string OutPutGameState, string CurrentGameState, int GameID, string UserName, string RequestedTurn)
        {
            Models.MoveModel move = JsonConvert.DeserializeObject<Models.MoveModel>(RequestedTurn);
            Models.PerilGameStateModel CurGameState = JsonConvert.DeserializeObject<Models.PerilGameStateModel>(CurrentGameState);

            //int retInt = 0;

            //switch (CurGameState.Phase)
            //{
            //    case 0: // case of initial setup phase. placement only of necromancer
            //        if (move.howMany == -1 && move.To.Owner == "Neutral") // -1 to mean move player && To must be Neutral 
            //        {
            //            if (UserName == CurGameState.TurnOrder[CurGameState.CurrentTurn]) // check if is users turn
            //            {
            //                Types.Territory t = move.To;
            //                t.Owner = UserName;
            //                CurGameState.Territories.Add(t);
            //                CurGameState.CurrentTurn++; // this advances the turn order 

            //                retInt = 1;
            //            }
            //        }
            //        // calc front end of turn
            //        CurGameState.CurrentTurn = 0; // set to first players turn
            //        PrepareTurn(JsonConvert.SerializeObject(CurGameState), 0);

            //        break; // end case 0

            //    case 1: // case of allocation
            //        if (CurGameState.TurnOrder[CurGameState.CurrentTurn] == UserName && move.To.Owner == UserName)
            //        { // must be your turn and moving to your territory
            //            if (move.howMany <= CurGameState.Players.Single(x => x.Name == UserName).Unallocated)
            //            { // howMany must be less then or equal to the number of that players unallocated
            //                Types.Territory t = move.To;
            //                t.ForceCount += move.howMany;
            //                CurGameState.Players.Single(x => x.Name == UserName).Unallocated += -move.howMany;

            //                retInt = 1;
            //            }
            //        }

            //        break; // end case 1

            //    case 2: // case of attack
            //        if (CurGameState.TurnOrder[CurGameState.CurrentTurn] == UserName && CurGameState.ActiveBattle != null) { } // show battle screen
            //                                                                                                                      // if its your turn and activeBattle is not null. show battle
            //        if (CurGameState.TurnOrder[CurGameState.CurrentTurn] == UserName && move.From.Owner == UserName && move.To.Owner != UserName)
            //        { // must be your turn and and moving from your territory to a where u arent the owner
            //            if (move.howMany < move.From.ForceCount)
            //            { // howMany must have less then number in territory
            //                Types.Battle thisBattle = new Types.Battle(RequestedTurn);
            //                CurGameState.ActiveBattle = thisBattle;
            //                // show Battle screen
            //                // update final battle results

            //                retInt = 1; 
            //            }
            //        }

            //        break; // end case 2

            //    case 3: // case of movement
            //        if (CurGameState.TurnOrder[CurGameState.CurrentTurn] == UserName && move.To.Owner == UserName)
            //        { // must be your turn and moving to an owned territory
            //            if (move.howMany == -1)// case of leader movement
            //            {
            //                if (!CurGameState.Players.Single(x => x.Name == UserName).leaderMoved)
            //                {
            //                    CurGameState.Players.Single(x => x.Name == UserName).LeaderLocation = move.To.TerritoryNumber;
            //                    CurGameState.Players.Single(x => x.Name == UserName).leaderMoved = true;

            //                    retInt = 1;
            //                }
            //            }
            //            else if (move.From.Moveable >= move.howMany && move.howMany != -1) // case of moving troops
            //            {
            //                Types.Territory t = move.To;
            //                Types.Territory f = move.From;
            //                t.ForceCount += move.howMany;
            //                f.ForceCount = f.ForceCount - move.howMany;
            //                f.Moveable += -move.howMany;

            //                retInt = 1;
            //            }
            //        }

            //        break; // end case 3
            //}
            // serialize here
            OutPutGameState = JsonConvert.SerializeObject(CurGameState);
            // put curGameState into OutPutGameState

            return 0;
        } // end try take turn

        /// <summary>
        /// Sets up the First players, First turn.
        /// </summary>
        /// <param name="CurGamState"></param>
        /// <param name="whomsTurn"></param>
        /// <returns>JSON string GameState.</returns>
        private string PrepareTurn(string CurGamState, int whomsTurn)
        {
            string retString; // holds the string that will be returned
            PerilGameStateModel thisState = JsonConvert.DeserializeObject<PerilGameStateModel>(CurGamState);
            string whom = thisState.Players.Single(x => x.TurnPosition == whomsTurn).Name; // get name of current player
            List<Types.Territory> holder = thisState.Territories.Where(x => x.Owner == whom).ToList(); // gets list of territories owned by player
            int value = 0; // holder for powergain
            foreach (Types.Territory t in holder)
            { value += t.PowerValue; } // FE add PowerValue of each Territory

            value = value * thisState.PowerRate; // Rate * totalPowerValue = totalPowerGain. overlapped storage

            if (value < MinPowerGain) { value = MinPowerGain; } // if you gain less then Min, change to min
            Types.Player p = thisState.Players.Single(x => x.Name == whom); // make copy of player
            p.PowerTotal += value; // add value to players powerTotal

            thisState.Players.Add(p); // add player to thisState, serialize, return
            retString = JsonConvert.SerializeObject(thisState);
            return retString;
        }
    }

}