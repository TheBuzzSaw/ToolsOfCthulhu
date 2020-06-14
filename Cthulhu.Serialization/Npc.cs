namespace Cthulhu.Serialization
{
    public readonly struct Npc
    {
        public readonly string Name { get; }
        public readonly string Type { get; }
        public readonly int SpriteId { get; }
        public readonly int TownVariationIndex { get; }
        public readonly float X { get; }
        public readonly float Y { get; }
        public readonly int HomeX { get; }
        public readonly int HomeY { get; }
        public readonly bool IsHomeless { get; }

        internal Npc(MemoryReader reader, int version)
        {
            if (190 <= version)
            {
                SpriteId = reader.ReadInt32();
                Type = "default";
            }
            else
            {
                SpriteId = 0;
                Type = reader.ReadBString();
            }

            Name = reader.ReadBString();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            IsHomeless = reader.ReadBoolean();
            HomeX = reader.ReadInt32();
            HomeY = reader.ReadInt32();

            if (213 <= version && reader.ReadBoolean())
                TownVariationIndex = reader.ReadInt32();
            else
                TownVariationIndex = 0;
        }

        public override string ToString() => "NPC " + Name;
    }
}