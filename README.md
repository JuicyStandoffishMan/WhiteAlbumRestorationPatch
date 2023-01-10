# White Album 1 Translation Project

This is a collection of tools to aid in extracting and modifying [White Album 1 (2012 PC version)](https://vndb.org/r20439) resources to create an accompanying [English translation](#translation).

## Building

Visual Studio 2022 with .NET Core 6.

## Usage

### Script Extraction
Drag 'Data/Game/Script.sdat' into the window. Click "Export" to export an excel spreadsheet for translating, and "Import" to re-import it. Click save to update Script.sdat.

### Modifying and Importing Scripts
The tool will apply the script located in the 'Data/Game/excel' folder. Note that column **F "Edited"** is the English text that gets used when importing. The columns on the right are for aiding in translating and editing. When finished editing, click "Import" for the script and then "Save" to update the Scripts.sdat file and then reload the game.

To use the translated scripts that are located in the [tl folder of this repo](/tl/), copy its files into your 'Data/Game/excel/trimmed' folder and click "Merge Trimmed" for each script you want to merge. This will merge the English columns F-J into your full scripts in the 'Data/Game/excel' folder if they exist or with a new script. Then click "Save".

**Batch importing and saving is not yet supported**, so you have to do the above process manually for each script.

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

Alternatively, there's a [Lunar IPS patch available for it here](/resources/Font%20patch.zip). Just apply it to 'Data/Font.pck'.

### Batch Import/Export/Merging
Currently a TODO, which will happen when Form1.cs is made to be less terrible.

## Translation
An ongoing effort is being made to translate the game. Translated scripts can be found in the [tl folder](/tl/). Batch importing is not supported yet.

### Progress
1/1107

### [Video Preview](https://www.youtube.com/watch?v=9sM0kkkhB_g)

### Translation Commit Guidelines
Only trimmed scripts with the original Japanese text taken out are allowed in this repo. Fortunately, this tool provides a feature to do this, as well as merge trimmed scripts with the full ones. See [Importing Scripts from the Repo](#importing-scripts-from-the-repo) for merging scripts from this repo with the source versions for editing.

#### Trimming Scripts for Commits
Click "Import" to load the full script into the tool and then click "Export Trimmed". This will write the trimmed version into the trimmed folder (eg. '/Data/Game/excel/trimmed').

Additionally, when pushing new spreadsheet files, be sure that:
- None of the Japanese text is included with the committed spreadsheet.
- The copied .bin file, which is a blob of the original file in the blob folder, is not included.
- 'Block ID' (column B) is unchanged and either the JP text (column E) is blank or exactly matches the original for the importing to work.
- Personal information is not attached to the file. [Use this guide for removing it in Excel](https://support.microsoft.com/en-us/office/remove-hidden-data-and-personal-information-by-inspecting-documents-presentations-or-workbooks-356b7b5d-77af-44fe-a07f-9aa4d085966f). Exported trimmed scripts do not contain an author, but they may contain file path information.

#### Text Processing Notes
- Quotation marks are automatically added to dialogue (lines with an author) if they're missing.
- Word wrapping on spaces and hyphens and textbox breaking are handled automatically, since the game's backlog crashes if lines are too long.
- Ellipses ... are automatically replaced by their single-character counterpart ….

## Licenses
- [Scarlet](https://github.com/xdanieldzd/Scarlet/blob/master/LICENSE.md)
- [DALTools](https://github.com/thesupersonic16/DALTools)

## Disclaimer
This repo and its contributors are not affiliated with Leaf/Aquaplus. None of the original game's content is in this repo for copyright reasons. Also the code sucks.
