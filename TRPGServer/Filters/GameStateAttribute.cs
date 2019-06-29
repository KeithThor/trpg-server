using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGGame.Managers;

namespace TRPGServer.Filters
{
    /// <summary>
    /// Identifies an action that can only be accessed if the player's game state matches any of the
    /// given parameters.
    /// </summary>
    public class GameStateAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Identifies an action that can only be accessed if the player's game state matches any of the
        /// given parameters.
        /// </summary>
        /// <param name="validStates">States that allow the player to access the action.</param>
        public GameStateAttribute(params string[] validStates) : base(typeof(GameStateFilter))
        {
            Arguments = new object[] { validStates };
        }

        private class GameStateFilter : IActionFilter
        {
            private readonly IStateManager _stateManager;
            private readonly string[] _validStates;

            public GameStateFilter(IStateManager stateManager, string[] validStates)
            {
                _stateManager = stateManager;
                _validStates = validStates;
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                // Empty, dont care
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var userId = Guid.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var state = _stateManager.GetPlayerState(userId);

                if (!_validStates.Any(vState => vState == state))
                {
                    context.Result = new BadRequestResult();
                }
            }
        }
    }
}
