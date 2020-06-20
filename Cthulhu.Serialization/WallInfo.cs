namespace Cthulhu.Serialization
{
    public readonly struct WallInfo
    {
        public readonly string Name { get; }
        public readonly int WallId { get; }
        public readonly Color24? Color { get; }
        public readonly int Blend { get; }

        public WallInfo(int wallId, string name, Color24? color, int blend)
        {
            WallId = wallId;
            Name = name;
            Color = color;
            Blend = blend;
        }

        public override string ToString() => $"{Name} ({WallId})";
    }
}