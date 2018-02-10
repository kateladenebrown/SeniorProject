using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TurnBasedGameAPI.ViewModels
{
    public partial class UserDetailsModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}