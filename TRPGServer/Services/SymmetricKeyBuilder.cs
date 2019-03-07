using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRPGServer.Services
{
    public class SymmetricKeyBuilder
    {
        private readonly IConfiguration _config;
        private string signingKey;
        public SymmetricKeyBuilder(IConfiguration config)
        {
            _config = config;
        }

        public SymmetricSecurityKey GetSymmetricKey()
        {
            if (string.IsNullOrEmpty(signingKey))
            {
                using (var reader = new StreamReader(_config.GetSection("FileLocations")["Keys"]))
                {
                    string part1 = Environment.GetEnvironmentVariable("ASPNETCORE_SIGNINGKEYPART");
                    string part2 = reader.ReadToEnd();
                    signingKey = part1 + part2;
                }
            }

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        }
    }
}
