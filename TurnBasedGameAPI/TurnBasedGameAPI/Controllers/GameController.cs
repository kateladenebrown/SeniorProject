using GameEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using GameEF;
//using System.Web.Mvc;

namespace TurnBasedGameAPI.Controllers
{
    //[Authorize]
    [RoutePrefix("api/Game")]
    public class GameController : ApiController
    {
        // POST: api/Game/Create
        // Coded by Stephen 2/7/18
        /// <summary>
        /// Starts a new game instance with the caller and listed users as players.
        /// </summary>
        /// <param name="players">A list of usernames specifying who should be a player.</param>
        /// <returns>A message indicating that the game was created successfully, or an error otherwise.</returns>
        [HttpPost]
        [Route("Create", Name = "Create New Game")]
        public IHttpActionResult GameCreate(List<string> players)
        {
            using (var db = new GameEntities())
            {
                try
                {
                    Game g = new Game(); // create a game
                    g.Start = System.DateTime.Now; // set start time to now
                    g.Status = 1; // set to pending status ( 1 )
                    db.Games.Add(g); // add g to Games to generate the Game.ID

                    foreach ( string name in players)
                    {
                        try
                        {
                            GameUser usr = new GameUser(); // make a GameUser holder
                            usr.UserID = db.AspNetUsers.Single(x => x.UserName == name).Id; // find the single ID, where the UserName is the current name
                            usr.GameID = g.ID; 
                            usr.Status = 1; // set each player to pending status
                            g.GameUsers.Add(usr); // add each GameUser iteration to the game instance
                        }
                        catch(Exception e) { return Content( System.Net.HttpStatusCode.BadRequest ,"Failure to create GameUser."); }
                    } // end foreach

                    // actions to make initiating user active and added to list of players
                    AspNetUser tmp = new AspNetUser();
                    tmp = db.AspNetUsers.Single(x => x.UserName == User.Identity.Name);
                    players.Add(tmp.UserName); // adds current player to list of players
                    GameUser u = db.GameUsers.Single(x => x.UserID == tmp.Id );
                    u.UserID = tmp.Id;
                    u.GameID = g.ID;
                    u.Status = 2; // 2 is active
                    g.GameUsers.Add(u);

                    try { db.Games.Add(g); }
                    catch (Exception e) { return Content(System.Net.HttpStatusCode.NotModified, "Failure to add Game to Database."); }

                    try { db.SaveChanges(); } // save changes to db 
                    catch(Exception e) { return Content(System.Net.HttpStatusCode.InternalServerError, "Server failed to save changes. "); }

                    return Ok("Everything went shwimminminingly");
                } 
                catch (Exception e)
                { return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error and was unable to create the game. Please inform the development team."); } // end catch1
            } // end using 
        } // end GameCreate 

        /* testing notes for GameCreate: 
         * Method requires a List<string>. 
         * Send an empty list or one that has a 0 count.
         * Send List<string>  
         * Send List<string> where one userId is invalid $$ no catch for this specifically yet
         */



        // GET: api/Game/MyGames
        // -Written by Garrick 1/23/18
        /// <summary>
        /// Retrieves a list of games the user is or was a player in.
        /// </summary>
        /// <param name="gameStatus">The value of the gamestatus to filter for, default is -1 for ignore</param>
        /// <returns>A list of Game objects.</returns>
        [HttpGet]
        [Route("MyGames", Name = "Get My Games")]
        public IHttpActionResult GetMyGames(int gameStatus = -1) // string? Gamestatus to check for active vs inactive games
        {
            using (var db = new GameEntities())
            {
                try
                {
                    IQueryable<Game> myGames = db.GameUsers
                        .Where(gu => gu.UserID == User.Identity.Name)
                        .Select(g => g.Game);
                    if (gameStatus != -1)
                    {
                        myGames = myGames.Where(x => x.Status == gameStatus);
                    }

                    return Ok(myGames.ToList());
                }
                catch (InvalidOperationException e)//User does not exist
                {
                    return NotFound();
                }
                catch (Exception e)
                {
                    return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error retrieving the list of games. Please inform the development team.");
                }
            }
        }

        // GET: api/Game/GameHistory
        // @Michael Case, 1/23/18
        /// <summary>
        /// Retrieves all game records related to a specific game.
        /// GameID
        /// </summary>
        /// <param name="id">The ID of the game whose history should be returned.</param>
        /// <returns>A list of GameState objects.</returns>
        [HttpGet]
        [Route("GameHistory", Name = "Get Game History")]
        public IHttpActionResult GetGameHistory(int id)
        {
            using (var db = new GameEntities())
            {
                try
                {
                    List<GameState> myGames = db.Games.Single(g => g.ID == id).GameStates.ToList();

                    return Ok(myGames); // return something for the time being
                }
                catch (Exception e)
                {
                    return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error when attempting to retrieve the game history. Please inform the development team.");
                }
                //using (var db = new Game.ENTITIES())
                //{
                //    var gameHistory = db.games.where(gameHistory => GetMyGames.id);
                //    return Ok("Game Controller GetGameHistory API Call");
                //}

                //return Ok("Game Controller GetGameHistory API Call");
            }
            
        }

        // GET: api/Game
        // >Tyler Lancaster, 1/25/18
        /// <summary>
        /// Returns the latest game record for the passed-in GameID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetGame(int id)
        {
            using (var db = new GameEntities())
            {
                try
                {
                    //Note: The database is ordered by timestampt descending, we can use the first record.
                    GameState game = db.GameStates.First(gs => gs.GameID == id);

                    return Ok("Game Controller GetGame API Call");
                }
                catch (Exception e)
                {
                    return Content(System.Net.HttpStatusCode.InternalServerError, "GetGame call failed");
                }
            }
        }
    }
}