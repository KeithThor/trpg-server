using System;
using System.Collections.Generic;
using TRPGServer.Models;

namespace TRPGServer.Services
{
    public interface IDisplayEntityStore
    {
        IEnumerable<DisplayEntity> GetDisplayEntities();
    }
}