using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using GameEF;
//using System.Web.Mvc;

namespace TurnBasedGameAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        // GET: api/User/GetAll
        // -Written by Garrick 1/22/18
        /// <summary>
        /// Retrieves a list of all active user's usernames.
        /// </summary>
        /// <returns>A list of usernames.</returns>
        [HttpGet]
        [Route("GetAll", Name = "Get All Users")]
        public IHttpActionResult GetAll()
        {
            try
            {
                //var all = db.user.where(active => true)
                return Ok("User Controller GetAll API Call");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error while attempting to retrieve the list of users. Please inform the development team.");
            }
        }

        // GET: api/User/GetPersonalDetails
        // Coded by Stephen 1/24/18
        /// <summary>
        /// Retrieves personal details for the currently logged in user.
        /// </summary>
        /// <returns>A single User object.</returns>
        [Authorize]
        [HttpGet]
        [Route("GetPersonalDetails", Name = "Get User Personal Details")]
        public IHttpActionResult GetPersonalDetails()
        {
            User u = new User();
            try
            {
                using (var db = new GameEntities())
                {
                    u = db.Users.Single(x => x.Username == User.Identity.Name );
                }
                return Ok(u);
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server was unable to retrieve your personal details. Please inform the development team.");
            }
        } // end GetPersonalDetails

        //POST: api/User/Create
        // -Written by Garrick 1/23/18
        /// <summary>
        /// Creates a new user account with the specified details.
        /// (Handles account reactivation as well.)
        /// </summary>
        /// <returns>A message indicating that the user was created/re-activated successfully, or an error otherwise.</returns>
        [HttpPost]
        [Route("Create", Name = "Create New User")]
        public IHttpActionResult CreateUser() //(Username, Password, Email, Name, etc)
        {
            try
            {
                //add user to db
                return Ok("User Controller CreateUser API Call");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error attempting to create the user account. Please inform the development team.");
            }
        }

        // GET: api/User/{id}
        // @Michael Case, 1/23/18
        /// <summary>
        /// Retrieve's the publicly visible information for a single user.
        /// </summary>
        /// <param name="id">The ID for the user whose details should be returned.</param>
        /// <returns>A single User object (with only the publicly visible fields populated.</returns>
        [HttpGet]
        [Route("{id}", Name = "Get User By ID")]
        public IHttpActionResult GetByUserID(int id)
        {
            try
            {
                // This is a stub. It won't work, syntax, names, etc, are wrong,
                // but should be similar to the final implementation

                // -Cameron: looks good. Commented out for now so that the solution will build.

                //using (var db = new TurnBasedGameAPI.ENTITIES())
                //{
                //    var user = db.users.where(user => user.id);
                //    return Ok("User Controller GetByUserID API Call");
                //}

                return Ok("User Controller GetByUserID API Call");
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error attempting to retrieve the user details. Please inform the development team.");

                // -Cameron: NotFound is great to use, but should not be in the outer-most catch block; it should be returned if the database doesn't contain an entry with the ID specified.
                //return NotFound();
            }
        }

        // PUT: api/User/UpdatePassword
        // @Michael Case, 1/23/18
        /// <summary>
        /// Updates a users password, while verifying that
        /// the new password meets the minimum strength
        /// requirements.
        /// </summary>
        /// <param name="password">The new password as a string.</param>
        /// <returns>A message indicating that the password was updated successfully, or an error otherwise.</returns>
        [HttpPut]
        [Route("UpdatePassword", Name = "Update Password")]
        public IHttpActionResult UpdatePassword(string password)
        {
            // Use another function to check password validity
            if (1 == 1) // (CheckPassword(password))
            {
                return Ok("User Controller UpdatePassword API Call");
            }
            else
            {
                return BadRequest("The password specified does not meet the complexity requirements.");
            }
        }

        // DELETE: api/User/Delete
        // coded by Stephen 1/24/18
        /// <summary>
        /// Deactivates the current users account.
        /// </summary>
        /// <returns>A message indicating that the account was successfully deactivated, or an error otherwise.</returns>
        [HttpDelete]
        [Route("Delete", Name = "Delete User Account")]
        public IHttpActionResult deleteUser(int id)
        {
            // tell database to toggle active to false on user matching 'id'
            try
            {
                using (var db = new GameEntities())
                {
                    db.Users.Single(x => x.ID == id).Active = false; // set active to false
                    db.SaveChanges();   // save changes made to db back to main DataBase
                }           
                    return Ok(); // ??do we want to return a bool or something??
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, "The server encountered an error while attempting to deactive the account. Please inform the development team.");
            }
        }

        // GET: api/User/GetActive
        // >Tyler Lancaster, 1/25/18
        /// <summary>
        /// Returns all active users' usernames
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetActive()
        {
            try
            {
                //var activeUsers = db.user.where(active => true);
            }
            catch (Exception e)
            {
                throw new Exception("Error in GetActive API call");
            }
            return activeUserList;
        }

        // PUT: api/User
        // >Tyler Lancaster, 1/25/18
        /// <summary>
        /// Updates user's publicly-available information
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult UpdatePersonalDetails(User user)
        {
            try
            {
                //check user authentication or assume user has access?
                //update username
                //What other details are available for change?
            }
            catch (Exception e)
            {
                throw new Exception("Error in UpdatePersonalDetails API call");
            }
            return Ok("User Controller UpdatePersonalDetails API Call");
        }
    }
}