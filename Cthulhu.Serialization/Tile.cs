using System.Collections;
using Cthulhu.Serialization.Extensions;

namespace Cthulhu.Serialization
{
    public readonly struct Tile
    {
        private const int ActiveMask = 1 << 0;
        private const int ColorPresentMask = 1 << 1;
        private const int DummyMask = 1 << 2;
        private const int WallPresentMask = 1 << 3;
        private const int WallColorPresentMask = 1 << 4;
        private const int LiquidPresentMask = 1 << 5;
        private const int LiquidLavaMask = 1 << 6;
        private const int LiquidHoneyMask = 1 << 7;
        private const int RedWireMask = 1 << 8;
        private const int GreenWireMask = 1 << 9;
        private const int BlueWireMask = 1 << 10;
        private const int YellowWireMask = 1 << 11;
        private const int HalfTileMask = 1 << 12;
        private const int ActuatorPresentMask = 1 << 13;
        private const int InactiveMask = 1 << 14;

        private readonly int _flags;

        public readonly short TextureU { get; }
        public readonly short TextureV { get; }
        public readonly ushort TileType { get; }
        public readonly byte WallType { get; }
        public readonly byte WallColor { get; }
        public readonly byte ColorValue { get; }
        public readonly byte LiquidAmount { get; }
        public readonly bool IsActive => _flags.MaskAll(ActiveMask);
        public readonly bool HasColor => _flags.MaskAll(ColorPresentMask);
        public readonly bool HasWall => _flags.MaskAll(WallPresentMask);
        public readonly bool HasWallColor => _flags.MaskAll(WallColorPresentMask);
        public readonly bool HasLiquid => _flags.MaskAll(LiquidPresentMask);
        public readonly bool HasLava => _flags.MaskAll(LiquidLavaMask);
        public readonly bool HasHoney => _flags.MaskAll(LiquidHoneyMask);
        public readonly bool HasRedWire => _flags.MaskAll(RedWireMask);
        public readonly bool HasGreenWire => _flags.MaskAll(GreenWireMask);
        public readonly bool HasBlueWire => _flags.MaskAll(BlueWireMask);
        public readonly bool HasYellowWire => _flags.MaskAll(YellowWireMask);
        public readonly bool IsHalfTile => _flags.MaskAll(HalfTileMask);
        public readonly bool HasActuator => _flags.MaskAll(ActuatorPresentMask);

        internal Tile(MemoryReader reader, BitArray importance, out int runLength)
        {
            int flags = 0;
            byte b = 0;
            byte b2 = 0;
            byte b3 = reader.ReadByte();
            byte b4 = 0;

            TextureU = -1;
            TextureV = -1;
            TileType = 0;
            WallType = 0;
            WallColor = 0;
            ColorValue = 0;
            LiquidAmount = 0;

            if ((b3 & 1) == 1)
            {
                b2 = reader.ReadByte();

                if ((b2 & 1) == 1)
                {
                    b = reader.ReadByte();
                }
            }

            if ((b3 & 2) == 2)
            {
                int num2;
                flags |= ActiveMask;

                if ((b3 & 32) == 32)
                {
                    b4 = reader.ReadByte();
                    num2 = reader.ReadByte();
                    num2 = (num2 << 8 | b4);
                }
                else
                {
                    num2 = reader.ReadByte();
                }

                TileType = (ushort)num2;

                if (importance[num2])
                {
                    TextureU = reader.ReadInt16();
                    TextureV = reader.ReadInt16();

                    if (TileType == 144)
                        TextureV = 0;
                }

                if ((b & 8) == 8)
                {
                    ColorValue = reader.ReadByte();
                }
            }

            if ((b3 & 4) == 4)
            {
                WallType = reader.ReadByte();
                flags |= WallPresentMask;

                if ((b & 16) == 16)
                {
                    WallColor = reader.ReadByte();
                    flags |= WallColorPresentMask;
                }
            }

            b4 = (byte)((b3 & 24) >> 3);

            if (b4 != 0)
            {
                flags |= LiquidPresentMask;
                LiquidAmount = reader.ReadByte();

                if (b4 > 1)
                {
                    if (b4 == 2)
                        flags |= LiquidLavaMask;
                    else
                        flags |= LiquidHoneyMask;
                }
            }

            if (b2 > 1)
            {
                if ((b2 & 2) == 2)
                    flags |= RedWireMask;
                
                if ((b2 & 4) == 4)
                    flags |= GreenWireMask;
                
                if ((b2 & 8) == 8)
                    flags |= BlueWireMask;
            }

            if (b > 0)
            {
                if ((b & 2) == 2)
                    flags |= ActuatorPresentMask;
                
                if ((b & 4) == 4)
                    flags &= ~ActiveMask;
                
                if ((b & 32) == 32)
                    flags |= YellowWireMask;
                
                if ((b & 64) == 64)
                {
                    b4 = reader.ReadByte();
                    WallType = (byte)(b4 << 8 | WallType);
                }
            }

            _flags = flags;
            
            b4 = (byte)((b3 & 192) >> 6);

            if (b4 == 0)
            {
                runLength = 0;
            }
            else
            {
                if (b4 == 1)
                    runLength = reader.ReadByte();
                else
                    runLength = reader.ReadInt16();
            }
        }

        public override string ToString() => TileType.ToString();
    }
}
