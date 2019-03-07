using System.Collections.Generic;

namespace TRPGServer.Models
{
    /// <summary>
    /// A data object representing an in-game chat message and it's contents.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Contains the username of the user who sent this message.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contains the contents of this message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Returns true if the user who sent this message is an admin.
        /// </summary>
        public bool IsUserAdmin { get; set; }

        /// <summary>
        /// Represents the type of message being sent, ie 'private', 'local', 'global'.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The usernames of the targets of this message, if any.
        /// </summary>
        public IEnumerable<string> MessageTargets { get; set; }
    }
}
