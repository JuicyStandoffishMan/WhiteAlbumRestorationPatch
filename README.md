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

There's a [Lunar IPS patch available for it here](https://github.com/JuicyStandoffishMan/WhiteAlbumTranslationTools/blob/master/resources/Font%20patch.zip) that gets applied to Font.pck.

## Translation
An ongoing effort is being made to translate the game. Translated scripts can be found in the [tl folder](https://github.com/JuicyStandoffishMan/WhiteAlbumTranslationTools/tree/master/tl). Batch importing is not support yet.

### Progress
1/1107

### Commit Guidelines
When pushing new spreadsheet files, be sure that:
- None of the Japanese text is included with the committed spreadsheet.
- The copied .bin file, which is a blob of the original file, is not included.
- Personal information is not attached to the file. [Use this guide for removing it in Excel](https://support.microsoft.com/en-us/office/remove-hidden-data-and-personal-information-by-inspecting-documents-presentations-or-workbooks-356b7b5d-77af-44fe-a07f-9aa4d085966f).

Make sure 'Block ID' (column B) is unchanged and either the JP text (column E) is blank or exactly matches the original for the importing to work.

## Licenses
- [Scarlet](https://github.com/xdanieldzd/Scarlet/blob/master/LICENSE.md)
- [DALTools](https://github.com/thesupersonic16/DALTools)

## Disclaimer
This repo and its contributors are not affiliated with Leaf/Aquaplus. None of the original game's content is in this repo for copyright reasons. Also the code sucks.
