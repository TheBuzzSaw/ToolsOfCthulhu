using System;
using System.Collections.Immutable;
using System.Xml;

namespace Cthulhu.Serialization
{
    public class TileInfo
    {
        public static TileInfo Create(XmlReader reader) => Create(reader, new TileInfo());
        private static TileInfo Create(XmlReader reader, TileInfo tileInfo)
        {
            var name = reader["name"];
            tileInfo.Name = name is null ? tileInfo.Name : name;

            var color = reader["color"];

            if (color != null)
                tileInfo.Color = Color24.Parse(color);
            
            tileInfo.IsTransparent = ReadBoolean(reader, "letLight", tileInfo.IsTransparent);
            tileInfo.IsSolid = ReadBoolean(reader, "solid", tileInfo.IsSolid);
            tileInfo.HasExtra = ReadBoolean(reader, "hasExtra", tileInfo.HasExtra);
            tileInfo.IsGrass = ReadBoolean(reader, "isGrass", tileInfo.IsGrass);
            tileInfo.IsStone = ReadBoolean(reader, "isStone", tileInfo.IsStone);
            tileInfo.CanMerge = ReadBoolean(reader, "merge", tileInfo.CanMerge);
            tileInfo.Light = ReadDouble(reader, "light", tileInfo.Light);
            tileInfo.LightR = ReadDouble(reader, "lightr", tileInfo.LightR);
            tileInfo.LightG = ReadDouble(reader, "lightg", tileInfo.LightG);
            tileInfo.LightB = ReadDouble(reader, "lightb", tileInfo.LightB);
            tileInfo.TileId = ReadInt16(reader, "num", tileInfo.TileId);
            tileInfo.U = ReadInt16(reader, "u", tileInfo.U);
            tileInfo.V = ReadInt16(reader, "v", tileInfo.V);
            tileInfo.MinU = ReadInt16(reader, "minu", tileInfo.MinU);
            tileInfo.MinV = ReadInt16(reader, "minv", tileInfo.MinV);
            tileInfo.MaxU = ReadInt16(reader, "maxu", tileInfo.MaxU);
            tileInfo.MaxV = ReadInt16(reader, "maxv", tileInfo.MaxV);

            if (!reader.IsEmptyElement)
            {
                var builder = ImmutableArray.CreateBuilder<TileInfo>();

                while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.Name != "var")
                        throw new InvalidOperationException($"Expected 'var'. Actual '{reader.Name}'.");
                    
                    var clone = tileInfo.Clone();
                    var variant = Create(reader, clone);
                    builder.Add(variant);
                }

                tileInfo.Variants = builder.ToImmutable();
            }

            return tileInfo;
        }

        private static bool ReadBoolean(
            XmlReader reader,
            string attributeName,
            bool defaultValue)
        {
            var text = reader[attributeName];
            return text is null ? defaultValue : text == "1";
        }

        private static double ReadDouble(
            XmlReader reader,
            string attributeName,
            double defaultValue)
        {
            var text = reader[attributeName];
            return text is null ? defaultValue : double.Parse(text);
        }

        private static short ReadInt16(
            XmlReader reader,
            string attributeName,
            short defaultValue)
        {
            var text = reader[attributeName];
            return text is null ? defaultValue : short.Parse(text);
        }
        
        public double Light { get; private set; }
        public double LightR { get; private set; }
        public double LightG { get; private set; }
        public double LightB { get; private set; }
        public ImmutableArray<TileInfo> Variants { get; private set; } = ImmutableArray<TileInfo>.Empty;
        public string Name { get; private set; }
        public Color24? Color { get; private set; }
        public short TileId { get; private set; }
        public short U { get; private set; } = -1;
        public short V { get; private set; } = -1;
        public short MinU { get; private set; } = -1;
        public short MinV { get; private set; } = -1;
        public short MaxU { get; private set; } = -1;
        public short MaxV { get; private set; } = -1;
        public short Blend { get; private set; } = -1;
        public bool HasExtra { get; private set; }
        public bool IsTransparent { get; private set; }
        public bool IsSolid { get; private set; }
        public bool IsStone { get; private set; }
        public bool IsGrass { get; private set; }
        public bool CanMerge { get; private set; }
        public bool IsHighlighting { get; private set; }

        internal TileInfo Find(short u, short v)
        {
            foreach (var variant in Variants)
            {
                if ((variant.U < 0 || variant.U == u) &&
                   (variant.V < 0 || variant.V == v) &&
                   (variant.MinU < 0 || variant.MinU <= u) &&
                   (variant.MinV < 0 || variant.MinV <= v) &&
                   (variant.MaxU < 0 || variant.MaxU > u) &&
                   (variant.MaxV < 0 || variant.MaxV > v))
                {
                    return variant.Find(u, v);
                }
            }
            
            return this;
        }

        private TileInfo Clone() => (TileInfo)MemberwiseClone();
        public override string ToString() => Name;
    }
}