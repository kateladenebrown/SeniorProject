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
        private int MinPowerGain { get; set; } = 0;
        private int TroopCost { get; set; } = 0;

        enum Phase { Setup, Allocation, Attack, Move };
        // This enum handles the negative values for the HowMany in Move Modal. Unsure if we wanted to seperate these
        enum Move { Leader = -1, EndTurn = -2, Retreat = -3 };

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

            //Using is pending and requesting a move to "active"
            if (currentUserStatus == 1 && requestedStatus == 2)
            {
                if (otherPendingUsers)
                {
                    return 1;
                }
                else //If there are no other pending users, start the game.
                {
                    outputGameState = CreateGame(usernameStatusList);

                    return 2;
                }
            }
            //User rejected the invite.
            else if (currentUserStatus == 1 && requestedStatus == 3)
            {
                return 3;
            }
            //User forfeited the game.
            else if (currentUserStatus == 2 && requestedStatus == 3)
            {
                PerilGameState tempGameState = JsonConvert.DeserializeObject<PerilGameState>(currentGameState);

                //If there are 2 or more remaining active users, the game is not yet over.
                if (usernameStatusList.Where(x => x.Item1 != callingUsername && x.Item2 == 2).Count() > 1)
                {
                    Player currentPlayer = tempGameState.Players[callingUsername];

                    foreach (int i in currentPlayer.TerritoryList)
                    {
                        tempGameState.Territories[i].Owner = null;
                    }

                    currentPlayer.HexColor = "#a8a8a8";  //Grey

                    tempGameState.TurnOrder.Remove(currentPlayer.ID);

                    outputGameState = JsonConvert.SerializeObject(tempGameState);

                    return 1;
                }
                else //There is only one other active player left, so the game is over.
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
        /// Creates the initial game state object.
        /// </summary>
        /// Coded by Stephen 
        /// <param name="playerNames">The list of players in the game.</param>
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
                p.ID = j;
                j++;
                thisGame.Players.Add(selectedUser.Item1, p);
            } while (usernameStatusList.Count() > 0);

            return JsonConvert.SerializeObject(thisGame);
        }

        public int TryTakeTurn(ref string outPutGameState, string currentGameState, int gameID, string userName, string requestedTurn)
        {
            MoveModel move = JsonConvert.DeserializeObject<MoveModel>(requestedTurn);
            PerilGameState curGameState = JsonConvert.DeserializeObject<PerilGameState>(currentGameState);
            Player player = curGameState.Players[userName];
            int retInt = 0; // zero is invalid request code, will return this unless move works

            // Check if it is the user's turn.
            if (player.ID == curGameState.TurnOrder[curGameState.CurrentTurn])
            {
                Territory to = (curGameState.Territories.ContainsKey(move.To)) ? curGameState.Territories[move.To] : null;
                Territory from = (curGameState.Territories.ContainsKey(move.From)) ? curGameState.Territories[move.From] : null;

                switch (curGameState.Phase)
                {
                    case (int)Phase.Setup:

                        //If the location the user is trying to claim is available (and they haven't yet chosen), give them the territory.
                        if (to.Owner == null && player.LeaderLocation == -1)
                        {
                            to.Owner = userName; // change owner to username
                            retInt = 1;

                            if (IncrementTurnOrder(ref curGameState))
                            {
                                curGameState.Phase = (int)Phase.Allocation;
                                CalculatePowerGain(ref curGameState);
                            }
                        }
                        break;

                    case (int)Phase.Allocation:

                        if (player.LeaderLocation == -1 && move.From == (int)Move.Leader)
                        {
                            if (player.PowerTotal >= player.LeaderCost && player.TerritoryList.Contains(move.To))
                            {
                                player.PowerTotal -= player.LeaderCost;
                                player.LeaderCost = player.LeaderCost * 2;
                                player.LeaderLocation = move.To;
                                retInt = 1;
                            }
                        }
                        else if (player.LeaderLocation != -1 && move.HowMany > 0)
                        {
                            // Check if the player has enough currency to make the purchase.
                            int CostHolder = move.HowMany * TroopCost;
                            if (player.PowerTotal >= CostHolder)
                            {
                                player.PowerTotal -= CostHolder;
                                curGameState.Territories[player.LeaderLocation].ForceCount += move.HowMany;
                                retInt = 1;
                            }
                        }
                        else if (move.HowMany == (int)Move.EndTurn)
                        {
                            curGameState.Phase = (int)Phase.Attack;
                            retInt = 1;
                        }
                        break;

                    case (int)Phase.Attack:

                        //If there is a battle going on and there is not yet a victor:
                        if (curGameState.ActiveBattle != null)
                        {
                            if (move.HowMany == (int)Move.Retreat)
                            {
                                FinalResult(ref curGameState, false);
                            }
                            else
                            {
                                Attack(ref curGameState);
                            }
                            break;
                        }
                        //Validate battle creation call.
                        else if (from.Owner == userName
                            && to.Owner != userName
                            && move.HowMany < from.ForceCount
                            && move.HowMany > 0
                            && from.Connections.Contains(move.To))
                        {
                            curGameState.ActiveBattle = new Battle(move);
                            retInt = 1;
                        }
                        else if (move.HowMany == (int)Move.EndTurn)
                        {
                            curGameState.Phase = (int)Phase.Move;
                            SetMovableTroops(ref curGameState);
                            retInt = 1;
                        }
                        
                        break;

                    case (int)Phase.Move:
                        if (to.Owner != userName)
                        {
                            break;
                        }

                        if (move.HowMany == (int)Move.Leader && !player.LeaderMoved)
                        {
                            player.LeaderLocation = move.To;
                            player.LeaderMoved = true;
                        }
                        else if (move.HowMany <= from.Moveable && move.HowMany > 0 && (from.ForceCount - move.HowMany) > 0)
                        {
                            to.ForceCount += move.HowMany;
                            from.ForceCount -= move.HowMany;
                            from.Moveable -= move.HowMany;
                        }
                        else if (move.HowMany == (int)Move.EndTurn)
                        {
                            IncrementTurnOrder(ref curGameState);
                            CalculatePowerGain(ref curGameState);
                        }
                        else
                        {
                            break;
                        }

                        retInt = 1;
                        break;
                }
            }

            outPutGameState = JsonConvert.SerializeObject(curGameState);

            return retInt;
        }

        /// <summary>
        /// Increments the turn order, looping back to the start if needed.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <returns>Whether or not turn order reset.</returns>
        private bool IncrementTurnOrder(ref PerilGameState gameState)
        {
            int nextTurnIndex = gameState.TurnOrder.IndexOf(gameState.CurrentTurn) + 1;

            if (nextTurnIndex >= gameState.TurnOrder.Count())
            {
                gameState.CurrentTurn = gameState.TurnOrder[0];
                return true;
            }
            else
            {
                gameState.CurrentTurn = gameState.TurnOrder[nextTurnIndex];
                return false;
            }
        }

        /// <summary>
        /// Sets up all player's turns. Counts Territories powervalue times powerGainRate and adds power to players total.
        /// </summary>
        /// Coded by Stephen
        /// <param name="CurGamState"></param>
        /// <param name="currentTurn"></param>
        /// <returns>No return. Acts upon ref to CurGameState</returns>
        private void CalculatePowerGain(ref PerilGameState curGameState)
        {
            int currentTurn = curGameState.CurrentTurn;
            Player player = curGameState.Players.Single(x => x.Value.ID == currentTurn).Value;

            int value = 0; // holder for powergain

            foreach (int t in player.TerritoryList)
            {
                value += curGameState.Territories[t].PowerValue * curGameState.PowerRate;
            }

            player.PowerTotal += (value < MinPowerGain) ? MinPowerGain : value;
        }

        private void SetMovableTroops(ref PerilGameState gameState)
        {
            int currentTurn = gameState.CurrentTurn;
            Player player = gameState.Players.Single(x => x.Value.ID == currentTurn).Value;
            player.LeaderMoved = false;

            foreach (int i in player.TerritoryList)
            {
                Territory t = gameState.Territories[i];
                t.Moveable = t.ForceCount;
            }
        }


        // Coded by Stephen
        // Modified by James
        /// <summary>
        /// Used to run attack algorithm when a player attacks another. 
        /// </summary>
        private void Attack(ref PerilGameState gameState)
        {
            Random random = new Random();
            int attackDice = gameState.ActiveBattle.RemainingAttackers >= 3 ? 3 : gameState.ActiveBattle.RemainingAttackers;
            int defenderDice = gameState.ActiveBattle.RemainingDefenders >= 2 ? 2 : gameState.ActiveBattle.RemainingDefenders;

            int[] attackRolls = {
                    random.Next(1, 7),
                    random.Next(1, 7),
                    random.Next(1, 7)
                };
            Array.Sort(attackRolls);

            int[] defendRolls = {
                    random.Next(1, 7),
                    random.Next(1, 7)
                };
            Array.Sort(defendRolls);

            // Count losses
            int counter = (attackDice <= defenderDice) ? attackDice : defenderDice;
            for (int i = 0; i < counter; i++)
            {
                if (attackRolls[attackDice - 1 - i] < defendRolls[defenderDice - 1 - i])
                {
                    gameState.ActiveBattle.AttackersLost++;
                }
                else
                {
                    gameState.ActiveBattle.DefendersLost++;
                }
            }


            if (gameState.ActiveBattle.RemainingAttackers < 1)
            {
                FinalResult(ref gameState, true);
            }
            else if (gameState.ActiveBattle.RemainingDefenders < 1)
            {
                FinalResult(ref gameState, false);
            }
        }

        // Code by Stephen and Tyler
        // Modified by James
        /// <summary>
        /// Handles the end battle result
        /// </summary>
        /// <param name="attackerWon"></param>
        private void FinalResult(ref PerilGameState gameState, bool attackerWon)
        {
            Random random = new Random();
            int revivedTroops = 0;

            Territory from = gameState.Territories[gameState.ActiveBattle.FromTerritory];
            Territory to = gameState.Territories[gameState.ActiveBattle.Committed];
            Player attacker = gameState.Players[from.Owner];
            Player defender = gameState.Players[to.Owner];

            if (attackerWon == true)
            {
                // Determine how many troops to revive based on % out of 100
                for (int i = 0; i < gameState.ActiveBattle.DefendersLost; i++)
                {
                    if (random.Next(1, 101) <= gameState.ReviveRate)
                    {
                        revivedTroops++;
                    }
                }

                // Defenders leader killed/taken
                if (defender.LeaderLocation == to.ID)
                {
                    defender.LeaderLocation = -1;
                }

                // Place all committed & resurrected troops to new territory
                to.Owner = attacker.Name;
                to.ForceCount = gameState.ActiveBattle.RemainingAttackers + revivedTroops;
                // Remove committed troops for attacking territory
                from.ForceCount -= gameState.ActiveBattle.Committed;
            }
            else
            {
                // Determine how many troops to revive based on % out of 100
                for (int i = 0; i < gameState.ActiveBattle.AttackersLost; i++)
                {
                    if (random.Next(1, 101) <= gameState.ReviveRate)
                    {
                        revivedTroops++;
                    }
                }
                // Set territories new troop counts
                to.ForceCount = gameState.ActiveBattle.RemainingDefenders + revivedTroops;
                from.ForceCount -= gameState.ActiveBattle.AttackersLost;
            }

            gameState.BattleResult = new BattleResult(gameState.ActiveBattle.AttackersLost, gameState.ActiveBattle.DefendersLost, attackerWon, revivedTroops, defender.Name, attacker.Name);
            gameState.ActiveBattle = null;
        }

    }
}