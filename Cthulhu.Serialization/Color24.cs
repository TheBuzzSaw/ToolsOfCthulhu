using System;

namespace Cthulhu.Serialization
{
    public readonly struct Color24 : IEquatable<Color24>
    {
        private const string HexDigits = "0123456789abcdef";
        private static char GetHighDigit(byte value) => HexDigits[value >> 4];
        private static char GetLowDigit(byte value) => HexDigits[value & 0xf];
        
        private static int ParseHex(char c)
        {
            if ('0' <= c && c <= '9')
                return c - '0';
            else if ('a' <= c && c <= 'f')
                return c - 'a' + 10;
            else if ('A' <= c && c <= 'F')
                return c - 'A' + 10;
            else
                throw new ArgumentException("Not a valid hex digit", nameof(c));
        }

        private static byte ParseHex(char a, char b)
        {
            return (byte)((ParseHex(a) << 4) | ParseHex(b));
        }

        public static Color24 Parse(ReadOnlySpan<char> text)
        {
            if (text.Length != 7)
                throw new ArgumentException("Input must contain 7 characters", nameof(text));
            
            if (text[0] != '#')
                throw new ArgumentException("Input must start with '#'.", nameof(text));
            
            return new Color24(
                ParseHex(text[1], text[2]),
                ParseHex(text[3], text[4]),
                ParseHex(text[5], text[6]));
        }

        public readonly byte R;
        public readonly byte G;
        public readonly byte B;

        public Color24(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public bool Equals(Color24 other) => R == other.R && G == other.G && B == other.B;
        public override bool Equals(object obj) => obj is Color24 other && Equals(other);
        public override int GetHashCode() => (R << 16) | (G << 8) | B;

        public override string ToString()
        {
            return string.Create(7, this, (span, state) =>
            {
                span[0] = '#';
                span[1] = GetHighDigit(state.R);
                span[2] = GetLowDigit(state.R);
                span[3] = GetHighDigit(state.G);
                span[4] = GetLowDigit(state.G);
                span[5] = GetHighDigit(state.B);
                span[6] = GetLowDigit(state.B);
            });
        }
    }
}