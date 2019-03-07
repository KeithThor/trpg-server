using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRPGServer.Hubs
{
    [Authorize]
    public class MapStateHub: Hub
    {
        // Add method to add a client to a client group
    }
}
