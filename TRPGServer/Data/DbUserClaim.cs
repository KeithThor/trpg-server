using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRPGServer.Data
{
    /// <summary>
    /// Represents a user claim in the database.
    /// </summary>
    public class DbUserClaim
    {
        /// <summary>
        /// The Id of the user claim.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the user claim.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the user claim.
        /// </summary>
        public string Value { get; set; }
    }
}
