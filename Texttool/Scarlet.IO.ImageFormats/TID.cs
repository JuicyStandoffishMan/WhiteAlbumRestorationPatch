﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

using Scarlet.Drawing;
using Scarlet.IO;

namespace Scarlet.IO.ImageFormats
{
    // TODO: finish & verify me! Can TIDs contain multiple images? If so, how's it work?
    // TODO #2: check for more indexed TIDs & verify them, Nep-Sega has at least a few!

    // "E:\[SSD User Data]\Downloads\[[[nep-test]]]\GAME.cpk RB1 Vita" "E:\[SSD User Data]\Downloads\[[[nep-test]]]\GAME00000 RB1 PC"

    public enum TidFormatUnknownBit0 : byte
    {
        Unset = 0x00,
        Set = 0x01
    }

    public enum TidFormatChannelOrder : byte
    {
        Rgba = 0x00,
        Argb = 0x02
    }

    public enum TidFormatCompressionFlag : byte
    {
        NotCompressed = 0x00,
        Compressed = 0x04
    }

    public enum TidFormatUnknownBit3 : byte
    {
        Unset = 0x00,
        Set = 0x08
    }

    public enum TidFormatUnknownBit4 : byte
    {
        Unset = 0x00,
        Set = 0x10
    }

    public enum TidFormatUnknownBit5 : byte
    {
        Unset = 0x00,
        Set = 0x20
    }

    public enum TidFormatUnknownBit6 : byte
    {
        Unset = 0x00,
        Set = 0x40
    }

    public enum TidFormatUnknownBit7 : byte
    {
        Unset = 0x00,
        Set = 0x80
    }

    [MagicNumber("TID", 0x00)]
    public class TID : ImageFormat
    {
        public string MagicNumber { get; private set; }
        public byte PixelFormat { get; private set; }
        public uint FileSize { get; private set; }
        public uint ImageDataOffset { get; private set; } // mostly 0x80?
        public uint Unknown0x0C { get; private set; } // 1? Num images?
        public uint Unknown0x10 { get; private set; } // 1? Num filenames?
        public uint Unknown0x14 { get; private set; } // 0x20? Filename offset?
        public uint Unknown0x18 { get; private set; }
        public uint Unknown0x1C { get; private set; }
        public string FileName { get; private set; }
        public uint Unknown0x40 { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint BitsPerPixel { get; private set; } // 32 for non-indexed, 8 for indexed (Nep-Sega)?
        public ushort Unknown0x50 { get; private set; } // 1?
        public ushort Unknown0x52 { get; private set; } // 1?
        public uint PaletteDataSize { get; private set; }
        public uint ImageDataSize { get; private set; }
        public uint Unknown0x5C { get; private set; } // mostly 0x80; same as ImageDataOffset?
        public uint Unknown0x60 { get; private set; } // mostly 0, 0x04 w/ DXT5?
        public string CompressionFourCC { get; private set; }
        public uint Unknown0x68 { get; private set; } // mostly 0?
        public uint Unknown0x6C { get; private set; }
        public uint Unknown0x70 { get; private set; }
        public uint Unknown0x74 { get; private set; }
        public byte Unknown0x78 { get; private set; } // 0 or 1?
        public byte Unknown0x79 { get; private set; } // 0 or 1?
        public ushort Unknown0x7A { get; private set; } // mostly 0, 0x02 w/ DXT5?
        public ushort Unknown0x7C { get; private set; }
        public ushort Unknown0x7E { get; private set; }

        public byte[] PixelData { get; private set; }
        public byte[] PaletteData { get; private set; }

        ImageBinary imageBinary;

        protected override void OnOpen(EndianBinaryReader reader)
        {
            MagicNumber = Encoding.ASCII.GetString(reader.ReadBytes(3));
            PixelFormat = reader.ReadByte();
            FileSize = reader.ReadUInt32();
            ImageDataOffset = reader.ReadUInt32();
            Unknown0x0C = reader.ReadUInt32();
            Unknown0x10 = reader.ReadUInt32();
            Unknown0x14 = reader.ReadUInt32();
            Unknown0x18 = reader.ReadUInt32();
            Unknown0x1C = reader.ReadUInt32();
            FileName = Encoding.ASCII.GetString(reader.ReadBytes(0x20)).TrimEnd('\0');
            Unknown0x40 = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            BitsPerPixel = reader.ReadUInt32();
            Unknown0x50 = reader.ReadUInt16();
            Unknown0x52 = reader.ReadUInt16();
            PaletteDataSize = reader.ReadUInt32();
            ImageDataSize = reader.ReadUInt32();
            Unknown0x5C = reader.ReadUInt32();
            Unknown0x60 = reader.ReadUInt32();
            CompressionFourCC = Encoding.ASCII.GetString(reader.ReadBytes(4)).TrimEnd('\0');
            Unknown0x68 = reader.ReadUInt32();
            Unknown0x6C = reader.ReadUInt32();
            Unknown0x70 = reader.ReadUInt32();
            Unknown0x74 = reader.ReadUInt32();
            Unknown0x78 = reader.ReadByte();
            Unknown0x79 = reader.ReadByte();
            Unknown0x7A = reader.ReadUInt16();
            Unknown0x7C = reader.ReadUInt16();
            Unknown0x7E = reader.ReadUInt16();

            if (reader.BaseStream.Position != 0x80)
                throw new Exception("TID stream position mismatch");

            PaletteData = reader.ReadBytes((int)PaletteDataSize);

            reader.BaseStream.Seek(ImageDataOffset, SeekOrigin.Begin);
            PixelData = reader.ReadBytes((int)ImageDataSize);

            TidFormatChannelOrder pixelChannelOrder = ((TidFormatChannelOrder)PixelFormat & TidFormatChannelOrder.Argb);
            TidFormatCompressionFlag pixelCompression = ((TidFormatCompressionFlag)PixelFormat & TidFormatCompressionFlag.Compressed);

            bool pixelUnknownBit0 = (((TidFormatUnknownBit0)PixelFormat & TidFormatUnknownBit0.Set) == TidFormatUnknownBit0.Set);
            bool pixelUnknownBit3 = (((TidFormatUnknownBit3)PixelFormat & TidFormatUnknownBit3.Set) == TidFormatUnknownBit3.Set);
            bool pixelUnknownBit4 = (((TidFormatUnknownBit4)PixelFormat & TidFormatUnknownBit4.Set) == TidFormatUnknownBit4.Set);   // TODO: vertical flip according to some docs, verify
            bool pixelUnknownBit5 = (((TidFormatUnknownBit5)PixelFormat & TidFormatUnknownBit5.Set) == TidFormatUnknownBit5.Set);
            bool pixelUnknownBit6 = (((TidFormatUnknownBit6)PixelFormat & TidFormatUnknownBit6.Set) == TidFormatUnknownBit6.Set);
            bool pixelUnknownBit7 = (((TidFormatUnknownBit7)PixelFormat & TidFormatUnknownBit7.Set) == TidFormatUnknownBit7.Set);

            PixelDataFormat pixelFormat, paletteFormat = PixelDataFormat.Undefined;

            if (pixelCompression == TidFormatCompressionFlag.Compressed)
            {
                switch (CompressionFourCC)
                {
                    case "DXT1": pixelFormat = PixelDataFormat.FormatDXT1Rgba; break;
                    case "DXT3": pixelFormat = PixelDataFormat.FormatDXT3; break;
                    case "DXT5": pixelFormat = PixelDataFormat.FormatDXT5; break;
                    default: throw new Exception(string.Format("Unimplemented TID compression format '{0}'", CompressionFourCC));
                }
            }
            else if (pixelCompression == TidFormatCompressionFlag.NotCompressed)
            {
                if (PaletteDataSize != 0)
                {
                    if (pixelChannelOrder == TidFormatChannelOrder.Rgba)
                        paletteFormat = PixelDataFormat.FormatRgba8888;
                    else if (pixelChannelOrder == TidFormatChannelOrder.Argb)
                        paletteFormat = PixelDataFormat.FormatArgb8888;
                    else
                        throw new Exception("Invalid TID channel order; should not be reached?!");

                    if (BitsPerPixel == 8)
                        pixelFormat = PixelDataFormat.FormatIndexed8;
                    else
                        throw new Exception("Invalid or unsupported TID bits per pixel in indexed mode");
                }
                else
                {
                    if (BitsPerPixel == 32)
                    {
                        if (pixelChannelOrder == TidFormatChannelOrder.Rgba)
                            pixelFormat = PixelDataFormat.FormatRgba8888;
                        else if (pixelChannelOrder == TidFormatChannelOrder.Argb)
                            pixelFormat = PixelDataFormat.FormatArgb8888;
                        else
                            throw new Exception("Invalid TID channel order; should not be reached?!");
                    }
                    else
                        throw new Exception("Invalid or unsupported TID bits per pixel in non-indexed mode");
                }
            }
            else
                throw new Exception("Invalid TID compression flag; should not be reached?!");

            // TODO: verify if [Compressed == Swizzled] is correct, or if swizzling depends on other factors

            if (pixelCompression == TidFormatCompressionFlag.Compressed)
                pixelFormat |= PixelDataFormat.PixelOrderingSwizzledVita;

            imageBinary = new ImageBinary();
            imageBinary.Width = (int)Width;
            imageBinary.Height = (int)Height;
            imageBinary.InputPixelFormat = pixelFormat;
            imageBinary.InputPaletteFormat = paletteFormat;
            imageBinary.InputEndianness = Endian.BigEndian;

            imageBinary.AddInputPixels(PixelData);
            imageBinary.AddInputPalette(PaletteData);
        }

        public override int GetImageCount()
        {
            return 1;
        }

        public override int GetPaletteCount()
        {
            return 0;
        }

        protected override Bitmap OnGetBitmap(int imageIndex, int paletteIndex)
        {
            return imageBinary.GetBitmap(imageIndex, paletteIndex);
        }
    }
}
