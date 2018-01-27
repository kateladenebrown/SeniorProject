using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace TurnBasedGameAPI.Controllers
{
    public class UserController : ApiController
    {
        // GET: api/User/GetAll
        /// <summary>
        /// Returns all Users' usernames
        /// -Written by Garrick 1/22/18
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetAll()
        {
            try
            {
                //var all = db.user.where(active => true)
            }
            catch (Exception e)
            {
                throw new Exception("Error in GetAll API call");
            }
            return Ok("User Controller GetAll API Call");
        }

        //POST: api/User/CreateUser
        /// <summary>
        /// Returns whether or not a user was created successfully.
        /// Handles account reactivation as well
        /// -Written by Garrick 1/23/18
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult CreateUser() //(Username, Password, Email, Name, etc)
        {
            try
            {
                //add user to db
            }
            catch (Exception e)
            {
                throw new Exception("Error in CreateUser API call");
            }
            return Ok("User Controller CreateUser API Call");
        }

        // GET: api/Game/GetPeronalData
        /// <summary>
        /// Used to gather all personal data about a user.
        /// </summary>
        /// coded by Stephen Bailey 1/24/18
        /// <param name="id"></param>
        /// <returns>  Returns all personal data about a user.</returns>
        public IHttpActionResult GetPersonalDetails(UserId id)
        {
            // ask database for personal details from tables matching the user id 
            try
            {
                using (var db = new Game.ENTITIES()) 
                {
                    User u = db.Users.Single(u => u.UserId = id);
                }
                
                return Ok( u ); // returns a user object from db matching id.
            }
            catch (Exception e) { return Exception("Failure in getPersonalData"); }
        }

        // GET: api/Game/DeleteUser
        /// <summary>
        /// Used to mark users as inactive. Toggles boolean "active" to false
        /// </summary>
        /// coded by Stephen Bailey 1/24/18
        /// <param name="id"></param>
        /// <returns>IHttp result code</returns>
        public IHttpActionResult DeleteUser(UserId id)
        {
            // tell database to toggle active to false on user matching 'id'
            try
            {
                using (var db = new Game.ENTITIES()) 
                {
                    User u = db.Users.Single(u => u.UserId = id);
                    u.Active = false ;
                    db.SaveChanges();
                    return Ok("Game Controller deleteUser API Call"); //  ?? would returning a bool help with this return ??        
                }
            }
            catch (Exception e) { return Exception("failure to deleteUser"); }
        }

        // GET: api/User
        /// <summary>
        /// Returns public information of a single user 
        /// based on passed in ID.
        /// @Michael Case, 1/23/18
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetByUserID(UserID id) // custom object, int, etc for id? implementation decision for later
        {
            try
            {
                // This is a stub. It won't work, syntax, names, etc, are wrong,
                // but should be similar to the final implementation
                using (var db = new TurnBasedGameAPI.ENTITIES())
                {
                    var user = db.users.where(user => user.id);
                    return Ok("User Controller GetByUserID API Call");
                }
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        // PUT: api/User
        /// <summary>
        /// Updates a users password, while verifying that
        /// the new password meets the minimum strength
        /// requirements.
        /// @Michael Case, 1/23/18
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public IHttpActionResult UpdatePassword(String password)
        {
            // Use another function to check password validity
            if (CheckPassword(password))
            {
                return Ok("User Controller UpdatePassword API Call");
            }
            else
            {
                return Exception("UpdatePassword call failed");
            }
        }


    }
}