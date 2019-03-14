using System;
using System.Collections.Generic;
using System.Text;
using TRPGShared;

namespace TRPGGame.EventArgs
{
    public class MapStateChangedArgs
    {
        public IReadOnlyList<IReadOnlyList<int?>> MapSpaces;
        public IReadOnlyDictionary<int, Coordinate> Entities;
    }
}
