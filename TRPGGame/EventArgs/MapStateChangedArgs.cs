using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.EventArgs
{
    public class MapStateChangedArgs
    {
        public IReadOnlyList<IReadOnlyList<int?>> MapSpaces;
        public IEnumerable<int> Entities;
    }
}
