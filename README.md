# NPC Face Swapper

## What this patcher does
Originally I had intended to make a simple modern update to [SkyFem](https://www.loverslab.com/files/file/7549-skyfem-all-npcs-now-female-special-edition/). However, I noticed that this approach could be generalized to several options. This patcher will randomly swap NPC appearances based on user settings. The main algorithm has the following options:
* Make all female
* Make all male
* Reverse genders of all NPCs
* Randomize gender choice

Additionally, the patcher can generate a batch csv for importing into xVASynth in order to generate voice lines which would have changed during swapping.

## How it works
After filtering available NPCs based on user specified settings, it will randomly choose a "destination" NPC (the NPC that a new appearance is applied to), a "source" NPC (whose appearance is taken from the last winning override for that NPC), and the source plugin (that the appearance data originates in). It will make this pairing for all destination NPCs.

Once the pairs have been created, the patcher will copy the current last override for each destination NPC, overwrite its appearance data with the appearance data from the source NPC and store it in the patch mod. This way, any previous mod that alters AI or inventory, weapons, or armor will be intact afterwards.

If the xVASynth option and the voice changer (not selecting original voice) option are both chosen, each of the destination NPCs will have their available spoken dialogue lines examined and repacked into a csv file that xVASynth can read and process.

## Requirements
* For now, you need to be using MO2. Vortex support may come later.
* All NPC overhaul plugins need to occupy the same mod folder as their facegen.
* Facegen must be extracted from BSAs.
* In order to generate xVASynth voices, you will need to install and use that application independently. The only thing this patcher will do is generate the csv that is used to drive the xVASynth generation process.
* If you don't generate voices, you will be able to choose between having original voices and a random vanilla voice. If you choose not to generate voices and have a random voice, you will likely need Fuz Ro Doh. A few vanilla lines are voiced by all voice types, but not many, and definitely not unique dialogue.

## Warning
Generation may take some time and likely depends on your processor and your storage medium. Avoid HDD if at all possible. With my i7700k and SSD, it takes about 12-15 minutes for the npc swap process (~6500 npcs) and another 12-15 for the xVASynth generation. YMMV.

xVASynth generation will take a long time. For example, about 200k lines run on the CPU+GPU (CUDA enabled) setting would take about 65 hours for my RTX 2080.

## Upcoming features
* Vortex support
* Non skyrim xVASynth model support

## Mod Recommendations
Reserved for future mod/patcher recommendations.

## Result
The patcher will generate a new plugin and copy facegen into "NPC Face Swap FaceGen" in your MO2 mods folder. If you choose, it will also generate a batch csv in "NPC Face Swap VoiceGen" in your MO2 mods folder. The batch csv will automatically determine the sub path that the voice file needs to go into when it is generated, however in xVASynth you must choose to direct the output to "NPC Face Swap VoiceGen" or another mod if you so desire. Renaming and/or combining these mods should not be an issue.

## Settings

### Suggested settings
Use the source plugin whitelist, selecting whatever NPC appearance overhauls you have. You can also select follower mods if you want to see their appearance as well.

Use the source NPC blacklist if there are some specific appearances you don't like.

Use the destination NPC blacklist if there are some characters you don't want to change. For example, Inigo or Lucien, since they are full of depth and are fully voiced.

Use the race blacklist if you want to ignore certain races. For example Khajiit and Argonians or OldPeople races.

### Gender Settings

#### Randomize Genders
This setting will flip a coin on each NPC to determine whether or not to change the gender. If successful, a swap will be made for it. If not, the NPC will be left alone.

#### Randomize Within Genders
This setting will randomize unchanged NPCs without changing gender.

---

### Plugin Settings
These plugin settings will form the initial list of NPCs that will be filtered down by additional settings.

#### Source Plugin Whitelist
Select plugins to allow NPC appearances to come only from plugins on this list. Do not populate both the whitelist and the blacklist. I strongly recommend you use this setting and populate it with your chosen NPC appearance overhaul mods.

#### Source Plugin Blacklist
Select plugins to prevent NPC appearances to come from plugins on this list. Do not populate both the whitelist and the blacklist. This option is not recommended because you will have to blacklist all plugins which contain NPC appearences you don't want to see (which could take time for a large mod list).

#### Destination Plugin Whitelist
Select plugins to allow NPC appearances to only overwrite NPCs *originating* in plugins on this list. Do not populate both the whitelist and the blacklist.

#### Destination Plugin Blacklist
Select plugins to prevent NPC appearances to overwrite NPCs *originating* in plugins on this list. Do not populate both the whitelist and the blacklist.

---

### Race Settings
Whitelists and blacklists after this point are no longer mutually exclusive. NPCs listed in following whitelist will be included if not already, and NPCs listed in following blacklists will be removed if they were included prior.

#### Source Race Blacklist
Select a race whose NPCs should *not* be used to provide appearances.

#### Destination Race Blacklist
Select a race whose NPCs are *not* to be changed.

---

### NPC Settings

#### Source NPC Whitelist
Select specific NPCs to provide their appearances for swaps. Their appearance is based on the last winning conflict. Only these will be selected for swaps.

#### Source NPC Blacklist
Select specific NPCs which should *not* to provide their appearances. 

#### Destination NPC Whitelist
Select specific NPCs to be changed with new appearances.

#### Destination NPC Blacklist
Select specific NPCs to avoid being changed, that otherwise would be selected.

#### Destination Faction Whitelist
Select factions whose NPCs are to be changed.

#### Destination Faction Whitelist
Select factions whose NPCs are *not* to be changed.

---

### NPC Swap Settings
Use these three lists in order to specify a specific appearance swap. You need to specify three things in order to make a swap: a destination NPC, a source NPC, a source Plugin which contains the source NPCs appearance data that is desired. It can even be one that is overwritten.

You will need to populate each list carefully. The n-th position in each list is correlated with each other, e.g. the 3rd NPC in the destination list will get their new appearance from the 3rd NPC in the source list as determined by the 3rd plugin in the source plugin list.

If your order is incorrect between the lists, you will get the wrong swaps, which isn't detectable by the program. If you forget to input something on one of the lists you will get an error.

---

### Futa Settings
You can set males which have been changed into females to have schlongs if you have Schlongs of Skyrim and an appropriate SOS addon installed. You will need to select all addons whose schlongs you wish to see applied. Selecting multiple addons will result in a random choice for valid NPCs.
If you wish to turn all into futanari or want the selection to be random rather than just former male NPCs, you should instead utilize SOS's MCM menu for schlong distribution and not use this setting.

#### Futa Size Randomization
Select 0 for a normal distribution or select 1 for a uniform distribution of sizes.

## Futa Choice settings
These three lists function the same as the NPC swap lists.

The lists are provided make a specific schlong selection for a specific NPC. You will need to choose the NPC, the addon plugin, and the size (between 1 and 20).

In the case where you wish only to specify the addon or the size you can use the following:
Select the Skyrim.esm plugin to use one of your specified addon plugins at random.
Select -1 as the schlong size to choose the size randomly, according to the Futa Size Randomization setting.

---

### Voice Settings
You can choose to generate the xVASynth batch csv, silent or original voice. 

### MO2 Mods folder
Please provide the path to your MO2 mods folder

### MO2 profile folder
Please provide the path to your MO2 profile folder

## Manually edit settings
You can view or edit your settings json manually, it is located in (Synthesis installation folder)/Data/Skyrim Special Edition/NPC Face Swapper/settings.json

## Help or Suggestions
For now, create an issue. For help, post your log.txt found in (Synthesis installation folder)/Data/Skyrim Special Edition/NPC Face Swapper/log.txt

## Changelog
1.0.0 - Initial release
1.1.0 - Updated readme, completed functionality on voice options, included race based npc filtering option