# White Album Restoration Patch 0.1
A WIP patch that delocalizes some content in White Album (2023 PC Steam version), namely:

- Restoring honorifics (-kun, -san, etc.) along with first/last names used
- Removing Miss, Mr., etc.
- Replacing "Older Lady" with "Senpai"
- Replacing "doll" with Rina-chan

Please support the developers and purchase the game! This project, like the original, was created out of a love for it. We're super fortunate to have gotten an official re-release, and this projects aims to just make tiny adjustments to the script that are ever so slightly more faithful to the original text out of sheer personal preference.

**Note:** As of 8/4/2023, this patch is largely untested and was created mostly automatically, and will be improved over time. Use at your own discretion! **If you run into a problem with this patch applied, open an issue here.**

## How to apply
1) Locate your **Data/Game/ENG/Script.sdat** file. For example, `C:\Program Files (x86)\Steam\steamapps\common\WHITE ALBUM\Data\Game\ENG\Script.sdat`
2) Make a backup of it. For example, just copy/paste the file in Windows so you have a copy of it. If you don't and want to restore, you'll have to re-install the game from Steam.
3) Double-check that you made a backup of it.
4) Download the [xdelta file](wa_honorifics.xdelta) and apply to your **Data/Game/ENG/Script.sdat** file using Delta Patcher.
5) If you wish to restore the backup, simply delete the modified Script.sdat file and replace it with your backup `Script.sdat` file (make sure that's what it's named. **Note that it's `Script.sdat`, as in singular `Script` and `sdat` as the extension**).

Again, if you run into issues like any game crashes, please uninstall the patch using step 5 and open up an issue here instead of reporting an issue to the developers.

## Source
[View the master branch](https://github.com/JuicyStandoffishMan/WhiteAlbumRestorationPatch/tree/master)

![wa](wa.png)
