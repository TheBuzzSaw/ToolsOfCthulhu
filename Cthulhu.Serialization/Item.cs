namespace Cthulhu.Serialization
{
    public readonly struct Item
    {
        public readonly int Id { get; }
        public readonly short Count { get; }
        public readonly byte PrefixId { get; }

        public Item(int id, short count, byte prefixId)
        {
            Id = id;
            Count = count;
            PrefixId = prefixId;
        }

        public override string ToString()
        {
            return $"{Id}:{PrefixId} ({Count})";
        }
    }
}