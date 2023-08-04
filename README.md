# White Album Restoration Project

![img](yuki.png)

This is a collection of tools to aid in extracting and modifying [White Album 1 (2023 Steam PC version)](https://vndb.org/r108248) resources to delocalize content. Up-to-date modified scripts **with the original texts taken out** are available in the [stripped folder](stripped/).

Please support the developers and purchase the game! This project, like the original, was created out of a love for it. We're super fortunate to have gotten an official re-release, and this projects aims to just make tiny adjustments to the script that are ever so slightly more faithful to the original text out of sheer personal preference.

## Building
You will need to have these installed:
- Visual Studio 2022 with .NET Core 6.
- Python

## Usage

### Script Extraction
There are a handful of mixed application and now Python scripts for extracting, modifying, and importing the scripts.

1) Install the Steam version in English. You can change it by going into the game's properties from the library and changing the language.
2) Navigate to the install directory (likely `C:\Program Files (x86)\Steam\steamapps\common\WHITE ALBUM\`)
3) Drag the file `Data\Game\ENG\Script.sdat` into the text tools's window
4) Click "Export All". This will create a folder in the above directory called "excel\eng\" with the scripts inside.
5) Change the game's language to Japanese from Steam
6) Repeat steps 3-4, except instead of ENG, it'll be JPN
   - **Note:** A couple of the Japanese scripts will report a mismatch and have a row count mismatch. This will be looked into eventually. For now, you can just click OK and ignore them.
8) Change the game's language back to English and re-drag the `Data\Game\ENG\Script.sdat` file into the text tool's window
9) You should now have these folders:
   - ENG\excel\eng
   - JPN\excel\jpn

10) Copy the jpn folder into the ENG\excel folder, so you have ENG\excel\eng and ENG\excel\jpn
11) Copy all the python scripts from the [pyscripts](pyscripts/) folder into the ENG\excel folder where your eng and jpn folders reside
12) Run `python merge-en-jp.py` and it should produce a "merged" folder with a joint version of the scripts inside:
    - Column E "Japanese" with the Japanese text
    - Column G "Initial" with the English text
    - Column F "Edited" that's blank (though it might not have the "Edited" header

13) (Optional) If you want to more easily modify the stripped scripts in this repo, copy the `stripped` folder where your eng/jpn/merged folder and python scripts are, and run `python merge_stripped_into_merged.py` which will merge all stripped scripts into their merged versions.

### Script Modification and Re-importing
Follow these steps to re-import your modified scripts into the game.

**Column F "Edited", which sits between the original Japanese and English columns, is the column used when importing modified scripts, NOT Column G "Initial".**

1) Run `python strip.py` to create a folder called "stripped" with all the scripts inside it and their original Japanese & English texts removed. **These are the only scripts allowed in this repository.**
2) (Optional) Run `python remove_file_metadata.py` to remove all of Excel's metadata from the scripts
3) If you haven't already, re-drag the Data\Game\ENG\Script.sdat into the text tool's window
4) Select the script you want to import from the dropdown on the top left, or click Import All
5) Click Save on the bottom left, and now your Scripts.sdat file has been updated. The changes will appear in-game.
6) (Optional) If you'd like to commit your changes to this repo, submit a pull request with the modified scripts. **Only stripped scripts without the original texts are allowed.**
7) (Optional) If you want to more easily modify the stripped scripts in this repo, copy the `stripped` folder where your eng/jpn/merged folder and python scripts are, and run `python merge_stripped_into_merged.py` which will merge all stripped scripts into their merged versions.


## Licenses
- [Scarlet](https://github.com/xdanieldzd/Scarlet/blob/master/LICENSE.md)
- [DALTools](https://github.com/thesupersonic16/DALTools)

## Disclaimer
This repo and its contributors are not affiliated with Leaf, Aquaplus, DMM, or Shiravune. None of the original game's content (outside of a screenshot) is in this repo for copyright reasons.

This repo used to exist as a translation project for the 2012 PC version, but has been revamped to "delocalize" some of the content, like the choice to remove honorifics in the names. As such, the code, which was a mess originally, is an even bigger mess now thanks to its quick fixes to support the newer release.
