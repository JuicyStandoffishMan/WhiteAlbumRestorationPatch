# White Album 1 Translation Tools

This is a collection of tools to aid in extracting and modifying White Album 1 (2012 PC version) resources to create an English translation.

## Building

Visual Studio 2022 with .NET Core 6.

## Usage

### Script Extraction
Drag 'Data/Game/Script.sdat' into the window. Click "Export" to export an excel spreadsheet for translating, and "Import" to re-import it. Click save to update Script.sdat.

### Executable Patching
Drag 'WHITE ALBUM.exe' into the window to apply the following data changes:
```
public int CharsPerLine = 0x3D;
...
-0x17DC1 = (byte)CharsPerLine; // Visual chars per line
-0x17B9C = (byte)CharsPerLine; // Actual chars per line
-0x176C6 = 0x0E; // X spacing
-0x176D7 = 0x24; // Y spacing
```

### Font Extraction
This was a manual process that extracted 'MAINTEXT.fnt' from 'Font.pck' and stripped the header, producing a raw .tex file that was then converted into MAINTEXT.png. It was then scaled down to 75%, expanded vertically to be 1512x1134, and had English characters with decoration manually added. The new font characters are available in resources/MAINTEXT.png, but the original characters are blacked out due to copyright. Dragging this file will repack it into 'Font.pck', assuming it's in the same folder where the file was dragged from.

## Licenses
- [Scarlet](https://github.com/xdanieldzd/Scarlet/blob/master/LICENSE.md)
- [DALTools](https://github.com/thesupersonic16/DALTools)

## Disclaimer
This repo and its contributors are not affiliated with Leaf/Aquaplus. None of the original game's content is in this repo for copyright reasons. Also the code sucks.
