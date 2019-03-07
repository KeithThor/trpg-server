using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace TRPGServer.Data
{
    public class ApplicationUser : IdentityUser
    {
        public List<IdentityUserClaim<string>> Claims { get; set; }
    }
}