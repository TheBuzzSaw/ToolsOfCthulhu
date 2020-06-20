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
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            
            using var xmlReader = XmlReader.Create(stringReader, settings);
            var tileInfoById = new List<KeyValuePair<int, TileInfo>>();
            var itemNameById = new List<KeyValuePair<int, string>>();
            var npcById = new List<KeyValuePair<int, string>>();
            var prefixById = new List<KeyValuePair<int, string>>();
            var wallInfoById = new List<KeyValuePair<int, WallInfo>>();
            var global = new List<KeyValuePair<string, Color24>>();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "tile":
                        {
                            var tileInfo = TileInfo.Create(xmlReader);
                            tileInfoById.Add(KeyValuePair.Create((int)tileInfo.TileId, tileInfo));
                            break;
                        }

                        case "item":
                        {
                            var itemId = int.Parse(xmlReader["num"]);
                            var itemName = xmlReader["name"];
                            itemNameById.Add(KeyValuePair.Create(itemId, itemName));
                            break;
                        }

                        case "Npc":
                        {
                            var npcId = int.Parse(xmlReader["Id"]);
                            var npcName = xmlReader["Name"];
                            npcById.Add(KeyValuePair.Create(npcId, npcName));
                            break;
                        }

                        case "prefix":
                        {
                            var prefixId = int.Parse(xmlReader["num"]);
                            var prefixName = xmlReader["name"];
                            prefixById.Add(KeyValuePair.Create(prefixId, prefixName));
                            break;
                        }

                        case "wall":
                        {
                            var color = xmlReader["color"];
                            var blend = xmlReader["blend"];

                            var wallInfo = new WallInfo(
                                int.Parse(xmlReader["num"]),
                                xmlReader["name"],
                                color is null ? default(Color24?) : Color24.Parse(color),
                                blend is null ? -1 : int.Parse(blend));
                            
                            wallInfoById.Add(KeyValuePair.Create(wallInfo.WallId, wallInfo));
                            break;
                        }

                        case "global":
                        {
                            global.Add(
                                KeyValuePair.Create(
                                    xmlReader["id"],
                                    Color24.Parse(xmlReader["color"])));
                            break;
                        }
                    }
                }
            }

            var worldInfo = new WorldInfo
            {
                TileInfoById = tileInfoById.ToImmutableDictionary(),
                ItemNameById = itemNameById.ToImmutableDictionary(),
                NpcById = npcById.ToImmutableDictionary(),
                PrefixById = prefixById.ToImmutableDictionary(),
                WallInfoById = wallInfoById.ToImmutableDictionary(),
                Global = global.ToImmutableDictionary()
            };

            return worldInfo;
        }

        public ImmutableDictionary<int, TileInfo> TileInfoById { get; private set; }
        public ImmutableDictionary<int, string> ItemNameById { get; private set; }
        public ImmutableDictionary<int, string> NpcById { get; private set; }
        public ImmutableDictionary<int, string> PrefixById { get; private set; }
        public ImmutableDictionary<int, WallInfo> WallInfoById { get; private set; }
        public ImmutableDictionary<string, Color24> Global { get; private set; }

        private WorldInfo()
        {
        }

        public TileInfo FindTileInfo(int id, short u, short v)
        {
            if (TileInfoById.TryGetValue(id, out var tileInfo))
            {
                return tileInfo.Find(u, v);
            }
            else
            {
                return null;
            }
        }
    }
}