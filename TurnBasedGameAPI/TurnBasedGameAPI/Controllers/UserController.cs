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
        // GET: api/User
        public IHttpActionResult GetAll()
        {
            return Ok("User Controller GetAll API Call");
        }

        // GET: api/User
        public IHttpActionResult GetByUserID()
        {
            return Ok("User Controller GetByUserID API Call");
        }

        // PUT: api/User
        public IHttpActionResult UpdatePassword()
        {
            return Ok("User Controller UpdatePassword API Call");
        }


    }
}