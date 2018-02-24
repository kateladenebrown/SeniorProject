using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameEF
{
    public partial class SimpleUserModel
    {
        // Michael Case
        /// <summary>
        /// List of simplified users
        /// </summary>
        /// <param name="users">List of ASPNetUsers</param>
        public SimpleUserModel(List<AspNetUser> users)
        {
            foreach(AspNetUser u in users)
            {
                if (u.Active)
                {
                    usernames.Add(u.UserName);
                    ids.Add(u.Id);
                }
            }
        }

        public List<string> usernames;
        public List<string> ids;
    }
}