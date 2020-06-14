namespace Cthulhu.Serialization
{
    public readonly struct Sign
    {
        public readonly string Text { get; }
        public readonly int X { get; }
        public readonly int Y { get; }

        public Sign(int x, int y, string text)
        {
            X = x;
            Y = y;
            Text = text;
        }

        public override string ToString() => $"Sign at ({X}, {Y}): \"{Text}\"";
    }
}