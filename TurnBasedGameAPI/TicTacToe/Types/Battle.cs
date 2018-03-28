/*
 * RiZk Battle Json Object
 * Used to hold the data needed for the current active battle. 
 * Attack() used to calculate the next round of combat using the built in rules.
 * 
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peril.Models;

namespace Peril.Types
{
    public class Battle
    {
        // **  data section  ***

        int ReviveRate = 24; // used to determine howmany units were revived

        Types.Territory Contested; // this maintains its troop count
        Types.Territory From; // where contested is being attacked from
        int AttackerLost = 0;
        int DefenderLost = 0;
        int Commited;

        /// <summary>
        /// Constructor for Battle object. 
        /// Sent BattleData Json to set up. Use Battle.Attack() to 
        /// </summary>
        /// Coded by Stephen 
        /// <param name="battleData"></param>
        public Battle(string battleData) // needs to be another type. 
        {
            MoveModel batDat = JsonConvert.DeserializeObject<MoveModel>(battleData);
            Contested = batDat.To;
            From = batDat.From;
            Commited = batDat.howMany;

            AttackerLost = 0;
            DefenderLost = 0;
        }

        /// <summary>
        /// Helper method for Attack() to determine the highest rolls of the attacker.
        /// </summary>
        /// <param name="ary"></param>
        /// <returns>Tuple where Item1 is highest, Item2 is second highest</returns>
        private Tuple<int, int> FindHighest(int[] ary)
        {
            int last = 0;
            int previous = 0;
            foreach (int i in ary)
            {
                if (ary[i] > last)
                {
                    if (last > previous) { last = previous; }
                    last = ary[i];
                }
            }
            Tuple<int, int> highHolder = Tuple.Create<int, int>(last, previous);
            return highHolder;
        }

        /// <summary>
        /// Used to run attack algorithm when a player attacks another. 
        /// </summary>
        /// Coded by Stephen 
        /// <returns>string<MoveModel.Json> BattleResults</returns>
        public string Attack()
        {
            string BattResults = "";
            Random roller = new Random();

            int attackDice = Commited;
            int defenderDice = Contested.ForceCount;
            if (attackDice > 3) { attackDice = 3; }
            if (defenderDice > 2) { defenderDice = 2; }
            // dice are set at AttackMax 3 DefenderMax 2
            int[] attackRolls = new int[3];
            int[] defRolls = new int[2];
            for (int i = 0; i < attackDice; i++) { attackRolls[i] = roller.Next(0, 99); }
            for (int i = 0; i < defenderDice; i++) { defRolls[i] = roller.Next(0, 99); }
            Tuple<int, int> highestAttackRolls = FindHighest(attackRolls);

            if (defenderDice > 1 && attackDice > 1) // one or two can die this round
            { // case of 2 defender dice
                if (defRolls[0] > defRolls[1]) // which is the larger
                {
                    if (highestAttackRolls.Item1 > defRolls[0]) { DefenderLost++; }
                    else { AttackerLost++; }
                    if (highestAttackRolls.Item2 > defRolls[1]) { DefenderLost++; }
                    else { AttackerLost++; }
                }
                else // defRolls[1] is higher
                {
                    if (highestAttackRolls.Item1 > defRolls[1]) { DefenderLost++; Contested.ForceCount--; }
                    else { AttackerLost++; }
                    if (highestAttackRolls.Item2 > defRolls[0]) { DefenderLost++; Contested.ForceCount--; }
                    else { AttackerLost++; }
                }
            }
            else
            { // case of one side has only 1 attack/defend die, only need the top number
                if (defenderDice == 1)
                {
                    if (highestAttackRolls.Item1 > defRolls[0]) { DefenderLost++; Contested.ForceCount--; }
                    else { AttackerLost++; }
                }
                else
                { // case of attacker has only 1 attack die
                    if (defRolls[0] > defRolls[1])
                    {
                        if (highestAttackRolls.Item1 > defRolls[0]) { DefenderLost++; Contested.ForceCount--; }
                        else { AttackerLost++; }
                    }
                    else
                    {
                        if (highestAttackRolls.Item1 > defRolls[1]) { DefenderLost++; Contested.ForceCount--; }
                        else { AttackerLost++; }
                    }
                }
            }

            if (Contested.ForceCount < 1)
            { return FinalResult(true); }

            Models.MoveModel batRes = new MoveModel();
            batRes.From = From;
            batRes.To = Contested;
            batRes.howMany = Commited - AttackerLost;
            BattResults = JsonConvert.SerializeObject(batRes);
            return BattResults;

            // called when attacker has retreated, or attacker has won.
            //return the final state of the <Territory> Contested and From
        }

        public string FinalResult(bool attackerWon)
            {
                // use moveObject to relate result. 
                // state of each territory ie, troop count/ownership of contested/To
                // howMany is number for revive chance.
                string FinalResult = "";
                MoveModel FinRes = new MoveModel();
                Random roller = new Random();
                if (attackerWon)
                {
                    Contested.Owner = From.Owner;
                    if (true) { } // need to target territorys owner to chk if leader was present

                    From.ForceCount += -(Commited - AttackerLost);
                    int ReVed = 0;
                    //resolve revives as howMany
                    for (int i = 0; i < DefenderLost; i++)
                    { if (roller.Next(0, 99) <= ReviveRate) { ReVed++; } }
                    FinRes.howMany = ReVed;
                    Contested.ForceCount = Commited - AttackerLost;
                }
                else // defender Won
                {
                    FinRes.From.ForceCount += Commited - AttackerLost; // force count for attackers territory set on MoveModel.From
                    FinRes.To = Contested; // copy state of Contested into MoveModel.To 
                    int ReVed = 0; //resolve revives as MoveModel.howMany
                    for (int i = 0; i < AttackerLost; i++)
                    { if (roller.Next(0, 99) <= ReviveRate) { ReVed++; } } // calculate revived
                    FinRes.howMany = ReVed; // set howMany Revived 
                }
                //Active = false;
                FinalResult = JsonConvert.SerializeObject(FinRes);
                return FinalResult;
            }
        }
    }