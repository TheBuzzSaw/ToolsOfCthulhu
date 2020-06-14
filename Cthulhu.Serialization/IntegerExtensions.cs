namespace Cthulhu.Serialization.Extensions
{
    public static class IntegerExtensions
    {
        public static bool MaskAll(this int n, int mask) => (n & mask) == mask;
        public static bool MaskAll(this ulong n, ulong mask) => (n & mask) == mask;
    }
}