using System;

namespace Cthulhu.Serialization
{
    public readonly struct Point32 : IEquatable<Point32>
    {
        public readonly int X;
        public readonly int Y;

        public Point32(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point32 other) => X == other.X && Y == other.Y;
        public override bool Equals(Object obj) => obj is Point32 other && Equals(other);

        public override int GetHashCode()
        {
            // https://stackoverflow.com/a/720282
            unchecked
            {
                int hash = 27;
                hash = (13 * hash) + X;
                hash = (13 * hash) + Y;
                return hash;
            }
        }

        public override string ToString() => $"{X}, {Y}";
    }
}