using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class WebUser : IdentityUser
    {
        public List<Comment> Comments { get; set; }

        public WebUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public WebUser(string userName) : this()
        {
            UserName = userName;
        }
    }
}
