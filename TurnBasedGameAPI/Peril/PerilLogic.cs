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
        private int MinPowerGain { get; set; } = 6;
        private int TroopCost { get; set; } = 3;

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
                return RemovePlayer(callingUsername, ref currentGameState, ref outputGameState, ref usernameStatusList);
            }
            else
            {
                return 0;
            }
        }


        // Coded by Tyler Lancaster
        /// <summary>
        /// Removes given user. If only one active player remains, that player wins and game ends.
        /// </summary>
        /// <param name="user"> User to be removed </param>
        /// <param name="currentGameState"> reference </param>
        /// <param name="outputGameState"> reference </param>
        /// <param name="usernameStatusList"> reference </param>
        /// <returns>
        /// 1: Valid status change.
        /// 3: Valid status change, set game to inactive.</returns>
        public int RemovePlayer(string user, ref string currentGameState, ref string outputGameState, ref List<Tuple<string, int>> usernameStatusList)
        {
            PerilGameState tempGameState = JsonConvert.DeserializeObject<PerilGameState>(currentGameState);

            //If there are 2 or more remaining active users, the game is not yet over.
            if (usernameStatusList.Where(x => x.Item1 != user && x.Item2 == 2).Count() > 1)
            {
                Player currentPlayer = tempGameState.Players[user];
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
                tempGameState.Victor = usernameStatusList.Single(x => x.Item1 != user && x.Item2 == 2).Item1; // last remaining active player wins
                outputGameState = JsonConvert.SerializeObject(tempGameState);
                return 3;
            }
        }

        // Coded by Stephen
        // Modified by James
        /// <summary>
        /// Creates the initial game state object.
        /// </summary>
        /// Coded by Stephen 
        /// <param name="playerNames">The list of players in the game.</param>
        public string CreateGame(List<Tuple<string, int>> usernameStatusList)
        {
            PerilGameState thisGame = new PerilGameState();
            SetUpPlayers(usernameStatusList, ref thisGame);
            SetUpTerritories(ref thisGame);
            thisGame.CurrentTurn = thisGame.TurnOrder[0];
            thisGame.Phase = (int)Phase.Setup;
            return JsonConvert.SerializeObject(thisGame);
        }

        // Coded by Stephen, Cameron, & James
        /// <summary>
        /// Add players along with their starting values to the game state
        /// </summary>
        /// <param name="usernameStatusList"></param>
        /// <param name="gameState"></param>
        private void SetUpPlayers(List<Tuple<string, int>> usernameStatusList, ref PerilGameState gameState)
        {
            Random random = new Random();
            // List of possible player colors (8 total)
            List<string> colors = new List<string>();
            colors.Add("#0000FF"); // Blue
            colors.Add("#A52A2A"); // Brown
            colors.Add("#00008B"); // Darkblue
            colors.Add("#008000"); // Green
            colors.Add("#FFA500"); // Orange
            colors.Add("#800080"); // Purple
            colors.Add("#FF0000"); // Red
            colors.Add("#FFFF00"); // Yellow

            // Create players, add initial values to each player, and then add them to game state
            int i = 0;
            int j = 0;
            int k = 0;
            List<int> turnOrder = new List<int>();
            do
            {
                i = random.Next(usernameStatusList.Count());
                Tuple<string, int> selectedUser = usernameStatusList[i];
                usernameStatusList.Remove(selectedUser);

                Player p = new Player();
                p.Name = selectedUser.Item1;
                p.ID = j;
                p.PowerTotal = 18;
                turnOrder.Add(j);
                j++;

                // Add random color to player
                k = random.Next(0, colors.Count);
                p.HexColor = colors[k];
                colors.RemoveAt(k);
                gameState.Players.Add(selectedUser.Item1, p);
            } while (usernameStatusList.Count() > 0);
            gameState.TurnOrder = turnOrder;
        }

        // Coded by James
        /// <summary>
        /// Creates & populates each territory, adds them to dictionary, then adds dictionary to referenced game state
        /// </summary>
        /// <param name="gameState"></param>
        private void SetUpTerritories(ref PerilGameState gameState)
        {
            Dictionary<int, List<int>> connections = CreateTerritoryConnections();
            Dictionary<int, Territory> territories = new Dictionary<int, Territory>();

            foreach(int i in connections.Keys)
            {
                int connectionCount = connections[i].Count;
                Territory territory = new Territory(i, connectionCount, connectionCount, connections[i]);
                territories.Add(i, territory);
            }
            gameState.Territories = territories;
        }

        // Coded by James
        /// <summary>
        /// Creates a dictionary of each territory (key as territory id) to list of connecting territories (value as list of territory ids) 
        /// </summary>
        /// <returns>Dictionary of territory ids and connecting territory ids</returns>
        private Dictionary<int, List<int>> CreateTerritoryConnections()
        {
            Dictionary<int, List<int>> connections = new Dictionary<int, List<int>>
            {
                { 0, new List<int>() { 1, 36 } },
                { 1, new List<int>() { 0, 10, 14 } },
                { 2, new List<int>() { 3 } },
                { 3, new List<int>() { 2, 4 } },
                { 4, new List<int>() { 3, 5, 8, 9, 10 } },
                { 5, new List<int>() { 4, 7, 6 } },
                { 6, new List<int>() { 5 } },
                { 7, new List<int>() { 5, 6, 26 } },
                { 8, new List<int>() { 4, 9, 12 } },
                { 9, new List<int>() { 4, 8, 10, 11, 12 } },
                { 10, new List<int>() { 4, 9, 11, 1 } },
                { 11, new List<int>() { 14, 10, 9, 12 } },
                { 12, new List<int>() { 13, 11, 9, 8 } },
                { 13, new List<int>() { 12, 20, 21 } },
                { 14, new List<int>() { 1, 11, 15 } },
                { 15, new List<int>() { 14, 16, 17, 18 } },
                { 16, new List<int>() { 15, 17 } },
                { 17, new List<int>() { 16, 15 } },
                { 18, new List<int>() { 15, 19 } },
                { 19, new List<int>() { 18 } },
                { 20, new List<int>() { 27, 13, 22 } },
                { 21, new List<int>() { 22, 23, 13 } },
                { 22, new List<int>() { 21, 23, 24, 20 } },
                { 23, new List<int>() { 21, 22, 24, 25 } },
                { 24, new List<int>() { 22, 23, 25, 34 } },
                { 25, new List<int>() { 23, 24 } },
                { 26, new List<int>() { 7, 27, 28 } },
                { 27, new List<int>() { 28, 20, 26 } },
                { 28, new List<int>() { 26, 27, 29, 32 } },
                { 29, new List<int>() { 30, 28, 32, 31 } },
                { 30, new List<int>() { 29, 35 } },
                { 31, new List<int>() { 29, 32, 33, 34 } },
                { 32, new List<int>() { 28, 29, 31, 33 } },
                { 33, new List<int>() { 32, 31, 34 } },
                { 34, new List<int>() { 24, 33, 31, 41 } },
                { 35, new List<int>() { 30, 36, 37, 38, 42 } },
                { 36, new List<int>() { 0, 35, 37 } },
                { 37, new List<int>() { 36, 35, 38, 40 } },
                { 38, new List<int>() { 35, 37, 42, 39, 40 } },
                { 39, new List<int>() { 41, 42, 40, 38 } },
                { 40, new List<int>() { 39, 38, 37 } },
                { 41, new List<int>() { 34, 42, 39 } },
                { 42, new List<int>() { 41, 35, 38 } }
            };

            return connections;
        }

        // Coded by Cameron, Stephen, Tyler, & James
        /// <summary>
        /// Validates and accounts for current players allowable turn options
        /// </summary>
        /// <param name="outPutGameState"></param>
        /// <param name="currentGameState"></param>
        /// <param name="gameID"></param>
        /// <param name="userName"></param>
        /// <param name="requestedTurn"></param>
        /// <returns>0: Invalid request code or 1: Valid request code</returns>
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
                            player.TerritoryList.Add(move.To);

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
                            else // Returns 3 (game inactive) if attack results in game ending
                            {
                                if(Attack(ref curGameState))
                                {
                                    return 3;
                                }
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
                            curGameState.Phase = (int)Phase.Allocation;
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

        // Coded by Cameron, Tyler, & James
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

        // Coded by Stephen
        // Modified by James & Cameron
        /// <summary>
        /// Sets up all player's turns. Counts Territories powervalue times powerGainRate and adds power to players total.
        /// </summary>
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
                value += (int)Math.Floor(curGameState.PowerRate * curGameState.Territories[t].PowerValue);
            }

            player.PowerTotal += (value < MinPowerGain) ? MinPowerGain : value;
        }

        // Coded by Cameron, James, & Tyler
        /// <summary>
        /// Sets territories movable troop counts for the current player
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns>No return.</returns>
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
        /// <returns>No return.</returns>
        private bool Attack(ref PerilGameState gameState)
        {
            Random random = new Random();
            int attackDice = gameState.ActiveBattle.RemainingAttackers >= 3 ? 3 : gameState.ActiveBattle.RemainingAttackers;
            int defenderDice = gameState.ActiveBattle.RemainingDefenders >= 2 ? 2 : gameState.ActiveBattle.RemainingDefenders;
            bool gameOver = false;

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
                gameOver = FinalResult(ref gameState, true);
            }
            else if (gameState.ActiveBattle.RemainingDefenders < 1)
            {
                gameOver = FinalResult(ref gameState, false);
            }

            if (gameOver)
            {
                gameState.Victor = gameState.Players.First().Key;
            }
            return gameOver;
        }

        // Code by Stephen and Tyler
        // Modified by James & Cameron
        /// <summary>
        /// Handles the end battle result
        /// </summary>
        /// <param name="attackerWon"></param>
        private bool FinalResult(ref PerilGameState gameState, bool attackerWon)
        {
            Random random = new Random();
            int revivedTroops = 0;
            bool gameOver = false;

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

                // Handle battle outcome for attacker and defender
                to.Owner = attacker.Name;
                to.ForceCount = gameState.ActiveBattle.RemainingAttackers + revivedTroops;
                attacker.TerritoryList.Add(to.ID);
                from.ForceCount -= gameState.ActiveBattle.Committed;
                defender.TerritoryList.Remove(to.ID);
                if (ShouldRemovePlayer(defender))
                {
                    RemoveDefeatedPlayer(ref gameState, defender.Name);
                    gameOver = IsGameOver(gameState);
                }
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
            return gameOver;
        }

        // Coded by James
        /// <summary>
        /// Determines if player has no territories left & should be removed from the game
        /// </summary>
        /// <param name="player"></param>
        /// <returns>True if player has lost, false otherwise</returns>
        private bool ShouldRemovePlayer(Player player)
        {
            return player.TerritoryList.Count < 1;
        }

        // Coded by James
        /// <summary>
        /// Removes defeated player from turn order and from player list in game state
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="player"></param>
        private void RemoveDefeatedPlayer(ref PerilGameState gameState, string player)
        {
            gameState.TurnOrder.Remove(gameState.Players[player].ID);
            gameState.Players.Remove(player);
        }

        // Coded by James
        /// <summary>
        /// Determines if only 1 player left and game is over
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns>True if game is over, false otherwise</returns>
        private bool IsGameOver(PerilGameState gameState)
        {
            return gameState.Players.Count < 2;
        }
    }
}