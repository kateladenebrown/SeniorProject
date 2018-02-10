using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEF
{
    public partial class UserViewModel
    {
        // Michael Case, Feb. 7, 2018
        /// <summary>
        /// A User View Model of the AspNetUser. Returns an object with the public fields
        /// of the user of interest.
        /// </summary>
        /// <param name="u">AspNetUser in which the public fields will be used to return
        /// a user view model</param>
        public UserViewModel(AspNetUser u)
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
