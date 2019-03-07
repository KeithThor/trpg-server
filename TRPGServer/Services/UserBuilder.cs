using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGServer.Data;
using TRPGServer.Models;

namespace TRPGServer.Services
{
    /// <summary>
    /// Class responsible for building ApplicationUsers.
    /// </summary>
    public class UserBuilder
    {
        /// <summary>
        /// Creates and returns a default application user.
        /// </summary>
        /// <param name="user">The model data to be used to create the user.</param>
        /// <returns>A default application user.</returns>
        public ApplicationUser CreateUser(User user)
        {
            return new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = user.Username
            };
        }
    }
}
