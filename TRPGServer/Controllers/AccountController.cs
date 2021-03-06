﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGGame.Managers;
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
        private readonly IStateManager _stateManager;

        public AccountController(TokenBuilder tokenBuilder,
                                 UserManager<ApplicationUser> userManager,
                                 UserBuilder userBuilder,
                                 IStateManager stateManager)
        {
            _tokenBuilder = tokenBuilder;
            _userManager = userManager;
            _stateManager = stateManager;
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
                return BadRequest("Wrong password.");
            }

            var claims = await _userManager.GetClaimsAsync(foundUser);
            var token = _tokenBuilder.CreateToken(foundUser, claims);

            if (_stateManager.GetPlayerState(Guid.Parse(foundUser.Id)) == null)
            {
                _stateManager.SetPlayerMakeCharacter(Guid.Parse(foundUser.Id));
            }

            return new JsonResult(new
            {
                Token = token,
                Username = foundUser.UserName,
                UserId = foundUser.Id
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
                return Conflict("Username taken");
            }

            var userId = Guid.NewGuid();
            var newUser = new ApplicationUser
            {
                UserName = user.Username,
                Id = userId.ToString()
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Player")
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (result.Succeeded)
            {
                result = await _userManager.AddClaimsAsync(newUser, claims);

                if (result.Succeeded)
                {
                    var token = _tokenBuilder.CreateToken(newUser, claims);
                    _stateManager.SetPlayerMakeCharacter(userId);

                    return new JsonResult(new
                    {
                        Token = token,
                        Username = user.Username,
                        UserId = user.Id
                    });
                }
            }
            
            return BadRequest("Couldn't create user.");
        }
    }
}
