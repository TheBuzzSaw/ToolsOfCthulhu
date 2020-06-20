using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml;

namespace Cthulhu.Serialization
{
    public class WorldInfo
    {
        public static WorldInfo FromWorldInfoData(string xml)
        {
            using var stringReader = new StringReader(xml);
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true
            };
            
            using var xmlReader = XmlReader.Create(stringReader, settings);
            var tileInfoById = new List<KeyValuePair<short, TileInfo>>();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "tile":
                        {
                            var tileInfo = TileInfo.Create(xmlReader);
                            tileInfoById.Add(KeyValuePair.Create(tileInfo.TileId, tileInfo));
                            break;
                        }
                    }
                }
            }

            var worldInfo = new WorldInfo
            {
                TileInfoById = tileInfoById.ToImmutableDictionary()
            };

            return worldInfo;
        }

        public ImmutableDictionary<short, TileInfo> TileInfoById { get; private set; }

        private WorldInfo()
        {
        }
    }
}