# NPCFaceSwapper

## What this patcher does
Originally I had inteded to make a simple modern update to [SkyFem](https://www.loverslab.com/files/file/7549-skyfem-all-npcs-now-female-special-edition/). However, I noticed that this approach could be generalized to several options. This patcher will randomly swap NPC appearances based on user settings. The main algorithm has the following options:
* Make all female
* Make all male
* Reverse genders of all NPCs
* Randomize gender choice

Additionally, the patcher can generate a batch csv for importing into xVASynth in order to generate voice lines which would have changed during swapping.

## Requirements
* For now, you need to be using MO2. Vortex support may come later.
* All NPC overhaul plugins need to occupy the same mod folder as their facegen.
* In order to generate xVASynth voices, you will need to install and use that application independently. The only thing this patcher will do is generate the csv that is used to drive the xVASynth generation process.
* If you don't generate voices, you will be able to choose between having original voices and no voice. If you choose not to have a voice, you will likely need Fuz Ro Doh.

## Before you start
Generation may take some time and likely depends on your processor and your storage medium. Avoid HDD if at all possible. With my i7700k and SSD, it takes about 12-15 minutes for the npc swap process (~6500 npcs) and another 12-15 for the xVASynth generation. 

YMMV.

## Recommendations
Reserved for future mod/patcher recommendations.

## Result
The patcher will generate a new plugin and copy facegen into "NPC Face Swap FaceGen" in your MO2 mods folder. If you choose, it will also generate a batch csv in "NPC Face Swap VoiceGen" in your MO2 mods folder. The batch csv will automatically determine the sub path that the voice file needs to go into when it is generated, however in xVASynth you must choose to direct the output to "NPC Face Swap VoiceGen" or another mod if you so desire. Renaming and/or combining these mods should not be an issue.

## Settings

### Gender Settings

#### Randomize Genders
This setting will flip a coin on each NPC to determine whether or not to change the gender. If successful, a swap will be made for it. If not, the NPC will be left alone

#### Randomize Within Genders
This setting is not implemented yet.

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

### NPC Settings

#### Source NPC Whitelist
Select specific NPCs to provide their appearances for swaps. Their appearance is based on the last winning conflict. Only these will be selected for swaps. Do not populate both the whitelist and the blacklist.

#### Source NPC Blacklist
Select specific NPCs *not* to provide their appearances. Do not populate both the whitelist and the blacklist.

#### Destination NPC Whitelist
Select specific NPCs to be the recipients for new appearances. Only the selected NPCs will have their appearance changed. Do not populate both the whitelist and the blacklist.

#### Destination NPC Blacklist
Select specific NPCs to avoid being changed, that otherwise would be selected. Do not populate both the whitelist and the blacklist.

#### Race Blacklist
Select a race that should be ignored both as a source and as a destination.

### NPC Swap Settings
Use these three lists in order to specify a specific appearance swap. You need to specify three things in order to make a swap: a destination NPC, a source NPC, a source Plugin which contains the source NPCs appearance data that is desired. It can even be one that is overwritten.
You will need to populate each list carefully. The n-th position in each list will be paired with each other, so if your order is incorrect between the lists, you will get the wrong swaps.

### Futa Settings
You can set males which have been changed into females to have schlongs if you have Schlongs of Skyrim and an appropriate SOS addon installed. You will need to select all addons whose schlongs you wish to see applied. Selecting multiple addons will result in a random choice for valid NPCs.
If you wish to turn all into futanari or want the selection to be random rather than just former male NPCs, you should instead utilize SOS's MCM menu for schlong distribution and not use this setting.

Additionally, three lists are provided again which work to make a specific schlong selection for a specific NPC. You will need to choose the NPC, the addon plugin, and the size (between 1 and 20).

#### Futa Size Randomization
Select 0 for a normal distribution or Select 1 for a uniform distribution of sizes.

### Voice Settings
You can choose to generate the xVASynth batch csv, silent or original voice, or provide non vanilla xVASynth model names that can be used in a pool when a random voice needs to be chosen.

### MO2 Mods folder
Please provide the path to your MO2 mods folder

## MO2 profile folder
Please provide the path to your MO2 profile folder

## Help
For now, create an issue. Post your log.txt found in (Synthesis installation folder)/Data/Skyrim Special Edition/NPC Face Swapper/log.txt