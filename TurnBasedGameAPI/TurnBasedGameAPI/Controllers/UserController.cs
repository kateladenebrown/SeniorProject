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