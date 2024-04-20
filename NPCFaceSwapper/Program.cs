using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins.Order;
using System.Collections.Concurrent;
using System.Diagnostics;
using DynamicData;
using System.Linq;
using Synthesis.Bethesda;


namespace NPCFaceSwapper
{
    public class Program
    {

        static Lazy<Settings> Settings = null!;
        static string log_path = "";
        static bool log_console_output = true;
        static bool log_file_output = true;
        static Random rnd = new();
        static bool add_sexlab_kw = false;
        static string modsfolder = "";
        static IPatcherState<ISkyrimMod, ISkyrimModGetter>? state;
        static Dictionary<string, List<FormKey>> race_groups = new();
        static List<string> child_whitelist_words = new() { "child" };
        static List<string> race_blacklist_words = new() { "draugr" };


        static Dictionary<string, string> xvasynth_model_names_male = new()
        {
            {"MaleUniqueAncano","sk_ancano" },
            {"CrUniqueAlduin","sk_alduin" },
            {"MaleUniqueArngeir","sk_arngeir" },
            {"MaleUniqueBrynjolf","sk_brynjolf" },
            {"MaleUniqueCicero","sk_cicero" },
            {"MaleUniqueDelvinMallory","sk_delvin" },
            {"CrDragonVoice","sk_dragon" },
            {"MaleUniqueEsbern","sk_esbern" },
            {"MaleUniqueGalmar","sk_galmar" },
            {"MaleUniqueHadvar","sk_hadvar" },
            {"MaleUniqueKodlakWhitemane","sk_kodlakwhitemane" },
            {"MaleUniqueMercerFrey","sk_mercerfrey" },
            {"MaleUniqueNazir","sk_nazir" },
            {"CrUniqueOdahviing","sk_odahviing" },
            {"CrUniquePaarthurnax","sk_paarthurnax" },
            {"MaleUniqueTullius","sk_tullius" },
            {"MaleUniqueUlfric","sk_ulfric" },
            {"DLC1MaleUniqueFlorentius","sk_florentius" },
            {"DLC1MaleUniqueIsran","sk_isran" },
            {"DLC1MaleUniqueHarkon","sk_harkon" },
            {"DLC1MaleUniqueGelebor","sk_gelebor" },
            {"DLC1MaleUniqueDexion","sk_dexion" },
            {"DLC1MaleVampire","sk_malevampire"},
            {"DLC2MaleUniqueNeloth","sk_neloth" },
            {"DLC2MaleUniqueStorn","sk_storn" },
            {"DLC2MaleUniqueLleril","sk_lleril" },
            {"DLC2MaleUniqueAdril","sk_adril" },
            {"DLC2MaleUniqueModyn","sk_modyn" },
            {"DLC2MaleDarkElfCommoner","sk_maledarkelfcommoner" },
            {"DLC2MaleDarkElfCynical","sk_maledarkelfcynical"},

            {"MaleArgonian","sk_maleargonian"},
            {"MaleBandit","sk_malebandit"},
            {"MaleBrute","sk_malebrute"},
            {"MaleCommander","sk_malecommander"},
            {"MaleCommoner","sk_malecommoner"},
            {"MaleCommonerAccented","sk_malecommoneraccented"},
            {"MaleCondescending","sk_malecondescending"},
            {"MaleCoward","sk_malecoward"},
            {"MaleDrunk","sk_maledrunk"},
            {"MaleDarkElf","sk_maledunmer"},
            {"MaleElfHaughty","sk_maleelfhaughty"},
            {"MaleEvenToned","sk_maleeventoned"},
            {"MaleEvenTonedAccented","sk_maleeventonedaccented"},
            {"MaleGuard","sk_maleguard"},
            {"MaleKhajiit","sk_malekhajiit"},
            {"MaleNord","sk_malenord"},
            {"MaleNordCommander","sk_malenordcommander"},
            {"MaleOldGrumpy","sk_maleoldgrumpy"},
            {"MaleOldKindly","sk_maleoldkindly"},
            {"MaleOrc","sk_maleorc"},
            {"MaleSlyCynical","sk_maleslycynical"},
            {"MaleSoldier","sk_malesoldier"},
            {"MaleWarlock","sk_malewarlock"},
            {"MaleYoungEager","sk_maleyoungeager"}
        };

        static Dictionary<string, string> xvasynth_model_names_female = new()
        {
            {"FemaleUniqueAstrid","sk_astrid" },
            {"FemaleUniqueDelphine","sk_delphine" },
            {"FemaleUniqueElenwen","sk_elenwen" },
            {"FemaleUniqueKarliah","sk_karliah" },
            {"FemaleUniqueMaven","sk_maven" },
            {"FemaleUniqueMirabelleErvine","sk_mirabelleervine" },
            {"FemaleUniqueVex","sk_vex" },
            {"DLC1SeranaVoice","sk_serana" },
            {"DLC1FemaleUniqueValerica","sk_valerica" },
            {"DLC1FemaleUniqueFura","sk_fura" },
            {"DLC1FemaleVampire","sk_femalevampire"},
            {"DLC2FemaleUniqueFrea","sk_frea" },
            {"DLC2FemaleDarkElfCommoner","sk_femaledarkelfcommoner" },

            {"FemaleArgonian","sk_femaleargonian"},
            {"FemaleCommander","sk_femalecommander"},
            {"FemaleCommoner","sk_femalecommoner"},
            {"FemaleCondescending","sk_femalecondescending"},
            {"FemaleCoward","sk_femalecoward"},
            {"FemaleDarkElf","sk_femaledarkelf"},
            {"FemaleElfHaughty","sk_femaleelfhaughty"},
            {"FemaleEvenToned","sk_femaleeventoned"},
            {"FemaleKhajiit","sk_femalekhajiit"},
            {"FemaleNord","sk_femalenord"},
            {"FemaleOldGrumpy","sk_femaleoldgrumpy"},
            {"FemaleOldKindly","sk_femaleoldkindly"},
            {"FemaleOrc","sk_femaleorc"},
            {"FemaleShrill","sk_femaleshrill"},
            {"FemaleSultry","sk_femalesultry"},
            {"FemaleYoungEager","sk_femaleyoungeager"}
        };
        static List<string> voices_xvasynth_male = new();
        static List<string> voices_xvasynth_female = new();

        static List<string> vanilla_skyrim = new()
        {
            "skyrim",
            "dawnguard",
            "hearthfires",
            "dragonborn",
            "update"
        };

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(
                    nickname: "Settings",
                    path: "settings.json",
                    out Settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "NPC Face Swap.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> st)
        {
            state = st;
            log_path = Path.Combine(state.ExtraSettingsDataPath!, "log.txt");
            if (File.Exists(log_path)) File.Delete(log_path);

            //check settings for valid config
            if (!Validate_Settings()) return;

            modsfolder = Settings.Value.MO2_Mods_folder;
            rnd = new Random(Settings.Value.Random_Generation_Seed);

            // add player to blacklist
            var player = state.LinkCache.Resolve<INpcGetter>(FormKey.Factory("000007:Skyrim.esm"));
            Settings.Value.Npc_Settings.dest_npc_blacklist.Add(player);

            // add children to blacklist
            var child_race_list = new List<FormKey>();
            foreach (var race in state.LoadOrder.PriorityOrder.Race().WinningOverrides())
            {
                bool found = false;
                if (race.EditorID != null)
                {
                    foreach (string bl in child_whitelist_words)
                    {
                        if (race.EditorID.Contains(bl, StringComparison.OrdinalIgnoreCase))
                        {
                            //Settings.Value.Race_Settings.source_race_blacklist.Add(race);
                            //Settings.Value.Race_Settings.dest_race_blacklist.Add(race);
                            child_race_list.Add(race.FormKey);
                            found = true;
                            break;
                        }
                    }
                    foreach (string bl in race_blacklist_words)
                    {
                        if (race.EditorID.Contains(bl, StringComparison.OrdinalIgnoreCase))
                        {
                            Settings.Value.Race_Settings.destination_race_blacklist.Add(race.FormKey);
                            Settings.Value.Race_Settings.source_race_blacklist.Add(race.FormKey);
                            found = true;
                            break;
                        }
                    }
                }
                if (found) continue;

                foreach (var v in race.Voices)
                {
                    var voice = v.Resolve<IVoiceTypeGetter>(state.LinkCache);
                    if (voice.EditorID != null)
                    {
                        foreach (string bl in child_whitelist_words)
                        {
                            if (voice.EditorID.Contains(bl, StringComparison.OrdinalIgnoreCase))
                            {
                                //Settings.Value.Race_Settings.source_race_blacklist.Add(race);
                                //Settings.Value.Race_Settings.dest_race_blacklist.Add(race);
                                child_race_list.Add(race.FormKey);
                                found = true;
                                break;
                            }
                        }
                        foreach (string bl in race_blacklist_words)
                        {
                            if (voice.EditorID.Contains(bl, StringComparison.OrdinalIgnoreCase))
                            {
                                Settings.Value.Race_Settings.destination_race_blacklist.Add(race.FormKey);
                                Settings.Value.Race_Settings.source_race_blacklist.Add(race.FormKey);
                                found = true;
                                break;
                            }
                        }
                    }
                    if (found) break;
                }
                if (found) continue;

                if (race.Skin.TryResolve<IArmorGetter>(state.LinkCache,out var skin))
                {
                    if (skin.EditorID != null)
                    {
                        foreach (string bl in child_whitelist_words)
                        {
                            if (skin.EditorID.Contains(bl, StringComparison.OrdinalIgnoreCase))
                            {
                                //Settings.Value.Race_Settings.source_race_blacklist.Add(race);
                                //Settings.Value.Race_Settings.dest_race_blacklist.Add(race);
                                child_race_list.Add(race.FormKey);
                                found = true;
                                break;
                            }
                        }
                        foreach (string bl in race_blacklist_words)
                        {
                            if (skin.EditorID.Contains(bl, StringComparison.OrdinalIgnoreCase))
                            {
                                Settings.Value.Race_Settings.destination_race_blacklist.Add(race.FormKey);
                                Settings.Value.Race_Settings.source_race_blacklist.Add(race.FormKey);
                                found = true;
                                break;
                            }
                        }
                    }
                }
                if (found) continue;
            }

            if (Settings.Value.Race_Settings.Use_Race_Based_Matching)
            {
                if (Settings.Value.Race_Settings.Strict_Race_Matching)
                {
                    race_groups["Breton"] = new List<FormKey>()
                    {
                        FormKey.Factory("013741:Skyrim.esm"), //breton
                        FormKey.Factory("097A3E:Skyrim.esm"), //breton afflicted
                        FormKey.Factory("08883C:Skyrim.esm"), //BRETON vamp
                    };
                    race_groups["Imperial"] = new List<FormKey>()
                    {
                        FormKey.Factory("013744:Skyrim.esm"), //imperial
                        FormKey.Factory("088844:Skyrim.esm"), //IMPERIAL vamp
                    };
                    race_groups["Nord"] = new List<FormKey>()
                    {
                        FormKey.Factory("013746:Skyrim.esm"), //nord
                        FormKey.Factory("088794:Skyrim.esm"), //nord vamp
                        FormKey.Factory("03CA97:Dragonborn.esm"), //miraak
                    };
                    race_groups["Redguard"] = new List<FormKey>()
                    {
                        FormKey.Factory("013748:Skyrim.esm"), //redguard
                        FormKey.Factory("088846:Skyrim.esm"), //REDGUARD vamp
                    };
                    race_groups["DarkElf"] = new List<FormKey>()
                    {
                        FormKey.Factory("013742:Skyrim.esm"), //DARK ELF
                        FormKey.Factory("08883D:Skyrim.esm"), //DARK ELF VAMP
                    };
                    race_groups["HighElf"] = new List<FormKey>()
                    {
                        FormKey.Factory("013743:Skyrim.esm"), //HIGH ELF
                        FormKey.Factory("088840:Skyrim.esm"), //HIGH ELF VAMP
                    };
                    race_groups["Orc"] = new List<FormKey>()
                    {
                        FormKey.Factory("013747:Skyrim.esm"), //ORC
                        FormKey.Factory("0A82B9:Skyrim.esm"), //ORC VAMP
                    };
                    race_groups["WoodElf"] = new List<FormKey>()
                    {
                        FormKey.Factory("013749:Skyrim.esm"), //WOOD ELF
                        FormKey.Factory("088884:Skyrim.esm"), //WOOD ELF VAMP
                    };
                    race_groups["Argonian"] = new List<FormKey>()
                    {
                        FormKey.Factory("013740:Skyrim.esm"), //ARGONIAN
                        FormKey.Factory("08883A:Skyrim.esm"), //ARGONIAN VAMP

                    };
                    race_groups["Khajiit"] = new List<FormKey>()
                    {
                        FormKey.Factory("013745:Skyrim.esm"), //KHAJIIT
                        FormKey.Factory("088845:Skyrim.esm"), //KHAJIIT VAMP
                    };
                    race_groups["Elder"] = new List<FormKey>()
                    {
                        FormKey.Factory("067CD8:Skyrim.esm"), //elder
                        FormKey.Factory("0A82BA:Skyrim.esm"), //elder VAMP
                    };
                    race_groups["SnowElf"] = new List<FormKey>()
                    {
                        FormKey.Factory("00377D:Dawnguard.esm"), //snow elf
                    };
                    race_groups["Dremora"] = new List<FormKey>()
                    {
                        FormKey.Factory("0131F0:Skyrim.esm"), //dremora
                    };

                }
                else if (Settings.Value.Race_Settings.Humanlike_Race_Matching)
                {
                    race_groups["Humanlike"] = new List<FormKey>()
                    {
                        FormKey.Factory("013741:Skyrim.esm"), //breton
                        FormKey.Factory("097A3D:Skyrim.esm"), //breton afflicted
                        FormKey.Factory("013744:Skyrim.esm"), //imperial
                        FormKey.Factory("013746:Skyrim.esm"), //nord
                        FormKey.Factory("03CA97:Dragonborn.esm"), //miraak
                        FormKey.Factory("013748:Skyrim.esm"), //redguard
                        FormKey.Factory("088794:Skyrim.esm"), //nord vamp
                        FormKey.Factory("08883C:Skyrim.esm"), //BRETON vamp
                        FormKey.Factory("088844:Skyrim.esm"), //IMPERIAL vamp
                        FormKey.Factory("088846:Skyrim.esm"), //REDGUARD vamp
                        FormKey.Factory("013742:Skyrim.esm"), //DARK ELF
                        FormKey.Factory("013743:Skyrim.esm"), //HIGH ELF
                        FormKey.Factory("013747:Skyrim.esm"), //ORC
                        FormKey.Factory("013749:Skyrim.esm"), //WOOD ELF
                        FormKey.Factory("08883D:Skyrim.esm"), //DARK ELF VAMP
                        FormKey.Factory("088840:Skyrim.esm"), //HIGH ELF VAMP
                        FormKey.Factory("088884:Skyrim.esm"), //WOOD ELF VAMP
                        FormKey.Factory("0A82B9:Skyrim.esm"), //ORC VAMP
                        FormKey.Factory("067CD8:Skyrim.esm"), //elder
                        FormKey.Factory("0A82BA:Skyrim.esm"), //elder VAMP
                        FormKey.Factory("00377D:Dawnguard.esm"), //snow elf
                        FormKey.Factory("0131F0:Skyrim.esm"), //dremora
                    };
                    race_groups["Argonian"] = new List<FormKey>()
                    {
                        FormKey.Factory("013740:Skyrim.esm"), //ARGONIAN
                        FormKey.Factory("08883A:Skyrim.esm"), //ARGONIAN VAMP

                    };
                    race_groups["Khajiit"] = new List<FormKey>()
                    {
                        FormKey.Factory("013745:Skyrim.esm"), //KHAJIIT
                        FormKey.Factory("088845:Skyrim.esm"), //KHAJIIT VAMP
                    };
                }
                else
                {
                    race_groups["Human"] = new List<FormKey>()
                    {
                        FormKey.Factory("013741:Skyrim.esm"), //breton
                        FormKey.Factory("097A3D:Skyrim.esm"), //breton afflicted
                        FormKey.Factory("013744:Skyrim.esm"), //imperial
                        FormKey.Factory("013746:Skyrim.esm"), //nord
                        FormKey.Factory("03CA97:Dragonborn.esm"), //miraak
                        FormKey.Factory("013748:Skyrim.esm"), //redguard
                        FormKey.Factory("088794:Skyrim.esm"), //nord vamp
                        FormKey.Factory("08883C:Skyrim.esm"), //BRETON vamp
                        FormKey.Factory("088844:Skyrim.esm"), //IMPERIAL vamp
                        FormKey.Factory("088846:Skyrim.esm"), //REDGUARD vamp
                        FormKey.Factory("067CD8:Skyrim.esm"), //elder
                        FormKey.Factory("0A82BA:Skyrim.esm"), //elder VAMP
                    };
                    race_groups["Elves"] = new List<FormKey>()
                    {
                        FormKey.Factory("013742:Skyrim.esm"), //DARK ELF
                        FormKey.Factory("013743:Skyrim.esm"), //HIGH ELF
                        FormKey.Factory("013747:Skyrim.esm"), //ORC
                        FormKey.Factory("013749:Skyrim.esm"), //WOOD ELF
                        FormKey.Factory("08883D:Skyrim.esm"), //DARK ELF VAMP
                        FormKey.Factory("088840:Skyrim.esm"), //HIGH ELF VAMP
                        FormKey.Factory("088884:Skyrim.esm"), //WOOD ELF VAMP
                        FormKey.Factory("0A82B9:Skyrim.esm"), //ORC VAMP
                        FormKey.Factory("00377D:Dawnguard.esm"), //snow elf
                        FormKey.Factory("0131F0:Skyrim.esm"), //dremora
                    };
                    race_groups["Argonian"] = new List<FormKey>()
                    {
                        FormKey.Factory("013740:Skyrim.esm"), //ARGONIAN
                        FormKey.Factory("08883A:Skyrim.esm"), //ARGONIAN VAMP

                    };
                    race_groups["Khajiit"] = new List<FormKey>()
                    {
                        FormKey.Factory("013745:Skyrim.esm"), //KHAJIIT
                        FormKey.Factory("088845:Skyrim.esm"), //KHAJIIT VAMP
                    };

                }
            }
            else
            {
                race_groups["Adult"] = new List<FormKey>()
                {
                    FormKey.Factory("013741:Skyrim.esm"), //breton
                    FormKey.Factory("097A3D:Skyrim.esm"), //breton afflicted
                    FormKey.Factory("013744:Skyrim.esm"), //imperial
                    FormKey.Factory("013746:Skyrim.esm"), //nord
                    FormKey.Factory("013748:Skyrim.esm"), //redguard
                    FormKey.Factory("088794:Skyrim.esm"), //nord vamp
                    FormKey.Factory("03CA97:Dragonborn.esm"), //miraak
                    FormKey.Factory("08883C:Skyrim.esm"), //BRETON vamp
                    FormKey.Factory("088844:Skyrim.esm"), //IMPERIAL vamp
                    FormKey.Factory("088846:Skyrim.esm"), //REDGUARD vamp
                    FormKey.Factory("013742:Skyrim.esm"), //DARK ELF
                    FormKey.Factory("013743:Skyrim.esm"), //HIGH ELF
                    FormKey.Factory("013747:Skyrim.esm"), //ORC
                    FormKey.Factory("013749:Skyrim.esm"), //WOOD ELF
                    FormKey.Factory("08883D:Skyrim.esm"), //DARK ELF VAMP
                    FormKey.Factory("088840:Skyrim.esm"), //HIGH ELF VAMP
                    FormKey.Factory("088884:Skyrim.esm"), //WOOD ELF VAMP
                    FormKey.Factory("0A82B9:Skyrim.esm"), //ORC VAMP
                    FormKey.Factory("013740:Skyrim.esm"), //ARGONIAN
                    FormKey.Factory("08883A:Skyrim.esm"), //ARGONIAN VAMP
                    FormKey.Factory("013745:Skyrim.esm"), //KHAJIIT
                    FormKey.Factory("088845:Skyrim.esm"), //KHAJIIT VAMP
                    FormKey.Factory("067CD8:Skyrim.esm"), //elder
                    FormKey.Factory("0A82BA:Skyrim.esm"), //elder VAMP
                    FormKey.Factory("00377D:Dawnguard.esm"), //snow elf
                    FormKey.Factory("0131F0:Skyrim.esm"), //dremora
                };

            }
            race_groups["Child"] = new List<FormKey>()
            {
                FormKey.Factory("108272:Skyrim.esm"), //BRETON CHILD vamp
                FormKey.Factory("02C659:Skyrim.esm"), //IMPERIAL CHILD
                FormKey.Factory("02C65A:Skyrim.esm"), //REDGUARD CHILD
                FormKey.Factory("02C65B:Skyrim.esm"), //NORD CHILD
                FormKey.Factory("02C65C:Skyrim.esm"), //BRETON CHILD
            };
            race_groups["Other"] = new List<FormKey>();

            foreach (var r in child_race_list)
            {
                if (race_groups["Child"].Contains(r)) continue;
                race_groups["Child"].Add(r);
            }

            foreach (var kvp in xvasynth_model_names_female)
            {
                voices_xvasynth_female.Add(kvp.Value);
            }
            foreach (var kvp in xvasynth_model_names_male)
            {
                voices_xvasynth_male.Add(kvp.Value);
            }

            //add additional xvasyth models user provided
            if (Settings.Value.Voice_Settings.additional_xvasynth_female_models.Count > 0)
            {
                voices_xvasynth_female.AddRange(Settings.Value.Voice_Settings.additional_xvasynth_female_models);
            }
            if (Settings.Value.Voice_Settings.additional_xvasynth_male_models.Count > 0)
            {
                voices_xvasynth_male.AddRange(Settings.Value.Voice_Settings.additional_xvasynth_male_models);
            }




            //iterate through all races for valid ones
            // given that it's difficult to evaluate a race to determine whether its npcs will be valid, better to just check for facegen in each npc record
            //var valid_races = FindValidRaces(state);

            // SynArmorAAFixMissingGenderWorldModel patcher fixes the world models (nothing for weight sliders or first person
            // still probably better than this for now
            //EditArma(state);

            Log("Filtering valid npcs");



            //start by filtering down destination npc list based on settings
            var dest_npcs_male = new List<FormLink<INpcGetter>>();
            var dest_npcs_female = new List<FormLink<INpcGetter>>();
            var source_npcs_male = new List<FormLink<INpcGetter>>();
            var source_npcs_female = new List<FormLink<INpcGetter>>();


            Filter_NPCs(ref dest_npcs_male, ref dest_npcs_female, ref source_npcs_male, ref source_npcs_female);


            Log("Running matchmaker");


            //continue to populate the map with random choices (if any exist), given the source and destination filters and settings
            var npc_map=Matchmaker(source_npcs_female, source_npcs_male, dest_npcs_female, dest_npcs_male);

            Log("Writing swaps to plugin");

            //iterate through the mapping to replace appearance data in each destination npc with the appropriate source records
            var changed_voices = Swap_NPCs(npc_map);


            //voice gen. generate batch csv for use in xvasynth, user will have to use xvasynth on their own as there is no cli for it
            if (Settings.Value.Voice_Settings.Generate_Voice_CSV)
            {
                Log("Creating xVaSynth CSV");
                Generate_xVASynth_CSV(changed_voices);

            }// if generate voices

            //probably not going to be a problem, given all records in the plugin should be overrides
            state.PatchMod.ModHeader.Flags |= SkyrimModHeader.HeaderFlag.LightMaster;
        }

        private static bool Validate_Settings()
        {
            if (Settings.Value.Plugin_Settings.source_plugins_blacklist.Count > 0
                && Settings.Value.Plugin_Settings.source_plugins_whitelist.Count > 0)
            {
                Log("Plugins are listed in both the Source Plugins Whitelist and the Source Plugins Blacklist\nPlease only use one list for Source Plugins");
                return false;
            }
            if (Settings.Value.Plugin_Settings.dest_plugins_blacklist.Count > 0
                && Settings.Value.Plugin_Settings.dest_plugins_whitelist.Count > 0)
            {
                Log("Plugins are listed in both the Destination Plugins Whitelist and the Destination Plugins Blacklist\nPlease only use one list for Destination Plugins");
                return false;
            }

            if (Settings.Value.Npc_Swap_Settings.NPC_Swaps_Source_NPC_List.Count !=
                Settings.Value.Npc_Swap_Settings.NPC_Swaps_Dest_NPC_List.Count ||
                Settings.Value.Npc_Swap_Settings.NPC_Swaps_Source_NPC_List.Count !=
                Settings.Value.Npc_Swap_Settings.NPC_Swaps_Source_Plugin_List.Count)
            {
                Log("NPC Swap settings are incorrect. The Source, Dest, and Plugin lists must have the same length");
                return false;
            }
            if (Settings.Value.Race_Settings.Use_Race_Based_Matching)
            {
                if (Settings.Value.Race_Settings.Strict_Race_Matching && Settings.Value.Race_Settings.Humanlike_Race_Matching)
                {
                    Log("Only one niche race based matching setting can be used. Choose either Strict, Humanlike, or neither.");
                    return false;
                }
            }

            var gendersettings = Convert.ToInt16(Settings.Value.Gender_Settings.Make_All_Male) +
                Convert.ToInt16(Settings.Value.Gender_Settings.Make_All_Female) +
                Convert.ToInt16(Settings.Value.Gender_Settings.Randomize_Genders)+
                Convert.ToInt16(Settings.Value.Gender_Settings.Reverse_Genders);
            if (gendersettings > 1)
            {
                Log("Only one main gender setting can be used.");
                return false;
            }

            if (Settings.Value.Npc_Swap_Settings.NPC_Swaps_Source_NPC_List.Count == 0 &&
                gendersettings < 1 && !Settings.Value.Gender_Settings.Randomize_Within_Genders)
            {
                Log("At least one Gender setting must be selected if there are no NPC swaps to perform.");
                return false;
            }

            if (Settings.Value.Futa_Settings.Futa_Choice_NPC.Count !=
                Settings.Value.Futa_Settings.Futa_Choice_Addon.Count ||
                Settings.Value.Futa_Settings.Futa_Choice_NPC.Count !=
                Settings.Value.Futa_Settings.Futa_Choice_Size.Count)
            {
                Log("Futa choice settings are incorrect. The NPC, Size, and Addon lists must have the same length");
                return false;
            }

            if (Settings.Value.Futa_Settings.Futa_Size_Max < 1 || Settings.Value.Futa_Settings.Futa_Size_Max > 20)
            {
                Log("Futa size max must be between 1 and 20");
                return false;
            }
            if (Settings.Value.Futa_Settings.Futa_Size_Min < 1 || Settings.Value.Futa_Settings.Futa_Size_Min > 20)
            {
                Log("Futa size min must be between 1 and 20");
                return false;
            }
            if (Settings.Value.Futa_Settings.Futa_Size_randomization != 0 &&
                Settings.Value.Futa_Settings.Futa_Size_randomization != 1)
            {
                Log("Futa size randomization must be 0 for normal (bell curve) or 1 for uniform distribution");
                return false;
            }
            if (Settings.Value.MO2_Mods_folder == "")
            {
                Log("MO2 mods folder is not set.");
                return false;

            }
            if (Settings.Value.MO2_profile_folder == "")
            {
                Log("MO2 profile folder is not set.");
                return false;

            }
            if (Settings.Value.Futa_Settings.Set_Former_Males_as_Futa && Settings.Value.Futa_Settings.Futa_Addon_Plugins.Count == 0)
            {
                Log("You must select at least one SOS addon to set former males as futa.");
                return false;
            }

            if (Settings.Value.Voice_Settings.Generate_Voice_CSV && Settings.Value.Voice_Settings.Use_Original_Voice)
            {
                Log("You can select only \"Generate Voice CSV\" or \"Use Original Voice\", not both.");
                return false;
            }
            if (Settings.Value.Voice_Settings.Use_Original_Voice && Settings.Value.Voice_Settings.Use_Random_Voice)
            {
                Log("You can select only \"Use Random Voice\" or \"Use Original Voice\", not both.");
                return false;
            }

            return true;
        }

        private static string IncrementCounter(string cnt,int len)
        {
            int val = Convert.ToInt16(cnt);
            val++;
            return JustifyNumber(val,len);
        }

        private static string JustifyNumber(int val,int len)
        {
            string ret = val.ToString();
            while (ret.Length < len)
            {
                ret = "0" + ret;
            }
            return ret;
        }

        private static void Filter_NPCs(
            ref List<FormLink<INpcGetter>> dest_m,
            ref List<FormLink<INpcGetter>> dest_f,
            ref List<FormLink<INpcGetter>> source_m,
            ref List<FormLink<INpcGetter>> source_f)
        {

            //filter by plugin
            if (Settings.Value.Plugin_Settings.dest_plugins_whitelist.Count > 0)
            {
                foreach (var npcContext in state!.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {

                    if (Settings.Value.Plugin_Settings.dest_plugins_whitelist.Contains(npcContext.ModKey))
                    {
                        //filter by incompatible NPCs (creatures, animals, anything without facegen)
                        if (npcContext.Record.HeadParts == null || npcContext.Record.HeadParts.Count == 0) continue;
                        if (npcContext.Record.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits)) continue;
                        if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.IsCharGenFacePreset)) continue;

                        if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female)) dest_f.Add(new FormLink<INpcGetter>(npcContext.Record));
                        else dest_m.Add(new FormLink<INpcGetter>(npcContext.Record));
                    }
                }
            }
            else
            {
                foreach (var npcContext in state!.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {
                    if (Settings.Value.Plugin_Settings.dest_plugins_blacklist.Contains(npcContext.ModKey)) continue;

                    //filter by incompatible NPCs (creatures, animals, anything without facegen)
                    if (npcContext.Record.HeadParts == null || npcContext.Record.HeadParts.Count==0) continue;
                    if (npcContext.Record.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits)) continue;
                    if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.IsCharGenFacePreset)) continue;

                    if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female)) dest_f.Add(new FormLink<INpcGetter>(npcContext.Record));
                    else dest_m.Add(new FormLink<INpcGetter>(npcContext.Record));
                }
            }

            // filter by race
            if (Settings.Value.Race_Settings.destination_race_blacklist.Count > 0)
            {
                var toremove = new List<FormLink<INpcGetter>>();
                foreach (var npcfl in dest_m)
                {
                    var npc = npcfl.Resolve<INpcGetter>(state.LinkCache);
                    if (Settings.Value.Race_Settings.destination_race_blacklist.Contains(npc.Race))
                    {
                        toremove.Add(npcfl);
                        continue;
                    }
                    //if (!valid_races.Contains(npc.Race.Resolve(state.LinkCache)))
                    //{
                    //    dest_npcs.Remove(npc);
                    //}
                }
                foreach (var npcfl in dest_f)
                {
                    var npc = npcfl.Resolve<INpcGetter>(state.LinkCache);
                    if (Settings.Value.Race_Settings.destination_race_blacklist.Contains(npc.Race))
                    {
                        toremove.Add(npcfl);
                        continue;
                    }
                    //if (!valid_races.Contains(npc.Race.Resolve(state.LinkCache)))
                    //{
                    //    dest_npcs.Remove(npc);
                    //}
                }
                for (int i = 0; i < toremove.Count; i++)
                {
                    dest_m.Remove(toremove[i]);
                    dest_f.Remove(toremove[i]);
                }

            }


            // filter by factions
            if (Settings.Value.Npc_Settings.dest_fact_blacklist.Count > 0)
            {
                var toremove = new List<FormLink<INpcGetter>>();

                for (int i = 0; i < dest_m.Count; i++)
                {
                    var npc = dest_m[i].Resolve<INpcGetter>(state.LinkCache);

                    foreach (var fact in npc.Factions)
                    {
                        if (Settings.Value.Npc_Settings.dest_fact_blacklist.Contains(fact.Faction))
                        {
                            toremove.Add(dest_m[i]);
                            break;
                        }
                    }
                }
                for (int i=0;i<dest_f.Count;i++)
                {
                    var npc = dest_f[i].Resolve<INpcGetter>(state.LinkCache);

                    foreach (var fact in npc.Factions)
                    {
                        if (Settings.Value.Npc_Settings.dest_fact_blacklist.Contains(fact.Faction))
                        {
                            toremove.Add(dest_f[i]);
                            break;
                        }
                    }
                }
                for (int i = 0; i < toremove.Count;i++)
                {
                    dest_m.Remove(toremove[i]);
                    dest_f.Remove(toremove[i]);
                }
            }
            else if (Settings.Value.Npc_Settings.dest_fact_whitelist.Count > 0)
            {
                foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {
                    var npc = npcContext.Record;
                    bool found = false;
                    foreach (var fact in npc.Factions)
                    {
                        if (Settings.Value.Npc_Settings.dest_fact_whitelist.Contains(fact.Faction))
                        {
                            found = true;
                            break;
                        }
                    }
                    var fl = new FormLink<INpcGetter>(npc);
                    if (npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female))
                    {
                        if (found && !dest_f.Contains(fl)) dest_f.Add(fl);
                    }
                    else if (found && !dest_m.Contains(fl)) dest_m.Add(fl);
                }
            }

            //filter by npc lists
            if (Settings.Value.Npc_Settings.dest_npc_blacklist.Count > 0)
            {
                for(int i=0;i< Settings.Value.Npc_Settings.dest_npc_blacklist.Count;i++)
                {
                    var npc = Settings.Value.Npc_Settings.dest_npc_blacklist[i];
                    var fl = new FormLink<INpcGetter>(npc.FormKey);
                    dest_m.Remove(fl);
                    dest_f.Remove(fl);
                }
            }
            else if (Settings.Value.Npc_Settings.dest_npc_whitelist.Count > 0)
            {
                foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {
                    if (npcContext.Record.HeadParts == null || npcContext.Record.HeadParts.Count == 0)
                    {
                        Log($"Warning: Whitelisted npc {npcContext.Record.Name} {npcContext.Record.FormKey} is not able to be a face swap destination.");
                        continue;
                    }
                    var fl = new FormLink<INpcGetter>(npcContext.Record);
                    if (Settings.Value.Npc_Settings.dest_npc_whitelist.Contains(fl))
                    {
                        if (!npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female) && !dest_m.Contains(fl))
                            dest_m.Add(fl);
                        else if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female) && !dest_f.Contains(fl))
                            dest_f.Add(fl);
                    }
                }
            }



            // Source lists


            // filter by plugin
            if (Settings.Value.Plugin_Settings.source_plugins_whitelist.Count > 0)
            {
                foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {
                    //filter by incompatible NPCs (creatures, animals, anything without facegen)
                    if (Settings.Value.Plugin_Settings.source_plugins_whitelist.Contains(npcContext.ModKey))
                    {
                        if (npcContext.Record.HeadParts == null || npcContext.Record.HeadParts.Count == 0) continue;
                        if (npcContext.Record.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits)) continue;
                        if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.IsCharGenFacePreset)) continue;

                        if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female)) source_f.Add(new FormLink<INpcGetter>(npcContext.Record));
                        else source_m.Add(new FormLink<INpcGetter>(npcContext.Record));
                    }
                }
            }
            else
            {
                foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {
                    if (Settings.Value.Plugin_Settings.source_plugins_blacklist.Contains(npcContext.ModKey)) continue;
                    //filter by incompatible NPCs (creatures, animals, anything without facegen)
                    if (npcContext.Record.HeadParts == null || npcContext.Record.HeadParts.Count == 0) continue;
                    if (npcContext.Record.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits)) continue;
                    if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.IsCharGenFacePreset)) continue;

                    if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female)) source_f.Add(new FormLink<INpcGetter>(npcContext.Record));
                    else source_m.Add(new FormLink<INpcGetter>(npcContext.Record));
                }
            }

            // filter by race
            if (Settings.Value.Race_Settings.source_race_blacklist.Count > 0)
            {
                var toremove = new List<FormLink<INpcGetter>>();

                foreach (var npcfl in source_m)
                {
                    var npc = npcfl.Resolve<INpcGetter>(state.LinkCache);
                    if (Settings.Value.Race_Settings.source_race_blacklist.Contains(npc.Race))
                    {
                        toremove.Add(npcfl);
                        continue;
                    }
                    //if (!valid_races.Contains(npc.Race.Resolve(state.LinkCache)))
                    //{
                    //    source_npcs.Remove(npc);
                    //}
                }
                foreach (var npcfl in source_f)
                {
                    var npc = npcfl.Resolve<INpcGetter>(state.LinkCache);
                    if (Settings.Value.Race_Settings.source_race_blacklist.Contains(npc.Race))
                    {
                        toremove.Add(npcfl);
                        continue;
                    }
                    //if (!valid_races.Contains(npc.Race.Resolve(state.LinkCache)))
                    //{
                    //    source_npcs.Remove(npc);
                    //}
                }
                foreach (var npcfl in toremove)
                {
                    source_m.Remove(npcfl);
                    source_f.Remove(npcfl);
                }

            }

            //filter by npc lists

            if (Settings.Value.Npc_Settings.source_npc_blacklist.Count > 0)
            {
                foreach (var npcfl in Settings.Value.Npc_Settings.source_npc_blacklist)
                {
                    var fl = new FormLink<INpcGetter>(npcfl.FormKey);
                    source_m.Remove(fl);
                    source_f.Remove(fl);
                }
            }
            else if (Settings.Value.Npc_Settings.source_npc_whitelist.Count > 0)
            {
                foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {

                    if (npcContext.Record.HeadParts == null || npcContext.Record.HeadParts.Count == 0)
                    {
                        Log($"Warning: Whitelisted npc {npcContext.Record.Name} {npcContext.Record.FormKey} is not able to be a face swap source.");
                        continue;

                    }
                    var fl = new FormLink<INpcGetter>(npcContext.Record);
                    if (Settings.Value.Npc_Settings.source_npc_whitelist.Contains(fl))
                    {

                        if (!npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female) && !source_m.Contains(fl))
                            source_m.Add(fl);
                        else if (npcContext.Record.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female) && !source_f.Contains(fl))
                            source_f.Add(fl);
                    }
                }
            }
        }

        private static HashSet<NPCSwap> Matchmaker(
            List<FormLink<INpcGetter>> source_f,
            List<FormLink<INpcGetter>> source_m,
            List<FormLink<INpcGetter>> dest_f,
            List<FormLink<INpcGetter>> dest_m)
        {
            // create copies object to help with handling copies of the same npc (for example cicero)
            var copies = new Dictionary<string, List<List<FormKey>>>();

            foreach (var npc in state!.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                if (npc.Name == null) continue;
                if (npc.HeadParts == null || npc.TintLayers == null) continue;
                bool hasht = npc.HeadTexture.TryResolve<ITextureSetGetter>(state.LinkCache, out var ht);

                if (npc.HeadParts.Count == 0 && npc.TintLayers.Count < 4 && !hasht) continue;

                string name = npc.Name.ToString()!;

                if (copies.ContainsKey(name))
                {
                    var majorlist = copies[name];
                    bool found_match = false;

                    for (int i = 0; i < majorlist.Count; i++)
                    {
                        var checkednpc = state.LinkCache.Resolve<INpcGetter>(majorlist[i][0]);
                        if (checkednpc.HeadParts.Count != npc.HeadParts.Count)
                        {
                            // check other minorlists
                            continue;
                        }
                        bool headpart_mismatch = false;
                        foreach (var chp in checkednpc.HeadParts)
                        {
                            if (!npc.HeadParts.Contains(chp))
                            {
                                headpart_mismatch = true;
                                break;
                            }
                        }
                        if (headpart_mismatch) continue;
                        // here, the headparts match, the name is the same, so they belong to the same collection
                        copies[name][i].Add(npc.FormKey);
                        found_match = true;
                        break;
                    }
                    if (!found_match)
                    {
                        copies[name].Add(new List<FormKey> { npc.FormKey });
                    }
                }
                else
                {
                    copies[name] = new List<List<FormKey>> { new List<FormKey> { npc.FormKey } };
                }
            }

            //foreach (var namekey in copies.Keys)
            //{
            //    foreach(var minorlist in copies[namekey])
            //    {
            //        if (minorlist.Count > 1) Log($"{namekey} has a minorlist with {string.Join(",",minorlist.ToArray())}");
            //    }
            //}

            //populate the mapping starting with any predetermined NPC swap via settings
            var npc_map = new HashSet<NPCSwap>();

            if (Settings.Value.Npc_Swap_Settings.NPC_Swaps_Source_NPC_List.Count > 0)
            {
                for (int i = 0; i < Settings.Value.Npc_Swap_Settings.NPC_Swaps_Dest_NPC_List.Count; i++)
                {
                    var dnpcfl = Settings.Value.Npc_Swap_Settings.NPC_Swaps_Dest_NPC_List[i];
                    var snpcfl = Settings.Value.Npc_Swap_Settings.NPC_Swaps_Source_NPC_List[i];
                    var splug = Settings.Value.Npc_Swap_Settings.NPC_Swaps_Source_Plugin_List[i];

                    var dnpc = dnpcfl.Resolve<INpcGetter>(state.LinkCache);

                    // check name in copies
                    var dest_list = new List<FormKey> { };
                    var name = dnpc.Name!.ToString()!;
                    if (copies.ContainsKey(name))
                    {
                        var majorlist = copies[name];

                        for (int j = 0; j < majorlist.Count; j++)
                        {
                            var checkednpc = state.LinkCache.Resolve<INpcGetter>(majorlist[j][0]);
                            if (checkednpc.HeadParts.Count != dnpc.HeadParts.Count)
                            {
                                // check other minorlists
                                continue;
                            }
                            bool headpart_mismatch = false;
                            foreach (var chp in checkednpc.HeadParts)
                            {
                                if (!dnpc.HeadParts.Contains(chp))
                                {
                                    headpart_mismatch = true;
                                    break;
                                }
                            }
                            if (headpart_mismatch) continue;
                            // here, the headparts match, the name is the same, so they belong to the same collection
                            dest_list.AddRange(majorlist[j]);
                            break;
                        }
                    }
                    else dest_list.Add(dnpc.FormKey);

                    var dg = dnpc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female);
                    var sg = snpcfl.Resolve<INpcGetter>(state.LinkCache).Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female);

                    bool contains = Settings.Value.Futa_Settings.Futa_Choice_NPC.Contains(dnpcfl);
                    if ((!dg && sg) && (contains || Settings.Value.Futa_Settings.Set_Former_Males_as_Futa))
                    {
                        int index = 0;
                        ModKey sos_plug = new("Skyrim", ModType.Master);
                        sbyte sos_size = -1;

                        if (contains)
                        {
                            index = Settings.Value.Futa_Settings.Futa_Choice_NPC.IndexOf(dnpcfl);

                            sos_plug = Settings.Value.Futa_Settings.Futa_Choice_Addon[index];
                            sos_size = (sbyte)Settings.Value.Futa_Settings.Futa_Choice_Size[index];
                        }

                        if (sos_size == -1)
                        {
                            if (Settings.Value.Futa_Settings.Futa_Size_randomization == 0)
                            {
                                sos_size = (sbyte)RandomNormal(Settings.Value.Futa_Settings.Futa_Size_Min, Settings.Value.Futa_Settings.Futa_Size_Max);
                            }
                            else
                            {
                                sos_size = (sbyte)rnd.Next(Settings.Value.Futa_Settings.Futa_Size_Min, Settings.Value.Futa_Settings.Futa_Size_Max + 1);
                            }
                        }
                        if (sos_plug == null || sos_plug.IsNull || sos_plug.Name == "Skyrim")
                        {
                            sos_plug = Settings.Value.Futa_Settings.Futa_Addon_Plugins[rnd.Next(Settings.Value.Futa_Settings.Futa_Addon_Plugins.Count)];
                        }


                        foreach (var cdnpcfk in dest_list)
                        {
                            var fl = new FormLink<INpcGetter>(cdnpcfk);
                            npc_map.Add(new NPCSwap(fl, snpcfl, splug, true, sos_plug, sos_size));
                        }
                    }
                    else
                    {
                        foreach (var cdnpcfk in dest_list)
                        {
                            var fl = new FormLink<INpcGetter>(cdnpcfk);
                            npc_map.Add(new NPCSwap(dnpcfl, snpcfl, splug));
                        }
                    }
                }
            }

            // now assign the randomized npcs from the winning overrides
            // assign all male, assign all female, swap all genders, random gender assignment, random swaps within gender



            if (Settings.Value.Gender_Settings.Make_All_Male)
            {
                // we use the source_m and dest_f
                Create_Swaps(ref npc_map, copies, source_m, dest_f);

                if (Settings.Value.Gender_Settings.Randomize_Within_Genders)
                {
                    // use source_m and dest_m
                    Create_Swaps(ref npc_map, copies, source_m, dest_m);
                }
            }
            else if (Settings.Value.Gender_Settings.Make_All_Female)
            {
                // we use the source_f and dest_m
                Create_Swaps(ref npc_map, copies, source_f, dest_m);
                
                if (Settings.Value.Gender_Settings.Randomize_Within_Genders)
                {
                    // use source_f and dest_f
                    Create_Swaps(ref npc_map, copies, source_f, dest_f);
                    
                }
            }
            else if (Settings.Value.Gender_Settings.Randomize_Genders)
            {
                var dest_both = new List<FormLink<INpcGetter>>();
                dest_both.AddRange(dest_m);
                dest_both.AddRange(dest_f);
                var source_both = new List<FormLink<INpcGetter>>();
                source_both.AddRange(source_m);
                source_both.AddRange(source_f);

                Create_Swaps(ref npc_map, copies, source_both, dest_both);

            }
            else if (Settings.Value.Gender_Settings.Reverse_Genders)
            {
                // we use source_f to dest_m AND source_m to dest_f
                Create_Swaps(ref npc_map, copies, source_f, dest_m);
                Create_Swaps(ref npc_map, copies, source_m, dest_f);

            }
            else if (Settings.Value.Gender_Settings.Randomize_Within_Genders)
            {

                // we use the source_m and dest_f
                Create_Swaps(ref npc_map, copies, source_f, dest_f);
                Create_Swaps(ref npc_map, copies, source_m, dest_m);
            }

            //var dt = DateTime.Now;

            //using (StreamWriter outputFile = File.AppendText(state.ExtraSettingsDataPath+@$"\swaps_{dt.ToString("yyyy-MM-dd_HH:mm:ss")}.txt"))
            //{
            //    outputFile.WriteLine(sw.ToString());
            //}

            return npc_map;
        }

        private static void Create_Swaps(ref HashSet<NPCSwap> map,
            Dictionary<string, List<List<FormKey>>> copies,
            List<FormLink<INpcGetter>> source_list, 
            List<FormLink<INpcGetter>> dest_list)
        {
            //Log("Start create swap");
            var source_dict = new Dictionary<string, List<FormLink<INpcGetter>>>();
            var inc_dict = new Dictionary<string, int>();

            //Log("init inc dict and source dict");
            foreach (var kvp in race_groups)
            {
                inc_dict[kvp.Key] = 0;
                source_dict[kvp.Key] = new List<FormLink<INpcGetter>>();
            }
            //Log("randomize");
            var rs = new RandomSource(Settings.Value.Random_Generation_Seed);
            source_list.Randomize(rs);
            dest_list.Randomize(rs);
            int inc = 0;

            //Log("loop source list to populate source dict");
            foreach (var snpcfl in source_list)
            {
                var snpc = snpcfl.Resolve<INpcGetter>(state!.LinkCache);
                bool found = false;
                foreach (var kvp in race_groups)
                {
                    if (kvp.Value.Contains(snpc.Race.FormKey))
                    {
                        source_dict[kvp.Key].Add(snpcfl);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Log($"{snpc.FormKey} is a {snpc.Race} could not find entry in race groups");
                }
            }

            //Log("loop dest list");
            foreach (var dnpcfl in dest_list)
            {

                //Log("resolve dest npc");
                var dnpc = dnpcfl.Resolve<INpcGetter>(state!.LinkCache);

                // check name in copies
                //Log("Check name");
                var dest_copies = new List<FormKey> { };
                var name = "UnknownNPC";
                if (dnpc.Name != null && !dnpc.Name.String.IsNullOrEmpty()) name = dnpc.Name.String;

                if (copies.ContainsKey(name))
                {
                    var majorlist = copies[name];

                    for (int j = 0; j < majorlist.Count; j++)
                    {
                        var checkednpc = state.LinkCache.Resolve<INpcGetter>(majorlist[j][0]);
                        if (checkednpc.HeadParts.Count != dnpc.HeadParts.Count)
                        {
                            // check other minorlists
                            continue;
                        }
                        bool headpart_mismatch = false;
                        foreach (var chp in checkednpc.HeadParts)
                        {
                            if (!dnpc.HeadParts.Contains(chp))
                            {
                                headpart_mismatch = true;
                                break;
                            }
                        }
                        if (headpart_mismatch) continue;
                        // here, the headparts match, the name is the same, so they belong to the same collection
                        dest_copies.AddRange(majorlist[j]);
                        break;
                    }
                }
                else dest_copies.Add(dnpc.FormKey);

                //locate race appropriate swaps
                //Log("begin locating correct race group");
                var foxrace = FormKey.Factory("109C7C:Skyrim.esm");

                // find a source that has correct race
                var dnpc0 = state.LinkCache.Resolve<INpcGetter>(dest_copies[0]);
                var dracefk = dnpc0.Race.FormKey;
                string racename = "Other";
                if (dracefk.Equals(foxrace))
                {
                    Log($"Foxrace for {dnpc0.FormKey}");
                    continue;
                }
                foreach (var kvp in race_groups)
                {
                    if (kvp.Value.Contains(dracefk))
                    {
                        racename = kvp.Key;
                        break;
                    }
                }
                //Log($"npc {dnpc0.FormKey} racename was {racename}");
                FormLink<INpcGetter> dsnpcfl;
                if (racename == "Other")
                {
                    //Log($"npc {dnpc0} is being treated as Other");
                }

                
                //Log("Check futa");
                if (source_dict[racename].Count == 0)
                {
                    // choose a source from the main list
                    dsnpcfl = source_list[inc];
                    inc++;
                }
                else
                {
                    dsnpcfl = source_dict[racename][inc_dict[racename] % source_dict[racename].Count];
                    inc_dict[racename]++;
                }

                var dg = dnpc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female);
                var sg = dsnpcfl.Resolve<INpcGetter>(state.LinkCache).Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female);

                bool contains = Settings.Value.Futa_Settings.Futa_Choice_NPC.Contains(dnpcfl);
                if (!dg && sg && (contains || Settings.Value.Futa_Settings.Set_Former_Males_as_Futa))
                {
                    int index = 0;
                    ModKey sos_plug = new("Skyrim", ModType.Master);
                    sbyte sos_size = -1;

                    if (Settings.Value.Futa_Settings.Futa_Choice_NPC.Contains(dnpcfl))
                    {
                        index = Settings.Value.Futa_Settings.Futa_Choice_NPC.IndexOf(dnpcfl);

                        sos_plug = Settings.Value.Futa_Settings.Futa_Choice_Addon[index];
                        sos_size = (sbyte)Settings.Value.Futa_Settings.Futa_Choice_Size[index];
                    }


                    if (sos_size == -1)
                    {
                        if (Settings.Value.Futa_Settings.Futa_Size_randomization == 0)
                        {
                            sos_size = (sbyte)RandomNormal(Settings.Value.Futa_Settings.Futa_Size_Min, Settings.Value.Futa_Settings.Futa_Size_Max);
                        }
                        else
                        {
                            sos_size = (sbyte)rnd.Next(Settings.Value.Futa_Settings.Futa_Size_Min, Settings.Value.Futa_Settings.Futa_Size_Max + 1);
                        }
                    }
                    if (sos_plug.Name == "Skyrim")
                    {
                        sos_plug = Settings.Value.Futa_Settings.Futa_Addon_Plugins[rnd.Next(Settings.Value.Futa_Settings.Futa_Addon_Plugins.Count)];
                    }
                    //Log("create futa swap");
                    foreach (var cdnpcfk in dest_copies)
                    {
                        var fl = new FormLink<INpcGetter>(cdnpcfk);
                        if (map.Contains(new NPCSwap(fl, dsnpcfl, null))) continue;
                        map.Add(new NPCSwap(fl, dsnpcfl, null, true, sos_plug, sos_size));
                    }
                }
                else
                {
                    //Log("create normal swap");
                    foreach (var cdnpcfk in dest_copies)
                    {
                        var fl = new FormLink<INpcGetter>(cdnpcfk);
                        if (map.Contains(new NPCSwap(fl, dsnpcfl, null))) continue;
                        map.Add(new NPCSwap(fl, dsnpcfl, null));
                    }
                }
            }
        }

        private static ConcurrentDictionary<Npc, string> Swap_NPCs(HashSet<NPCSwap> npc_map)
        {

            // a quick map of sos addon modkeys to the faction form responsible for SOS placing schlong on npc
            var futa_addon_factions = new Dictionary<ModKey, IFactionGetter>();
            foreach (var addon in Settings.Value.Futa_Settings.Futa_Addon_Plugins)
            {

                var lo = new LoadOrder<IModListing<ISkyrimModGetter>>();
                if (state!.LoadOrder.TryGetValue<IModListing<ISkyrimModGetter>, ModKey>(addon, out var futa_listing) && futa_listing != null)
                {
                    var futa_factions = futa_listing.Mod!.Factions.Records.ToList();

                    if (futa_factions.Count > 1)
                    {
                        Log($"Addon {addon} has more than one faction");
                    }
                    foreach (var ff in futa_factions)
                    {
                        futa_addon_factions.Add(addon, ff);
                        break;
                    }
                }
            }

            // keyword from sexlab useful for marking futa as sexlab males
            IKeywordGetter? sl_treat_as_male_kw = null;
            if (ModKey.TryFromFileName("SexLab.esm", out var SLmk))
            {
                if (state!.LoadOrder.TryGetValue(SLmk, out var SL))
                {
                    add_sexlab_kw = true;
                    foreach (var kw in SL.Mod!.Keywords.Records)
                    {
                        if (kw.EditorID!.Contains("TreatMale"))
                        {
                            sl_treat_as_male_kw = kw;
                            break;
                        }
                    }
                }
            }
            if (Directory.Exists(modsfolder + @"\NPC Face Swap FaceGen\meshes")) Directory.Delete(modsfolder + @"\NPC Face Swap FaceGen\meshes", true);
            if (Directory.Exists(modsfolder + @"\NPC Face Swap FaceGen\textures")) Directory.Delete(modsfolder + @"\NPC Face Swap FaceGen\textures", true);


            var modlist_lines = new List<string>();
            using (var sr = new StreamReader(Settings.Value.MO2_profile_folder + @"\modlist.txt"))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    modlist_lines.Add(line.Replace("\r", "").Replace("\n", ""));
                }
            }

            var changed_voices = new ConcurrentDictionary<Npc,string>();
            Log($"There are {npc_map.Count} swaps to do");
            int swap_count = 0;

            //foreach (var swap in npc_map)
            var options = new ParallelOptions()
            {
                //You can hard code the value as follows
                //MaxDegreeOfParallelism = 4
                //But better to use the following statement
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2 - 1
            };
            var swap_logs = new List<string>();

            Stopwatch sw = new Stopwatch();


            log_file_output = false;
            sw.Start();
            Parallel.ForEach<NPCSwap>(npc_map,options, swap =>
            //foreach (var swap in npc_map)
            {

                var logs = new List<string>();
                var dnpc = swap.Dest_npc.Resolve<INpcGetter>(state!.LinkCache);
                var tempNpc = dnpc.DeepCopy();
                Log($"{swap_count}/{npc_map.Count} Dest npc {tempNpc.EditorID} {tempNpc.FormKey.ModKey} {tempNpc.FormKey}");

                Interlocked.Increment(ref swap_count);

                //var snpc = state.LinkCache.Resolve<INpcGetter>(kvp.Value.Item1);
                var source_plugin = new ModKey();
                if (swap.Source_plugin != null) source_plugin = (ModKey)swap.Source_plugin;


                var desired_context = swap.Source_npc.ResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(state.LinkCache);

                if (desired_context == null)
                {
                    logs.Add($"Bad npc? {swap.Source_npc.FormKey}");
                    //continue;
                    return;
                }

                // for the case where the matchmaker selected the source npc. no source plugin is selected.
                // to avoid searching for facegen in any kind of patch mod, use the earliest plugin with headparts matching the last plugin.
                // that should be the npc plugin with the facegen
                if (source_plugin == null || source_plugin.IsNull)
                {
                    var npc = swap.Source_npc.Resolve<INpcGetter>(state.LinkCache);
                    var hps = npc.HeadParts;

                    foreach (var context in swap.Source_npc.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(state.LinkCache))
                    {
                        bool has_same_hp = true;
                        if (hps.Count != context.Record.HeadParts.Count) continue;

                        foreach (var hp in context.Record.HeadParts)
                        {
                            if (!hps.Contains(hp))
                            {
                                has_same_hp = false;
                                break;
                            }
                        }
                        if (has_same_hp)
                        {
                            desired_context = context;
                        }
                    }
                    //now earliest_context has the context of the earliest mod which has the desired headparts
                    //which is 99.9999% likely the correct record we want

                }
                else
                {
                    bool was_found = false;
                    foreach (var context in swap.Source_npc.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(state.LinkCache))
                    {
                        //find the record in the plugin we want
                        if (!context.ModKey.Equals(source_plugin)) continue;
                        was_found = true;
                        desired_context = context;
                        break;
                    }
                    if (!was_found)
                    {
                        logs.Add($"NPC {swap.Source_npc.FormKey} is not in plugin {source_plugin}. Skipping");
                        //continue;
                        return;
                    }
                }

                logs.Add($"The source EditorID is {desired_context.Record.EditorID} in mod {desired_context.ModKey} {desired_context.Record.FormKey}");
                logs.Add($"Dest npc {tempNpc.EditorID} {tempNpc.FormKey.ModKey} {tempNpc.FormKey} swap from source {desired_context.Record.EditorID} in mod {desired_context.ModKey} {desired_context.Record.FormKey}");
                //copy the records
                //start with the last winning override of the dest npc
                // head parts, hair color, head texture, texture lighting, face morph, face parts, tint layers, voice
                // copy from source npc to dest npc

                var snpc = desired_context.Record.DeepCopy();

                tempNpc.HeadParts.RemoveAll(a => true);
                foreach (var hp in snpc.HeadParts) tempNpc.HeadParts.Add(hp);

                tempNpc.TintLayers.RemoveAll(a => true);
                foreach (var tl in snpc.TintLayers) tempNpc.TintLayers.Add(tl);

                tempNpc.HairColor = snpc.HairColor;
                tempNpc.HeadTexture = snpc.HeadTexture;
                tempNpc.TextureLighting = snpc.TextureLighting;
                tempNpc.FaceMorph = snpc.FaceMorph;
                tempNpc.FaceParts = snpc.FaceParts;
                tempNpc.Race = snpc.Race;


                if (!Settings.Value.Voice_Settings.Use_Original_Voice)
                {

                    var source_voice_fl = snpc.Voice;
                    bool source_voice_valid = source_voice_fl.TryResolve<IVoiceTypeGetter>(state.LinkCache, out var source_voice);

                    string svt = "";

                    if (Settings.Value.Voice_Settings.Use_Random_Voice ||
                            !source_voice_valid ||
                            source_voice == null ||
                            !vanilla_skyrim.Contains(source_voice.FormKey.ModKey.ToString(), StringComparer.OrdinalIgnoreCase))
                    {
                        string v = "";
                        if (snpc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female))
                        {
                            v = xvasynth_model_names_female.Keys.ToList<string>()[rnd.Next(0, xvasynth_model_names_female.Count)];
                        }
                        else
                        {
                            v = xvasynth_model_names_male.Keys.ToList<string>()[rnd.Next(0, xvasynth_model_names_male.Count)];
                        }
                        source_voice = state.LinkCache.Resolve<IVoiceTypeGetter>(v);
                        source_voice_fl = new FormLinkNullable<IVoiceTypeGetter>(source_voice.FormKey);

                    }

                    if (snpc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female))
                    {
                        svt = xvasynth_model_names_female[source_voice.EditorID!.ToString()];
                    }
                    else
                    {
                        svt = xvasynth_model_names_male[source_voice.EditorID!.ToString()];
                    }

                    if (!tempNpc.Voice.Equals(source_voice_fl))
                    {
                        bool dest_voice_valid = tempNpc.Voice.TryResolve<IVoiceTypeGetter>(state.LinkCache, out var vt);

                        if (tempNpc.Voice == null || !dest_voice_valid || vt == null)
                        {
                            logs.Add("\tVoice of dest npc is invalid, voice not needed");
                        }
                        else
                        {
                            changed_voices[tempNpc] = svt;

                            //logs.Add("check unique npc voice");
                            if (vt!.EditorID!.Contains("unique", StringComparison.OrdinalIgnoreCase) &&
                                Settings.Value.Voice_Settings.Generate_Voice_CSV)
                            {
                                logs.Add("\tVoice has word unique, leaving record in place");
                            }
                            //logs.Add("check named npc voice");
                            else if (tempNpc.Name != null &&
                                tempNpc.Name.String != null &&
                                vt.EditorID!.Contains(tempNpc.Name!.ToString().Split(" ")[0], StringComparison.OrdinalIgnoreCase) &&
                                Settings.Value.Voice_Settings.Generate_Voice_CSV)
                            {
                                logs.Add("\tVoice type contains npc name, leaving record in place");
                            }
                            else
                            {
                                tempNpc.Voice = source_voice_fl;

                            }
                        }
                    }
                }

                if (swap.Futa)
                {
                    var addon = swap.Sos_plugin;
                    var ff = futa_addon_factions[addon];

                    var rp = new RankPlacement
                    {
                        Faction = new FormLink<IFactionGetter>(ff.FormKey),
                        Rank = swap.Sos_size
                    };

                    tempNpc.Factions.Add(rp);

                    // sexlab treat as male
                    if (add_sexlab_kw)
                    {
                        if (tempNpc.Keywords == null) tempNpc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();

                        tempNpc.Keywords.Add(new FormLink<IKeywordGetter>(sl_treat_as_male_kw!));
                    }

                    tempNpc.Configuration.Flags = tempNpc.Configuration.Flags | NpcConfiguration.Flag.Female;

                    //this does not work
                    //tempNpc.Configuration.Flags.SetFlag(NpcConfiguration.Flag.Female, true);

                }
                else
                {
                    tempNpc.Configuration.Flags.SetFlag(NpcConfiguration.Flag.Female, false);
                }

                //bijin has worn body armor...
                tempNpc.WornArmor = snpc.WornArmor;

                lock (state)
                {
                    state!.PatchMod.Npcs.GetOrAddAsOverride(tempNpc);
                }

                //move facegen
                string path_hm_dest = @"\meshes\actors\character\facegendata\facegeom\" + tempNpc.FormKey.ModKey.FileName;
                string path_ft_dest = @"\textures\actors\character\facegendata\facetint\" + tempNpc.FormKey.ModKey.FileName;
                string path_hm_source = @"\meshes\actors\character\facegendata\facegeom\" + snpc.FormKey.ModKey.FileName;
                string path_ft_source = @"\textures\actors\character\facegendata\facetint\" + snpc.FormKey.ModKey.FileName;

                if (!Directory.Exists(modsfolder + @"\NPC Face Swap FaceGen" + path_hm_dest)) Directory.CreateDirectory(modsfolder + @"\NPC Face Swap FaceGen" + path_hm_dest);
                if (!Directory.Exists(modsfolder + @"\NPC Face Swap FaceGen" + path_ft_dest)) Directory.CreateDirectory(modsfolder + @"\NPC Face Swap FaceGen" + path_ft_dest);

                var valid_dirs=new List<Tuple<string,int>>();
                bool found_plugin = false;
                foreach (var d in new DirectoryInfo(modsfolder).EnumerateDirectories())
                {
                    //logs.Add(d.Name);
                    foreach (var f in d.EnumerateFiles())
                    {
                        //logs.Add($"\t{f.Name}");
                        if (f.Name == desired_context.ModKey.FileName)
                        {
                            //logs.Add($"\tsearching for mod folder. esp {desired_context.ModKey.FileName} found? {f.Name} in {d.Name}");

                            int i = 0;
                            foreach (var line in modlist_lines)
                            {
                                //logs.Add(mline);
                                if (line == "+"+ d.Name)
                                {

                                    found_plugin = true;
                                    valid_dirs.Add(new Tuple<string,int>(d.Name, i));
                                    
                                    break;
                                }
                                i++;
                            }
                        }

                    }
                }
                if (!found_plugin)
                {
                    logs.Add($"unable to find dir for plugin {desired_context.ModKey.FileName}");
                    //continue;
                    return;
                }

                valid_dirs.Sort(delegate (Tuple<string, int> a, Tuple<string, int> b)
                {
                    if (a.Item2 > b.Item2) return 1;
                    if (a.Item2 < b.Item2) return -1;
                    return 0;
                });


                bool found_facegen = false;
                foreach (var t in valid_dirs)
                {
                    string dir = t.Item1;

                    var dest_id = tempNpc.FormKey.ID.ToString("X");
                    while (dest_id.Length < 8) dest_id = "0" + dest_id;
                    var source_id = snpc.FormKey.ID.ToString("X");
                    while (source_id.Length < 8) source_id = "0" + source_id;

                    string source_head_mesh = modsfolder + @"\" + dir + path_hm_source + @"\" + source_id + ".nif";
                    string dest_head_mesh = modsfolder + @"\NPC Face Swap FaceGen" + path_hm_dest + @"\" + dest_id + ".nif";

                    string source_face_tint = modsfolder + @"\" + dir + path_ft_source + @"\" + source_id + ".dds";
                    string dest_face_tint = modsfolder + @"\NPC Face Swap FaceGen" + path_ft_dest + @"\" + dest_id + ".dds";

                    if (!File.Exists(source_head_mesh)) continue;
                    File.Copy(source_head_mesh, dest_head_mesh);

                    try
                    {
                        logs.Add($"\tCopying {source_face_tint} to {dest_face_tint}");
                        File.Copy(source_face_tint, dest_face_tint);
                    }
                    catch
                    {
                        logs.Add("Was not able to copy texture. perhaps does not need texture?");
                    }
                    found_facegen = true;
                    break;
                }
                if (!found_facegen)
                {
                    logs.Add($"Fail Dest npc {tempNpc.EditorID} {tempNpc.FormKey.ModKey} {tempNpc.FormKey} swap from source {desired_context.Record.EditorID} in mod {desired_context.ModKey} {desired_context.Record.FormKey} is complete.");

                    throw new Exception("Missing facegen");

                }


                logs.Add($"Dest npc {tempNpc.EditorID} {tempNpc.FormKey.ModKey} {tempNpc.FormKey} swap from source {desired_context.Record.EditorID} in mod {desired_context.ModKey} {desired_context.Record.FormKey} is complete.");

                lock (swap_logs)
                {
                    swap_logs.AddRange(logs);
                }

            }
            );

            log_file_output = true;
            sw.Stop();
            Log($"Finished npc swaps {sw.ElapsedMilliseconds}");

            // for all npcs
            //no parallelism
            //Finished npc swaps 826856

            //mdp 4
            // Finished npc swaps 688298

            // mdp 7
            //Finished npc swaps 709459

            // mdp 63
            //Finished npc swaps 724407

            Log(swap_logs,false);


            return changed_voices;
        }

        private static void Generate_xVASynth_CSV(ConcurrentDictionary<Npc, string> changed_voices)
        {
            // somehow find all dialog and voice files associated with that voice type record


            var voice_lines = new ConcurrentDictionary<VoiceLine,int>();

            var logs = new List<string>();


            int i = 0;

            var topic_fls = new List<FormLink<IDialogTopicGetter>>();

            foreach (var topic in state!.LoadOrder.PriorityOrder.DialogTopic().WinningOverrides())
            {
                topic_fls.Add(new FormLink<IDialogTopicGetter>(topic.FormKey));
            }

            log_file_output = false;

            var options = new ParallelOptions()
            {
                //You can hard code the value as follows
                //MaxDegreeOfParallelism = 7
                //But better to use the following statement
                MaxDegreeOfParallelism = Environment.ProcessorCount*2 - 1
            };
            Stopwatch sw = new();

            sw.Start();
            //foreach(var topic_fl in topic_fls)
            Parallel.ForEach<FormLink<IDialogTopicGetter>>(topic_fls,options, topic_fl =>
            {
                Interlocked.Increment(ref i);
                if (i % 100 == 0)
                {
                    Log($"check topic {topic_fl.FormKey} : {i}/{topic_fls.Count}");
                }
                //logs.Add($"check topic {topic_fl.FormKey} : {i}/{topic_fls.Count}");
                //Log($"\tcheck topic {topic.FormKey}");
                foreach (var topic_context in topic_fl.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(state!.LinkCache))
                {

                    var topic = topic_context.Record;



                    var checked_rba = new HashSet<FormKey>();

                    foreach (var rba in topic.Responses)
                    {
                        if (checked_rba.Contains(rba.FormKey)) continue;
                        checked_rba.Add(rba.FormKey);

                        //logs.Add($"\trba {rba.EditorID} {rba.FormKey}");
                        //Log($"\t\trba {rba.EditorID} {rba.FormKey}");


                        // func, thing's formkey, compare op, or flag ("or" if true, "and" if false)
                        var conditions = new List<Tuple<string, FormKey, string, bool>>();
                        var alias_cond = new List<Tuple<string, FormKey, string, bool>>();

                        foreach (var cond in rba.Conditions)
                        {
                            //logs.Add($"\t\tcondition");

                            var func = cond.Data.Function.ToString();
                            //logs.Add($"\t\t\tfunction {func}");

                            switch (func)
                            {
                                case "GetIsAliasRef":

                                    var qfk = topic.Quest.FormKey;
                                    var qfl = new FormLink<IQuestGetter>(qfk);
                                    if (!qfl.TryResolve(state.LinkCache, out var quest))
                                    {
                                        //logs.Add($"\t\t\tunable to resolve quest");
                                        //logs.Add($"\t\t\tquest: {topic.Quest.FormKey}");
                                        break;
                                    }


                                    var ind = ((GetIsAliasRefConditionData)cond.Data).ReferenceAliasIndex;

                                    var alias = quest.Aliases[0];
                                    var fr = alias.ForcedReference;
                                    foreach (var a in quest.Aliases)
                                    {
                                        alias = a;
                                        if (alias.ID == ind)
                                        {
                                            //logs.Add($"\t\t\tName {alias.Name}");
                                            fr = alias.ForcedReference;
                                            break;
                                        }
                                    }

                                    if (alias.Factions != null && alias.Factions.Count > 0)
                                    {
                                        //logs.Add($"\t\t\talias factions non zero");
                                        foreach (var f in alias.Factions)
                                        {
                                            alias_cond.Add(new Tuple<string, FormKey, string, bool>("GetInFaction", f.FormKey, "EqualTo", false));
                                        }
                                    }

                                    if (fr.TryResolve<IPlacedNpcGetter>(state.LinkCache, out var placed_npc))
                                    {
                                        //logs.Add($"\t\t\tnpc placer non null");

                                        alias_cond.Add(new Tuple<string, FormKey, string, bool>("GetIsID", placed_npc.Base.FormKey, cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));
                                    }
                                    if (alias.UniqueActor != null && !alias.UniqueActor.IsNull)
                                    {
                                        //logs.Add($"\t\t\tunique actor non null");
                                        alias_cond.Add(new Tuple<string, FormKey, string, bool>("GetIsID", alias.UniqueActor.FormKey, cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));

                                    }

                                    if (alias.Conditions != null && alias.Conditions.Count > 0)
                                    {
                                        //logs.Add($"\t\t\talias conditions non zero");
                                        foreach (var ac in alias.Conditions)
                                        {
                                            if (ac.Data.GetType() == typeof(GetIsIDConditionData))
                                            {
                                                var acc = (GetIsIDConditionData)ac.Data;
                                                alias_cond.Add(new Tuple<string, FormKey, string, bool>("GetIsID", acc.Object.Link.FormKey, cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));

                                            }
                                            if (ac.Data.GetType() == typeof(GetInFactionConditionData))
                                            {
                                                var acc = (GetInFactionConditionData)ac.Data;
                                                alias_cond.Add(new Tuple<string, FormKey, string, bool>("GetInFaction", acc.Faction.Link.FormKey, cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));
                                            }

                                        }
                                    }
                                    conditions.Add(new Tuple<string, FormKey, string, bool>(func, new FormKey(), cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));

                                    break;

                                case "GetIsID":
                                    var data_id = (GetIsIDConditionData)cond.Data;

                                    //logs.Add($"\t\t\tcd_data ref {data_id.Object.Link}");
                                    conditions.Add(new Tuple<string, FormKey, string, bool>(func, data_id.Object.Link.FormKey, cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));
                                    break;
                                case "GetInFaction":
                                    var data_fact = (GetInFactionConditionData)cond.Data;
                                    //logs.Add($"\t\t\tcd_data ref {data_fact.Faction.Link}");
                                    conditions.Add(new Tuple<string, FormKey, string, bool>(func, data_fact.Faction.Link.FormKey, cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));
                                    break;
                                case "GetIsVoiceType":
                                    var voice_data = (GetIsVoiceTypeConditionData)cond.Data;
                                    var v = voice_data.VoiceTypeOrList.Link;
                                    //logs.Add($"\t\t\tcd_data ref {v.FormKey}");
                                    conditions.Add(new Tuple<string, FormKey, string, bool>(func, v.FormKey, cond.CompareOperator.ToString(), cond.Flags.HasFlag(Condition.Flag.OR)));
                                    break;
                                default:
                                    break;
                            }


                        }// foreach cond in rba.Cond


                        foreach (var npckv in changed_voices)
                        {
                            var npc = npckv.Key;
                            //logs.Add($"\t\tCheck {npc.EditorID}");

                            if (Evaluate_Conditions(conditions, npc, alias_cond, ref logs))
                            {
                                //logs.Add($"\t\tConditions are met");
                                var qfk = topic.Quest.FormKey;
                                var qfl = new FormLink<IQuestGetter>(qfk);
                                if (!qfl.TryResolve(state.LinkCache, out var quest))
                                {
                                    //logs.Add($"\t\tunable to resolve quest");
                                    //logs.Add($"\t\tquest: {topic.Quest.FormKey}");
                                    continue;
                                }

                                //logs.Add($"\t\tquest {quest}");
                                //logs.Add($"\t\tquest {quest.EditorID!}");

                                foreach (var resp_line in rba.Responses)
                                {
                                    var line = resp_line.Text.String;
                                    var resp_number = resp_line.ResponseNumber;
                                    if (line != null)
                                    {
                                        string file_name = "";
                                        string fid = rba.FormKey.ID.ToString("X");
                                        while (fid.Length < 8) fid = "0" + fid;
                                        if (topic.EditorID == null)
                                        {
                                            file_name = TruncateLongString(quest.EditorID!, 25) +
                                                "__" + fid
                                                + "_" + resp_number.ToString();
                                        }
                                        else
                                        {
                                            string qid = "";
                                            string tid = "";
                                            if (quest.EditorID!.Length < 10)
                                            {
                                                qid = quest.EditorID!;
                                                tid = TruncateLongString(topic.EditorID, 25 - qid.Length);
                                            }
                                            else
                                            {
                                                qid = TruncateLongString(quest.EditorID!, 10);
                                                tid = TruncateLongString(topic.EditorID, 15);
                                            }

                                            file_name = qid + "_" + tid + "_" + fid + "_" + resp_number.ToString();
                                        }

                                        //logs.Add($"\t\tfile name {file_name}");
                                        //line, emotion, emotion value, file name
                                        var voice = npc.Voice.Resolve<IVoiceTypeGetter>(state.LinkCache);

                                        //logs.Add($"\t\tformkey {rba.FormKey}");
                                        voice_lines[new VoiceLine(
                                            line,
                                            resp_line.Emotion.ToString(),
                                            resp_line.EmotionValue,
                                            file_name,
                                            rba.FormKey.ToString().Split(":")[1],
                                            voice.EditorID!,
                                            npckv.Value,
                                            rba.FormKey)]=1;
                                        //logs.Add($"\t\tAdding line {resp_line.Emotion} {resp_line.EmotionValue} {line}");
                                    }
                                }
                                //logs.Add($"\t\tLines for this response gathered successfully");
                            }
                            else
                            {
                                //logs.Add("\t\tConditions not met");
                            }
                        } //foreach npc in changed voices

                    } // foreach rba

                } //foreach topic
            }// foreach topicfl
            );
            sw.Stop();

            // for 6 npcs
            // no parallelism
            // Finished checking NPC dialogs 7404
            // ????????

            // MaxDegreeOfParallelism = 63
            // Finished checking NPC dialogs 132197

            // mdp 7
            //Finished checking NPC dialogs 142324

            // for all npc
            // no parallelism
            //Finished checking NPC dialogs 2061679

            //mdp 7
            // Finished checking NPC dialogs 685712
            // Finished checking NPC dialogs 691321

            log_file_output = true;
            Log($"Finished checking NPC dialogs {sw.ElapsedMilliseconds}");


            using (StreamWriter outputFile = File.AppendText(log_path))
            {
                foreach (var line in logs)
                {
                    outputFile.WriteLine(line);
                }
            }

            // hopefully able to run xvasynth via cli here, if not then generate the csv needed for xvasynth gui
            // it does not, now to construct csv


            var csv_lines = new List<string>();
            string[] column = { "game_id", "voice_id", "text", "vocoder", "out_path" };

            csv_lines.Add(string.Join("|", column));

            var vanilla_voice_list = new List<string>();
            var game_folder = state!.DataFolderPath;
            var reader = Archive.CreateReader(GameRelease.SkyrimSE, Path.Combine(game_folder, "Skyrim - Voices_en0.bsa"));
            foreach (var file in reader.Files) vanilla_voice_list.Add(string.Join(".", file.Path.Split(".").Take(2).ToArray()));

            // sound/voice/plugin/voicetype/filename.xxx

            //using(StreamWriter outputFile = new StreamWriter(Path.Combine(state.ExtraSettingsDataPath!, "vl.txt")))
            //{
            //    foreach (string line in vanilla_voice_list)
            //        outputFile.WriteLine(line);
            //}
            log_console_output = false;
            foreach (var vl in voice_lines.Keys)
            {
                string full_path = @"sound\voice\" + vl.Plugin.ToLower() + @"\" + vl.VoiceTypeFSR.ToLower() + @"\" + vl.FileName.ToLower();
                //Log(full_path);

                if (vl.Text.TrimEnd().TrimStart().Length == 0) continue;
                if (vl.Text.StartsWith("{")) continue;
                if (vl.Text.StartsWith("(")) continue;
                if (vanilla_voice_list.Contains(full_path))
                {
                    Log($"Line {vl.FK} {vl.FileName} '{vl.Text}'");
                    Log($"Line not included because it has a vanilla voice already");
                    continue;
                }
                csv_lines.Add(string.Join("|", new string[] {
                    "skyrim",
                    vl.VoiceTypeReal,
                    vl.Text.Replace('"','\''),
                    "hifi",
                    full_path+".wav"
                }));
            }
            log_console_output = true;

            string voice_gen_path = modsfolder + @"\NPC Face Swap VoiceGen";
            if (!Directory.Exists(voice_gen_path)) Directory.CreateDirectory(voice_gen_path);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(voice_gen_path, "xva_synth_batch.csv")))
            {
                foreach (string line in csv_lines)
                    outputFile.WriteLine(line);
            }
        }
        
        private static bool Evaluate_Conditions(
            List<Tuple<string, FormKey, string, bool>> conditions, 
            Npc npc, 
            List<Tuple<string, FormKey, string, bool>> alias_cond,
            ref List<string>logs)
        {

            // func, thing's formkey, compare op, or flag ("or" if true, "and" if false)

            var voice = npc.Voice.Resolve<IVoiceTypeGetter>(state!.LinkCache);

            //logs.Add($"\t\treal voice {voice}");

            List<FormKey> npc_factions = new();
            foreach (var f in npc.Factions) npc_factions.Add(f.Faction.FormKey);

            bool conditions_met = false;

            // false=and, true=or
            bool checking_or = false;
            bool or_valid = false;
            int k = 0;
            //logs.Add($"\t\thas {conditions.Count} conditions");
            foreach (var t in conditions)
            {
                //logs.Add($"\t\t\tCheck # {k}:");
                k++;
                //logs.Add($"\t\t\t{t.Item1} {t.Item2} {t.Item3} {t.Item4}");
                if (t.Item4)
                {
                    //logs.Add($"\t\t\t\tIs an Or");
                    checking_or = true;
                    if (or_valid)
                    {
                        //logs.Add($"\t\t\t\tNo need to check, a previous or was true");
                        continue;
                    }
                }
                else if (checking_or)
                {
                    //logs.Add($"\t\t\t\tLast was an Or");
                    checking_or = false;
                    if (or_valid)
                    {
                        //logs.Add($"\t\t\t\tOrs are true");
                        or_valid = false;
                        continue;
                    }
                    else
                    {
                        //logs.Add($"\t\t\t\tNo ors were true");
                        conditions_met = false;
                        break;
                    }
                }

                if (t.Item1 == "GetIsID")
                {
                    //logs.Add($"\t\t\t\tIs a npc check");

                    if (t.Item2.Equals(npc.FormKey) && t.Item3 == "EqualTo")
                    {
                        //logs.Add($"\t\t\t\tnpc matches == 1");
                        conditions_met = true;
                        continue;
                    }
                    else if (!t.Item2.Equals(npc.FormKey) && t.Item3 == "NotEqualTo")
                    {
                        //logs.Add($"\t\t\t\tnpc not matches == 0");
                        conditions_met = true;
                        continue;
                    }
                    else
                    {
                        //logs.Add($"\t\t\t\tIs a faction check");

                        if (checking_or)
                        {
                            //logs.Add($"\t\t\t\tNo match, but check for other ors");
                            continue;
                        }
                        //logs.Add($"\t\t\t\tAand was not true, conditions NOT met");
                        conditions_met = false;
                        break;
                    }
                }
                else if (t.Item1 == "GetInFaction")
                {

                    if (npc_factions.Contains(t.Item2) && t.Item3 == "EqualTo")
                    {
                        //logs.Add($"\t\t\t\tfaction matches == 1");
                        conditions_met = true;
                        continue;
                    }
                    else if (!npc_factions.Contains(t.Item2) && t.Item3 == "NotEqualTo")
                    {
                        //logs.Add($"\t\t\t\tfaction not matches == 0");
                        conditions_met = true;
                        continue;
                    }
                    else
                    {
                        if (checking_or)
                        {
                            //logs.Add($"\t\t\t\tNo match, but check for other ors");
                            continue;
                        }
                        //logs.Add($"\t\t\t\tAand was not true, conditions NOT met");
                        conditions_met = false;
                        break;
                    }
                }
                else if (t.Item1 == "GetIsVoiceType")
                {

                    //logs.Add($"\t\t\t\tvoicetype/list is {t.Item2}");
                    if (state.LinkCache.TryResolve<IFormListGetter>(t.Item2,out var formlist))
                    {
                        var FL = formlist.DeepCopy();
                        if (FL.Items.Contains(voice) && t.Item3 == "EqualTo")
                        {
                            //logs.Add($"\t\t\t\tvoicelist matches == 1");
                            conditions_met = true;
                            continue;
                        }
                        else if (!FL.Items.Contains(voice) && t.Item3 == "NotEqualTo")
                        {
                            //logs.Add($"\t\t\t\tvoicelist not matches == 0");
                            conditions_met = true;
                            continue;
                        }
                        else
                        {
                            if (checking_or)
                            {
                                //logs.Add($"\t\t\t\tNo match, but check for other ors");
                                continue;
                            }
                            //logs.Add($"\t\t\t\tAand was not true, conditions NOT met");
                            conditions_met = false;
                            break;
                        }

                    }
                    else if (state.LinkCache.TryResolve<IVoiceTypeGetter>(t.Item2, out var cnd_voice))
                    {
                        if (voice.Equals(cnd_voice) && t.Item3 == "EqualTo")
                        {
                            //logs.Add($"\t\t\t\tvoice matches == 1");
                            conditions_met = true;
                            continue;
                        }
                        else if (!voice.Equals(cnd_voice) && t.Item3 == "NotEqualTo")
                        {
                            //logs.Add($"\t\t\t\tvoice not matches == 0");
                            conditions_met = true;
                            continue;
                        }
                        else
                        {
                            if (checking_or)
                            {
                                //logs.Add($"\t\t\t\tNo match, but check for other ors");
                                continue;
                            }
                            //logs.Add($"\t\t\t\tAand was not true, conditions NOT met");
                            conditions_met = false;
                            break;
                        }

                    }
                }
                else if (t.Item1 == "GetIsAliasRef")
                {

                    if (Evaluate_Conditions(alias_cond, npc, new List<Tuple<string, FormKey, string, bool>>(),ref logs))
                    {
                        //logs.Add($"\t\t\t\tfaction matches == 1");
                        conditions_met = true;
                        continue;
                    }
                    else
                    {
                        if (checking_or)
                        {
                            //logs.Add($"\t\t\t\tNo match, but check for other ors");
                            continue;
                        }
                        //logs.Add($"\t\t\t\tAand was not true, conditions NOT met");
                        conditions_met = false;
                        break;
                    }
                }
            }

            // edge case where a set of or conditions, all false, can still result in met conditions
            if (checking_or && !or_valid)
            {
                //logs.Add($"\t\t\tOr failed at end");
                conditions_met = false;
            }
            return conditions_met;

        }
        
        private static int RandomNormal(int low, int high)
        {
            double u1 = 1.0 - rnd.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rnd.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

            double mean = ((double)high + (double)low) / 2.0d;
            double reach= ((double)high - (double)low) / 2.0d;
            double stdDev = reach / 3;

            double randNormal =mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return Math.Max(Math.Min((int)Math.Round(randNormal),high),low);
        }

        private static void Log(string s,bool toconsole=true)
        {
            if (log_file_output)
            {
                using (StreamWriter outputFile = File.AppendText(log_path))
                {
                    outputFile.WriteLine(s);
                }
            }

            if (toconsole && log_console_output) Console.WriteLine(s);

        }

        private static void Log(IEnumerable<string> lst, bool toconsole = true)
        {
            if (log_file_output)
            {
                using (StreamWriter outputFile = File.AppendText(log_path))
                {
                    foreach (var str in lst)
                    {
                        outputFile.WriteLine(str);
                    }
                }
            }

            if (toconsole && log_console_output)
            { 
                foreach (var str in lst)
                {
                    Console.WriteLine(str);
                }
            }

        }
        private static string TruncateLongString(string str, int maxLength)
        {
            return str[0..Math.Min(str.Length, maxLength)];
        }

        private static List<IRaceGetter> FindValidRaces()
        {

            var npcKW = FormKey.Factory("013794:Skyrim.esm");
            var npcMiraakKW = FormKey.Factory("03CA98:Dragonborn.esm");

            var creatureKW = FormKey.Factory("013795:Skyrim.esm");
            var dwarvenKW = FormKey.Factory("01397A:Skyrim.esm");
            var animalKW = FormKey.Factory("013798:Skyrim.esm");
            var dragonKW = FormKey.Factory("035D59:Skyrim.esm");

            var defaultRace = FormKey.Factory("000019:Skyrim.esm");

            List<IRaceGetter> valid_races = new();
            foreach (var race in state!.LoadOrder.PriorityOrder.Race().WinningOverrides())
            {


                if (race.Keywords != null && (race.Keywords.Contains(creatureKW) || race.Keywords.Contains(animalKW)))
                {
                    if (race.Name?.String != null && !race.Name.String.Contains("Fox")) continue;
                }
                if (race.Keywords != null && race.Keywords.Contains(dwarvenKW)) continue;
                if (race.Keywords != null && race.Keywords.Contains(dragonKW)) continue;


                if (race.Keywords != null && (race.Keywords.Contains(npcKW) || race.Keywords.Contains(npcMiraakKW)))
                {

                    if ((((ulong)race.Flags >> 1) & 1) != 1) continue;

                    if (race.BodyTemplate != null && race.BodyTemplate.FirstPersonFlags == 0) continue;

                    valid_races.Add(race);
                }
            }
            return valid_races;
        }


        private static void EditArma()
        {
            //iterate through all arma to be sure that all armors have a female/male version
            foreach (var armoraddon in state!.LoadOrder.PriorityOrder.ArmorAddon().WinningOverrides())
            {
                Log($"{armoraddon.EditorID} : {armoraddon.BodyTemplate?.FirstPersonFlags}");
                bool male_slider = armoraddon.WeightSliderEnabled.Male;
                bool female_slider = armoraddon.WeightSliderEnabled.Female;

                if (male_slider != female_slider)
                {
                    var newaa = state.PatchMod.ArmorAddons.GetOrAddAsOverride(armoraddon);
                    if (!male_slider) newaa.WeightSliderEnabled.Male = true;
                    if (!female_slider) newaa.WeightSliderEnabled.Female = true;
                    Log($"{newaa.EditorID} : changed weight slider: {male_slider} {female_slider}");
                }

                bool male_first_model = !string.IsNullOrWhiteSpace(armoraddon.FirstPersonModel?.Male?.File.RawPath);
                bool female_first_model = !string.IsNullOrWhiteSpace(armoraddon.FirstPersonModel?.Female?.File.RawPath);

                if (male_first_model != female_first_model)
                {
                    var newaa = state.PatchMod.ArmorAddons.GetOrAddAsOverride(armoraddon);
                    Log($"{newaa.EditorID} : changed 1st person model: {male_first_model} {female_first_model}");
                    if (!male_first_model) newaa.FirstPersonModel!.Male = new Model() { File = "Armor\\Studded\\Male\\1stPersonbody_1.nif" };
                    if (!female_first_model) newaa.FirstPersonModel!.Female = new Model() { File = "Armor\\Studded\\Female\\1stPersonbody_1.nif" };
                }

                bool male_world_model = !string.IsNullOrWhiteSpace(armoraddon.WorldModel?.Male?.File.RawPath);
                bool female_world_model = !string.IsNullOrWhiteSpace(armoraddon.WorldModel?.Female?.File.RawPath);

                if (male_world_model != female_world_model)
                {
                    var newaa = state.PatchMod.ArmorAddons.GetOrAddAsOverride(armoraddon);
                    Log($"{newaa.EditorID} : changed world model: {male_world_model} {female_world_model}");
                    if (!male_world_model) newaa.WorldModel!.Male = new Model() { File = "Armor\\Studded\\Male\\1stPersonbody_1.nif" };
                    if (!female_world_model) newaa.WorldModel!.Female = new Model() { File = "Armor\\Studded\\Female\\1stPersonbody_1.nif" };
                }
            }
        }
    }
}
