using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TurnBasedGameAPI.Models
{
    public class UserDetailsModel
    {
        public UserDetailsModel (string FName, string LName, string UsrName, string EMl)
        {
            FirstName = FName;
            LastName = LName;
            UserName = UsrName;
            EMail = EMl;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }

    }
}