﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGShared
{
    /// <summary>
    /// Represents a point in an X-Y based graph.
    /// </summary>
    public class Coordinate
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Coordinate))
            {
                return false;
            }

            var coordinate = (Coordinate)obj;
            return PositionX == coordinate.PositionX &&
                   PositionY == coordinate.PositionY;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PositionX, PositionY);
        }

        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return c1.PositionX == c2.PositionX && c1.PositionY == c2.PositionY;
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return !(c1 == c2);
        }
    }
}
