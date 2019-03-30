using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGServer.Data;
using TRPGServer.Models;
using TRPGServer.Services;

namespace TRPGServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenBuilder _tokenBuilder;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(TokenBuilder tokenBuilder,
                                 UserManager<ApplicationUser> userManager,
                                 UserBuilder userBuilder)
        {
            _tokenBuilder = tokenBuilder;
            _userManager = userManager;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Modelstate invalid");
            }

            var foundUser = await _userManager.FindByNameAsync(user.Username);
            if (foundUser == null)
            {
                return BadRequest("No user was found with that username.");
            }

            var success = await _userManager.CheckPasswordAsync(foundUser, user.Password);
            if (!success)
            {
                return BadRequest("The wrong password was entered.");
            }

            var claims = await _userManager.GetClaimsAsync(foundUser);
            var token = _tokenBuilder.CreateToken(foundUser, claims);

            return Json(new
            {
                Token = token,
                Username = user.Username
            });
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Modelstate invalid");
            }

            if (await _userManager.FindByNameAsync(user.Username) != null)
            {
                return Conflict("A user with that username already exists.");
            }

            var newUser = new ApplicationUser
            {
                UserName = user.Username,
                Id = Guid.NewGuid().ToString()
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Player")
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (result.Succeeded)
            {
                result = await _userManager.AddClaimsAsync(newUser, claims);

                if (result.Succeeded)
                {
                    var token = _tokenBuilder.CreateToken(newUser, claims);
                    
                    return Json(new
                    {
                        Token = token,
                        Username = user.Username
                    });
                }
            }
            
            return BadRequest("Couldn't create user.");
        }
    }
}
