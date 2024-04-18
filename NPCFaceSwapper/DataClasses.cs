using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCFaceSwapper
{
    public class VoiceLine: IEquatable<VoiceLine>
    {
        public string Text="";
        public string Emotion="";
        public uint EmotionLevel=0;
        public string FileName="";
        public string Plugin = "";
        public string VoiceTypeFSR = "";
        public string VoiceTypeReal = "";
        public FormKey FK=new();


        public VoiceLine(string text, string emotion, uint emotionlevel, string filename,string plugin, string voice_type_fsr,string voice_type_real,FormKey fk) {
            Text = text;
            Emotion = emotion;  
            EmotionLevel = emotionlevel;
            FileName = filename;
            Plugin = plugin;
            VoiceTypeFSR = voice_type_fsr;
            VoiceTypeReal = voice_type_real;
            FK = fk;
        }
        public bool Equals (VoiceLine? other)
        {
            if (other == null) return false;
            return Text == other.Text && Emotion == other.Emotion && EmotionLevel == other.EmotionLevel && FileName==other.FileName && Plugin==other.Plugin && VoiceTypeFSR == other.VoiceTypeFSR && VoiceTypeReal == other.VoiceTypeReal && FK==other.FK;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode() + Emotion.GetHashCode() + EmotionLevel.GetHashCode() + FileName.GetHashCode() + Plugin.GetHashCode() + VoiceTypeFSR.GetHashCode() + VoiceTypeReal.GetHashCode() + FK.GetHashCode();
        }

    }
    public class NPCSwap : IEquatable<NPCSwap>
    {
        public IFormLinkGetter<INpcGetter> Dest_npc;
        public IFormLinkGetter<INpcGetter> Source_npc;
        public ModKey? Source_plugin;
        public bool Futa = false;
        public ModKey Sos_plugin;
        public sbyte Sos_size = 10;

        public NPCSwap(IFormLinkGetter<INpcGetter> dnpc, IFormLinkGetter<INpcGetter> snpc,ModKey? splug,bool futa,ModKey sos_plugin,sbyte sos_size)
        {
            Dest_npc = dnpc;
            Source_npc = snpc;
            Source_plugin = splug;
            Futa = futa;
            Sos_plugin = sos_plugin;
            Sos_size = sos_size;
        }
        public NPCSwap(IFormLinkGetter<INpcGetter> dnpc, IFormLinkGetter<INpcGetter> snpc, ModKey? splug)
        {
            Dest_npc = dnpc;
            Source_npc = snpc;
            Source_plugin = splug;
            Futa = false;
        }
        public bool Equals(NPCSwap? other)
        {
            if (other == null) return false;
            return other.Dest_npc.Equals(this.Dest_npc);

        }
        public override int GetHashCode()
        {
            return this.Dest_npc.GetHashCode();
        }

        public override string ToString()
        {
            if (Futa)
            {
                return Dest_npc.FormKey.ToString() + " " + Source_npc.FormKey.ToString() + " " + Source_plugin.ToString() + " " + Sos_plugin.ToString() + " " + Sos_size.ToString();

            }
            return Dest_npc.FormKey.ToString() + " " + Source_npc.FormKey.ToString() + " " + Source_plugin.ToString();
        }

    }
}
