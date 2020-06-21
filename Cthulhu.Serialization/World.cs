using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using Cthulhu.Serialization.Extensions;

namespace Cthulhu.Serialization
{
    public class World
    {
        public static string GetFolder()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var worldFolder = Path.Combine(documentsFolder, "My Games", "Terraria", "Worlds");
                return worldFolder;
            }
            else
            {
                return null;
            }
        }

        public static World FromWorldData(ReadOnlyMemory<byte> bytes)
        {
            var reader = new MemoryReader(bytes);
            var version = reader.ReadInt32();
            var world = new World(version);

            if (version <= 87)
            {
                throw new NotSupportedException("Unable to load world data version 87 and below.");
            }
            else
            {
                world.ReadData(reader);
            }

            return world;
        }

        private ImmutableArray<Tile> _tiles = default;

        [JsonIgnore] public ImmutableArray<Chest> Chests { get; private set; } = ImmutableArray<Chest>.Empty;
        [JsonIgnore] public ImmutableArray<Npc> Npcs { get; private set; } = ImmutableArray<Npc>.Empty;
        public ImmutableArray<Sign> Signs { get; private set; } = ImmutableArray<Sign>.Empty;
        public ImmutableArray<string> AnglersWhoFinishedToday { get; private set; } = ImmutableArray<string>.Empty;

        public int Version { get; }
        public uint Revision { get; private set; }
        public bool IsFavorite { get; private set; }
        public string Name { get; private set; }
        public string Seed { get; private set; }
        public ulong WorldGeneratorVersion { get; private set; }
        public Guid UniqueId { get; private set; }
        public int Id { get; private set; }
        public Rectangle Bounds { get; private set; }
        public int WorldHeightInTiles { get; private set; }
        public int WorldWidthInTiles { get; private set; }
        public int GameMode { get; private set; }
        public bool DrunkWorld { get; private set; }
        public bool GoodWorld { get; private set; }
        public long CreationTime { get; private set; }
        public byte MoonType { get; private set; }
        public ImmutableArray<int> TreeTypeXCoordinates { get; private set; } = ImmutableArray<int>.Empty;
        public ImmutableArray<int> TreeStyles { get; private set; } = ImmutableArray<int>.Empty;
        public ImmutableArray<int> CaveBackXCoordinates { get; private set; } = ImmutableArray<int>.Empty;
        public ImmutableArray<int> CaveBackStyles { get; private set; } = ImmutableArray<int>.Empty;
        public int IceBackStyle { get; private set; }
        public int JungleBackStyle { get; private set; }
        public int HellBackStyle { get; private set; }
        public int SpawnX { get; private set; }
        public int SpawnY { get; private set; }
        public double WorldSurfaceY { get; private set; }
        public double RockLayerY { get; private set; }
        public int HellLayerY { get; private set; }
        public double GameTime { get; private set; }
        public bool IsDay { get; private set; }
        public int MoonPhase { get; private set; }
        public bool BloodMoon { get; private set; }
        public bool Eclipse { get; private set; }
        public int DungeonX { get; private set; }
        public int DungeonY { get; private set; }
        public bool CrimsonWorld { get; private set; }
        public bool KilledEyeOfCthulhu { get; private set; }
        public bool KilledEaterOfWorlds { get; private set; }
        public bool KilledSkeletron { get; private set; }
        public bool KilledQueenBee { get; private set; }
        public bool KilledTheDestroyer { get; private set; }
        public bool KilledTheTwins { get; private set; }
        public bool KilledSkeletronPrime { get; private set; }
        public bool KilledAnyHardmodeBoss { get; private set; }
        public bool KilledPlantera { get; private set; }
        public bool KilledGolem { get; private set; }
        public bool KilledSlimeKing { get; private set; }
        public bool SavedGoblinTinkerer { get; private set; }
        public bool SavedWizard { get; private set; }
        public bool SavedMechanic { get; private set; }
        public bool DefeatedGoblinInvasion { get; private set; }
        public bool KilledClown { get; private set; }
        public bool DefeatedFrostLegion { get; private set; }
        public bool DefeatedPirates { get; private set; }
        public bool BrokeShadowOrb { get; private set; }
        public bool MeteorLanded { get; private set; }
        public byte ShadowOrbsBrokenModThree { get; private set; }
        public int AltarsSmashed { get; private set; }
        public bool Hardmode { get; private set; }
        public int GoblinInvasionDelay { get; private set; }
        public int GoblinInvasionSize { get; private set; }
        public int GoblinInvasionType { get; private set; }
        public double GoblinInvasionX { get; private set; }
        public double SlimeRainTime { get; private set; }
        public byte SundialCooldown { get; private set; }
        public bool IsRaining { get; private set; }
        public int RainTime { get; private set; }
        public float MaxRain { get; private set; }
        public int Tier1OreId { get; private set; }
        public int Tier2OreId { get; private set; }
        public int Tier3OreId { get; private set; }
        public byte TreeStyle { get; private set; }
        public byte CorruptionStyle { get; private set; }
        public byte JungleStyle { get; private set; }
        public byte SnowStyle { get; private set; }
        public byte HallowStyle { get; private set; }
        public byte CrimsonStyle { get; private set; }
        public byte DesertStyle { get; private set; }
        public byte OceanStyle { get; private set; }
        public int CloudBackground { get; private set; }
        public short CloudCount { get; private set; }
        public float WindSpeed { get; private set; }
        public bool SavedAngler { get; private set; }
        public int AnglerQuest { get; private set; }
        public bool SavedStylist { get; private set; }
        public bool SavedTaxCollector { get; private set; }
        public bool SavedGolfer { get; private set; }
        public int InvasionSizeStart { get; private set; }
        public int TempCultistDelay { get; private set; }
        public bool FastForwardTime { get; private set; }
        public bool KilledFishron { get; private set; }
        public bool KilledMartians { get; private set; }
        public bool KilledAncientCultist { get; private set; }
        public bool KilledMoonLord { get; private set; }
        public bool KilledHalloweenKing { get; private set; }
        public bool KilledHalloweenTree { get; private set; }
        public bool KilledChristmasIceQueen { get; private set; }
        public bool KilledChristmasSantank { get; private set; }
        public bool KilledChristmasTree { get; private set; }
        public bool KilledTowerSolar { get; private set; }
        public bool KilledTowerVortex { get; private set; }
        public bool KilledTowerNebula { get; private set; }
        public bool KilledTowerStardust { get; private set; }
        public bool TowerActiveSolar { get; private set; }
        public bool TowerActiveVortex { get; private set; }
        public bool TowerActiveNebula { get; private set; }
        public bool TowerActiveStardust { get; private set; }
        public bool LunarApocalypseActive { get; private set; }
        public bool PartyManual { get; private set; }
        public bool PartyGenuine { get; private set; }
        public int PartyCooldown { get; private set; }
        public bool SandstormHappening { get; private set; }
        public int SandstormTimeLeft { get; private set; }
        public float SandstormSeverity { get; private set; }
        public float SandstormIntendedSeverity { get; private set; }
        public bool SavedBartender { get; private set; }
        public bool KilledInvasionTier1 { get; private set; }
        public bool KilledInvasionTier2 { get; private set; }
        public bool KilledInvasionTier3 { get; private set; }
        public bool CombatBookWasUsed { get; private set; }
        public int SavedOreTiersCopper { get; private set; }
        public int SavedOreTiersIron { get; private set; }
        public int SavedOreTiersSilver { get; private set; }
        public int SavedOreTiersGold { get; private set; }
        public bool BoughtCat { get; private set; }
        public bool BoughtDog { get; private set; }
        public bool BoughtBunny { get; private set; }
        public bool KilledEmpressOfLight { get; private set; }
        public bool KilledQueenSlime { get; private set; }

        public IEnumerable<KeyValuePair<Point32, Tile>> Tiles
        {
            get
            {
                int x = 0;
                int y = 0;

                foreach (var tile in _tiles)
                {
                    var point = new Point32(x, y);
                    var pair = KeyValuePair.Create(point, tile);
                    yield return pair;

                    if (WorldHeightInTiles <= ++y)
                    {
                        ++x;
                        y = 0;
                    }
                }
            }
        }

        public Point32 GetGpsPosition(Point32 position)
        {
            var x = (position.X - WorldWidthInTiles / 2) * 2;
            var y = (position.Y - (int)WorldSurfaceY) * 2;
            return new Point32(x, y);
        }

        private World(int version)
        {
            Version = version;
        }

        private int IndexOf(int x, int y) => x * WorldHeightInTiles + y;
        public Tile GetTile(int x, int y) => _tiles[IndexOf(x, y)];

        private void ReadData(MemoryReader reader)
        {
            if (135 <= Version)
            {
                var header = reader.ReadUInt64() & 0x00ffffffffffffff;
                
                if (header != 0x6369676f6c6572)
                    throw new WorldParseException("Missing 'relogic' header");
                
                Revision = reader.ReadUInt32();
                
                var block = reader.ReadUInt64();
                IsFavorite = block.MaskAll(1);
            }

            var positionsLength = reader.ReadInt16();
            var positions = new int[positionsLength];
            
            for (int i = 0; i < positions.Length; ++i)
                positions[i] = reader.ReadInt32();
            
            var importanceLength = reader.ReadInt16();
            var importance = new BitArray(importanceLength);

            // TODO: Rework this?
            byte b = 0;
            byte b2 = 128;

            for (int i = 0; i < importance.Length; ++i)
            {
                if (b2 == 128)
                {
                    b = reader.ReadByte();
                    b2 = 1;
                }
                else
                {
                    b2 <<= 1;
                }

                if ((b & b2) == b2)
                    importance[i] = true;
            }

            void ThrowIfWrongPosition(int index)
            {
                var position = positions[index];
                if (reader.Position != position)
                    throw new WorldParseException($"Wrong file position. Expected: {position}. Actual: {reader.Position}.");
            }

            ThrowIfWrongPosition(0);
            ReadHeader(reader);
            ThrowIfWrongPosition(1);
            ReadTiles(reader, importance);
            ThrowIfWrongPosition(2);
            ReadChests(reader);
            ThrowIfWrongPosition(3);
            ReadSigns(reader);
            ThrowIfWrongPosition(4);
            ReadNpcs(reader);
        }

        private void ReadHeader(MemoryReader reader)
        {
            Name = reader.ReadBString();

            if (179 <= Version)
                Seed = reader.ReadBString();
            
            if (179 <= Version)
                WorldGeneratorVersion = reader.ReadUInt64();
            
            if (181 <= Version)
                UniqueId = reader.ReadGuid();
            
            Id = reader.ReadInt32();
            Bounds = new Rectangle(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());
            WorldHeightInTiles = reader.ReadInt32();
            WorldWidthInTiles = reader.ReadInt32();

            if (209 <= Version)
                GameMode = reader.ReadInt32();
            
            if (222 <= Version)
                DrunkWorld = reader.ReadBoolean();
            
            if (227 <= Version)
                GoodWorld = reader.ReadBoolean();
            
            if (141 <= Version)
                CreationTime = reader.ReadInt64();
            
            if (63 <= Version)
                MoonType = reader.ReadByte();
            
            if (44 <= Version)
            {
                TreeTypeXCoordinates = ImmutableArray.Create<int>(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32());
                
                TreeStyles = ImmutableArray.Create<int>(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32());
            }

            if (60 <= Version)
            {
                CaveBackXCoordinates = ImmutableArray.Create<int>(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32());
                
                CaveBackStyles = ImmutableArray.Create<int>(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32());

                IceBackStyle = reader.ReadInt32();
            }

            if (61 > Version)
            {
            }
            else
            {
                JungleBackStyle = reader.ReadInt32();
                HellBackStyle = reader.ReadInt32();
            }

            SpawnX = reader.ReadInt32();
            SpawnY = reader.ReadInt32();
            WorldSurfaceY = reader.ReadDouble();
            RockLayerY = reader.ReadDouble();
            GameTime = reader.ReadDouble();
            IsDay = reader.ReadBoolean();
            MoonPhase = reader.ReadInt32();
            BloodMoon = reader.ReadBoolean();

            if (70 <= Version)
                Eclipse = reader.ReadBoolean();
            
            DungeonX = reader.ReadInt32();
            DungeonY = reader.ReadInt32();

            if (56 <= Version)
                CrimsonWorld = reader.ReadBoolean();
            
            KilledEyeOfCthulhu = reader.ReadBoolean();
            KilledEaterOfWorlds = reader.ReadBoolean();
            KilledSkeletron = reader.ReadBoolean();
            
            if (66 <= Version)
                KilledQueenBee = reader.ReadBoolean();
            
            if (44 <= Version)
            {
                KilledTheDestroyer = reader.ReadBoolean();
                KilledTheTwins = reader.ReadBoolean();
                KilledSkeletronPrime = reader.ReadBoolean();
            }

            if (64 <= Version)
            {
                KilledAnyHardmodeBoss = reader.ReadBoolean();
                KilledPlantera = reader.ReadBoolean();
                KilledGolem = reader.ReadBoolean();
            }

            if (118 <= Version)
                KilledSlimeKing = reader.ReadBoolean();
            
            if (29 <= Version)
            {
                SavedGoblinTinkerer = reader.ReadBoolean();
                SavedWizard = reader.ReadBoolean();

                if (34 <= Version)
                    SavedMechanic = reader.ReadBoolean();
                
                DefeatedGoblinInvasion = reader.ReadBoolean();
            }

            if (32 <= Version)
                KilledClown = reader.ReadBoolean();
            
            if (37 <= Version)
                DefeatedFrostLegion = reader.ReadBoolean();
            
            if (56 <= Version)
                DefeatedPirates = reader.ReadBoolean();
            
            BrokeShadowOrb = reader.ReadBoolean();
            MeteorLanded = reader.ReadBoolean();
            ShadowOrbsBrokenModThree = reader.ReadByte();

            if (23 <= Version)
            {
                AltarsSmashed = reader.ReadInt32();
                Hardmode = reader.ReadBoolean();
            }

            GoblinInvasionDelay = reader.ReadInt32();
            GoblinInvasionSize = reader.ReadInt32();
            GoblinInvasionType = reader.ReadInt32();
            GoblinInvasionX = reader.ReadDouble();

            if (118 <= Version)
                SlimeRainTime = reader.ReadDouble();
            
            if (113 <= Version)
                SundialCooldown = reader.ReadByte();
            
            if (53 <= Version)
            {
                IsRaining = reader.ReadBoolean();
                RainTime = reader.ReadInt32();
                MaxRain = reader.ReadSingle();
            }

            if (54 <= Version)
            {
                Tier1OreId = reader.ReadInt32();
                Tier2OreId = reader.ReadInt32();
                Tier3OreId = reader.ReadInt32();
            }

            if (55 <= Version)
            {
                TreeStyle = reader.ReadByte();
                CorruptionStyle = reader.ReadByte();
                JungleStyle = reader.ReadByte();
            }

            if (60 <= Version)
            {
                SnowStyle = reader.ReadByte();
                HallowStyle = reader.ReadByte();
                CrimsonStyle = reader.ReadByte();
                DesertStyle = reader.ReadByte();
                OceanStyle = reader.ReadByte();

                CloudBackground = reader.ReadInt32();
            }

            if (62 <= Version)
            {
                CloudCount = reader.ReadInt16();
                WindSpeed = reader.ReadSingle();
            }

            if (95 <= Version)
            {
                var anglerCount = reader.ReadInt32();

                if (0 < anglerCount)
                {
                    var builder = ImmutableArray.CreateBuilder<string>(anglerCount);

                    for (int i = 0; i < anglerCount; ++i)
                    {
                        var angler = reader.ReadBString();
                        builder.Add(angler);
                    }

                    AnglersWhoFinishedToday = builder.MoveToImmutable();
                }
            }

            if (99 <= Version)
                SavedAngler = reader.ReadBoolean();
            
            if (101 <= Version)
                AnglerQuest = reader.ReadInt32();
            
            if (104 <= Version)
                SavedStylist = reader.ReadBoolean();
            
            if (129 <= Version)
                SavedTaxCollector = reader.ReadBoolean();
            
            if (201 <= Version)
                SavedGolfer = reader.ReadBoolean();
            
            if (107 <= Version)
                InvasionSizeStart = reader.ReadInt32();
            
            if (108 <= Version)
                TempCultistDelay = reader.ReadInt32();
            
            if (109 <= Version)
            {
                int n = reader.ReadInt16();
                for (int i = 0; i < n; ++i)
                {
                    // TODO: TerraMap is incomplete. It reads and ignores ints.
                    reader.Position += 4;
                }
            }

            if (128 <= Version)
                FastForwardTime = reader.ReadBoolean();
            
            if (131 <= Version)
            {
                KilledFishron = reader.ReadBoolean();
                KilledMartians = reader.ReadBoolean();
                KilledAncientCultist = reader.ReadBoolean();
                KilledMoonLord = reader.ReadBoolean();
                KilledHalloweenKing = reader.ReadBoolean();
                KilledHalloweenTree = reader.ReadBoolean();
                KilledChristmasIceQueen = reader.ReadBoolean();
                KilledChristmasSantank = reader.ReadBoolean();
                KilledChristmasTree = reader.ReadBoolean();
            }

            if (140 <= Version)
            {
                KilledTowerSolar = reader.ReadBoolean();
                KilledTowerVortex = reader.ReadBoolean();
                KilledTowerNebula = reader.ReadBoolean();
                KilledTowerStardust = reader.ReadBoolean();
                TowerActiveSolar = reader.ReadBoolean();
                TowerActiveVortex = reader.ReadBoolean();
                TowerActiveNebula = reader.ReadBoolean();
                TowerActiveStardust = reader.ReadBoolean();
                LunarApocalypseActive = reader.ReadBoolean();
            }

            if (170 <= Version)
            {
                PartyManual = reader.ReadBoolean();
                PartyGenuine = reader.ReadBoolean();
                PartyCooldown = reader.ReadInt32();

                int n = reader.ReadInt32();

                for (int i = 0; i < n; ++i)
                {
                    // TODO: Incomplete.
                    reader.Position += 4;
                }
            }

            if (174 <= Version)
            {
                SandstormHappening = reader.ReadBoolean();
                SandstormTimeLeft = reader.ReadInt32();
                SandstormSeverity = reader.ReadSingle();
                SandstormIntendedSeverity = reader.ReadSingle();
            }

            if (178 <= Version)
            {
                SavedBartender = reader.ReadBoolean();
                KilledInvasionTier1 = reader.ReadBoolean();
                KilledInvasionTier2 = reader.ReadBoolean();
                KilledInvasionTier3 = reader.ReadBoolean();
            }

            // Journey's End stuff

            // World BG stuff
            if (195 <= Version)
            {
                reader.ReadByte();
            }

            if (215 <= Version)
            {
                reader.ReadByte();
            }

            // Tree BG stuff
            if (196 <= Version)
            {
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }

            if (204 <= Version)
                CombatBookWasUsed = reader.ReadBoolean();
            
            // tempLanternNight stuff
            if (207 <= Version)
            {
                reader.ReadInt32();
                reader.ReadBoolean();
                reader.ReadBoolean();
                reader.ReadBoolean();
            }

            // tree tops info
            if (178 <= Version)
            {
                int n0 = reader.ReadInt32();
                int n1 = 0;

                while (n1 < n0 && n1 < 13)
                {
                    reader.ReadInt32();
                    n1++;
                }
            }

            if (212 <= Version)
            {
                reader.ReadBoolean(); // forceHalloweenForToday
                reader.ReadBoolean(); // forceChristmasForToday
            }

            if (216 <= Version)
            {
                SavedOreTiersCopper = reader.ReadInt32();
                SavedOreTiersIron = reader.ReadInt32();
                SavedOreTiersSilver = reader.ReadInt32();
                SavedOreTiersGold = reader.ReadInt32();
            }

            if (217 <= Version)
            {
                BoughtCat = reader.ReadBoolean();
                BoughtDog = reader.ReadBoolean();
                BoughtBunny = reader.ReadBoolean();
            }

            if (223 <= Version)
            {
                KilledEmpressOfLight = reader.ReadBoolean();
                KilledQueenSlime = reader.ReadBoolean();
            }

            var wsy = (int)WorldSurfaceY;
            HellLayerY = (WorldHeightInTiles - 230 - wsy) / 6;
            HellLayerY = HellLayerY * 6 + wsy - 5;
        }
    
        private void ReadTiles(MemoryReader reader, BitArray importance)
        {
            int tilesCounted = 0;
            int tileCount = WorldWidthInTiles * WorldHeightInTiles;
            var builder = ImmutableArray.CreateBuilder<Tile>(tileCount);

            for (int i = 0; i < tileCount; ++i)
            {
                var tile = new Tile(reader, importance, out var runLength);
                builder.Add(tile);
                ++tilesCounted;

                for (int j = 0; j < runLength; ++j)
                {
                    builder.Add(tile);
                    ++tilesCounted;
                    ++i;
                }
            }

            _tiles = builder.MoveToImmutable();
        }
    
        private void ReadChests(MemoryReader reader)
        {
            int chestCount = reader.ReadInt16(); // "num"
            int num2 = reader.ReadInt16();
            int num3;
            int num4;

            const int MaxItems = 40;

            if (num2 < MaxItems)
            {
                num3 = num2;
                num4 = 0;
            }
            else
            {
                num3 = MaxItems;
                num4 = num2 - MaxItems;
            }

            var chestBuilder = ImmutableArray.CreateBuilder<Chest>(chestCount);
            for (int i = 0; i < chestCount; ++i)
            {
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var name = reader.ReadBString();

                var itemsBuilder = ImmutableArray.CreateBuilder<Item>(num3);
                for (int j = 0; j < num3; ++j)
                {
                    var itemCount = reader.ReadInt16(); // "num5"

                    if (itemCount > 0)
                    {
                        var itemId = reader.ReadInt32();
                        var prefixId = reader.ReadByte();

                        var item = new Item(itemId, itemCount, prefixId);
                        itemsBuilder.Add(item);
                    }
                }

                for (int j = 0; j < num4; ++j)
                {
                    var num5 = reader.ReadInt16();

                    if (num5 > 0)
                    {
                        reader.ReadInt32();
                        reader.ReadByte();
                    }
                }

                var chest = new Chest(x, y, name, itemsBuilder.ToImmutable());
                chestBuilder.Add(chest);
            }

            Chests = chestBuilder.MoveToImmutable();
        }

        private void ReadSigns(MemoryReader reader)
        {
            int signCount = reader.ReadInt16();
            var signsBuilder = ImmutableArray.CreateBuilder<Sign>(signCount);

            for (int i = 0; i < signCount; ++i)
            {
                var text = reader.ReadBString();
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var sign = new Sign(x, y, text);
                var tile = GetTile(x, y);

                signsBuilder.Add(sign);
                //if (tile.IsActive && (tile.TileType == 55 || tile.TileType == 85))
            }

            Signs = signsBuilder.MoveToImmutable();
        }

        private void ReadNpcs(MemoryReader reader)
        {
            var npcsBuilder = ImmutableArray.CreateBuilder<Npc>();
            
            while (reader.ReadBoolean())
            {
                var npc = new Npc(reader, Version);
                npcsBuilder.Add(npc);
            }

            if (140 <= Version)
            {
                // These are all just thrown away, I guess...

                while (reader.ReadBoolean())
                {
                    if (190 <= Version)
                    {
                        reader.ReadInt32();
                    }
                    else
                    {
                        reader.ReadBString();
                    }

                    reader.ReadSingle();
                    reader.ReadSingle();
                }
            }

            Npcs = npcsBuilder.ToImmutable();
        }
    }
}
