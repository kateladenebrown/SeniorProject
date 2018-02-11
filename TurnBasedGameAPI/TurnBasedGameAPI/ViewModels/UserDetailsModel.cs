using GameEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TurnBasedGameAPI.ViewModels
{
    public partial class UserDetailsViewModel
    {
        /// <summary>
        /// A User View Model of the AspNetUser. Returns an object with the public and private fields
        /// of the user of interest, but without the protected fields like password hash.
        /// </summary>
        /// <param name="u">AspNetUser in which the public fields will be used to return
        /// a user view model</param>
        public UserDetailsViewModel(AspNetUser u)
        {
            Email = u.Email;
            FirstName = u.FirstName;
            LastName = u.LastName;
            PhoneNumber = u.PhoneNumber;
            UserName = u.UserName;
        }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}