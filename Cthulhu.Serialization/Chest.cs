using System.Collections.Immutable;

namespace Cthulhu.Serialization
{
    public readonly struct Chest
    {
        public readonly ImmutableArray<Item> Items { get; }
        public readonly string Name { get; }
        public readonly int X { get; }
        public readonly int Y { get; }

        internal Chest(int x, int y, string name, ImmutableArray<Item> items)
        {
            X = x;
            Y = y;
            Name = name;
            Items = items;
        }

        public override string ToString()
        {
            var name = string.IsNullOrWhiteSpace(Name) ? string.Empty : $" \"{Name}\"";
            var count = Items.IsDefaultOrEmpty ? 0 : Items.Length;
            var word = count == 1 ? "item" : "items";
            return $"Chest at ({X}, {Y}) [{count} {word}]{name}";
        }
    }
}