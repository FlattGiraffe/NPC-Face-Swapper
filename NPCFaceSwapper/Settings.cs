using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace NPCFaceSwapper
{
    public class Settings
    {
        [MaintainOrder]
        [SettingName("Gender Settings - Choose only 1 option, or none.")]
        [Tooltip("If you choose none and you choose no NPC swaps, this patcher will not do anything")]
        public GenderSettings Gender_Settings = new();
        [MaintainOrder]
        public PluginSettings Plugin_Settings = new();
        [MaintainOrder]
        [SettingName("NPC Settings")]
        public NpcSettings Npc_Settings = new();
        [MaintainOrder]
        [SettingName("NPC Swap Settings")]
        public NpcSwapSettings Npc_Swap_Settings = new();
        [MaintainOrder]
        public FutaSettings Futa_Settings  = new ();
        [MaintainOrder]
        public VoiceSettings Voice_Settings  = new ();
        [MaintainOrder]
        public string MO2_Mods_folder ="";
        [MaintainOrder]
        public string MO2_profile_folder = "";
        [MaintainOrder]
        public int Random_Generation_Seed = 0;
    }
    public class PluginSettings
    {
        [MaintainOrder]
        [SettingName("Source Plugin Whitelist - Populate only whitelist or blacklist, not both")]
        public List<ModKey> source_plugins_whitelist = new();
        [MaintainOrder]
        [SettingName("Source Plugin Blacklist - Populate only whitelist or blacklist, not both")]
        public List<ModKey> source_plugins_blacklist = new();

        [MaintainOrder]
        [SettingName("Destination Plugin Whitelist - Populate only whitelist or blacklist, not both")]
        public List<ModKey> dest_plugins_whitelist = new();
        [MaintainOrder]
        [SettingName("Destination Plugin Blacklist - Populate only whitelist or blacklist, not both")]
        public List<ModKey> dest_plugins_blacklist = new();
    }
    public class NpcSettings
    {
        [MaintainOrder]
        [SettingName("Source NPC Whitelist - Populate only whitelist or blacklist, not both")]
        public List<IFormLinkGetter<INpcGetter>> source_npc_whitelist = new();
        [MaintainOrder]
        [SettingName("Source NPC Blacklist - Populate only whitelist or blacklist, not both")]
        public List<IFormLinkGetter<INpcGetter>> source_npc_blacklist = new();

        [MaintainOrder]
        [SettingName("Destination NPC Whitelist - Populate only whitelist or blacklist, not both")]
        public List<IFormLinkGetter<INpcGetter>> dest_npc_whitelist = new();
        [MaintainOrder]
        [SettingName("Destination NPC Blacklist - Populate only whitelist or blacklist, not both")]
        public List<IFormLinkGetter<INpcGetter>> dest_npc_blacklist = new();

        [MaintainOrder]
        [SettingName("Destination Faction Whitelist - Populate only whitelist or blacklist, not both")]
        public List<IFormLinkGetter<IFactionGetter>> dest_fact_whitelist = new();
        [MaintainOrder]
        [SettingName("Destination Faction Blacklist - Populate only whitelist or blacklist, not both")]
        public List<IFormLinkGetter<IFactionGetter>> dest_fact_blacklist = new();

        [MaintainOrder]
        [SettingName("Race Blacklist - List of races to ignore")]
        public List<IFormLinkGetter<IRaceGetter>> race_blacklist = new();
    }
    public class NpcSwapSettings
    { 
        [MaintainOrder]
        [Tooltip("Pick your desired source NPC from a desired source Plugin and choose the destination NPC.")]
        public List<IFormLinkGetter<INpcGetter>> NPC_Swaps_Source_NPC_List = new();
        [MaintainOrder]
        [Tooltip("Pick your desired source NPC from a desired source Plugin and choose the destination NPC.")]
        public List<ModKey> NPC_Swaps_Source_Plugin_List = new();
        [MaintainOrder]
        [Tooltip("Pick your desired source NPC from a desired source Plugin and choose the destination NPC.")]
        public List<IFormLinkGetter<INpcGetter>> NPC_Swaps_Dest_NPC_List = new();
    }
    public class GenderSettings
    {
        [MaintainOrder]
        public bool Make_All_Female=false;
        [MaintainOrder]
        public bool Make_All_Male = false;
        [MaintainOrder]
        public bool Reverse_Genders = false;
        [MaintainOrder]
        [Tooltip("Allow some males to become female and some females to become female. Choice of gender swaps and their source faces will be random.")]
        public bool Randomize_Genders = false;
        [MaintainOrder]
        [Tooltip("Changes faces between females/futas and/or changes faces between males")]
        public bool Randomize_Within_Genders = false;

    }
    public class FutaSettings
    {
        [MaintainOrder]
        [Tooltip("Must have SOS and at least one addon installed.")]
        public bool Set_Former_Males_as_Futa = false;
        [MaintainOrder]
        [Tooltip("Select female applicable (ones that are intended to be \"worn\" by females without issue) SOS addon plugins.")]
        public List<ModKey> Futa_Addon_Plugins = new();
        [MaintainOrder]
        public byte Futa_Size_Min = 5;
        [MaintainOrder]
        public byte Futa_Size_Max = 15;
        [MaintainOrder]
        public byte Futa_Size_randomization = 0;
        // 0 for bell curve (normal) distribution, 1 for uniform dist

        [MaintainOrder]
        [Tooltip("Ensure that your NPC choice will be female.")]
        public List<IFormLinkGetter<INpcGetter>> Futa_Choice_NPC = new();
        [MaintainOrder]
        public List<ModKey> Futa_Choice_Addon=new();
        [MaintainOrder]
        public List<byte> Futa_Choice_Size=new();
    }
    public class VoiceSettings
    {
        [MaintainOrder]
        [Tooltip("Generate voice files for gender changed/voice changed NPCs. Will use xVASynth, so the voices will sound computer generated, to some degree.\nLeave unchecked will leave some lines unvoiced, so you will need Fuz Ro Doh")]
        public bool Generate_Voice_Files=false;
        [MaintainOrder]
        public List<string> additional_xvasynth_female_models = new();
        [MaintainOrder]
        public List<string> additional_xvasynth_male_models = new();

    }

}
