using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGGame;
using TRPGServer.Models;

namespace TRPGServer.Hubs
{
    /// <summary>
    /// Signalr hub responsible for sending in-game chat messages to players.
    /// </summary>
    [Authorize]
    public class ChatHub: Hub
    {
        private readonly WorldEntityManager _worldEntityManager;

        public ChatHub(WorldEntityManager worldEntityManager)
        {
            _worldEntityManager = worldEntityManager;
        }

        /// <summary>
        /// Sends a message to other players depending on the message type.
        /// </summary>
        /// <param name="message">A message object containing information about the message's contents and origin.</param>
        /// <returns></returns>
        public async Task SendMessage(Message message)
        {
            string username = Context.User.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            // Todo: Get user role from role manager to check if user is admin
            message.Username = username;
            message.IsUserAdmin = false;
            if (message.MessageType == MessageTypeConstants.Global)
            {
                await Clients.All.SendAsync("receiveMessage", message);
            }
            else if (message.MessageType == MessageTypeConstants.Local)
            {
                var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
                int mapId = manager.GetCurrentMap().Id;
                await Clients.Group($"map:{mapId}").SendAsync("receiveMessage", message);
            }
            else if (message.MessageType == MessageTypeConstants.Private)
            {
                foreach (var client in message.MessageTargets)
                {
                    Clients.User(client).SendAsync("receiveMessage", message);
                }
            }
        }

        /// <summary>
        /// Adds the caller to an appropriate map group depending on which map the caller's entity occupies.
        /// </summary>
        /// <returns></returns>
        public async Task AddToMap()
        {
            var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
            int mapId = manager.GetCurrentMap().Id;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"map:{mapId}");
        }

        /// <summary>
        /// Removes the caller from the old map group and adds it to the map group with the id of the player's entity's
        /// occupied map.
        /// </summary>
        /// <param name="oldMapId">The id of the old map.</param>
        /// <returns></returns>
        public async Task ChangeMapGroup(int oldMapId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"map:{oldMapId}");
            await AddToMap();
        }

        /// <summary>
        /// Removes the caller from a specified group chat given the group id.
        /// </summary>
        /// <param name="groupId">The id of the group chat to remove the caller from.</param>
        /// <returns></returns>
        public Task RemoveFromGroup(string groupId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }
    }
}
