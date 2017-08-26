using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using Xamarin.Forms;

namespace SCS2
{
	public static class Utilities
	{
		public static string ConvertFromXMLSafe(string s)
		{
			return s.Replace('_', ' ').Replace("-ap-", "'").Replace("-co-", ",").Replace("-lb-", "[").Replace("-rb-", "]").Replace("-op-", "(").Replace("-cp-", ")").Replace("-cn-", ":");
		}

		public static string ConvertToXMLSafe(string s)
		{
			return s.Replace(' ', '_').Replace("'", "-ap-").Replace(",", "-co-").Replace("[", "-lb-").Replace("]", "-rb-").Replace("(", "-op-").Replace(")", "-cp-").Replace(":", "-cn-");
		}
	}

    public struct TextBoxState
    {
        //public TextBox TB;
        //public BorderStyle BS;
        public bool bReadOnly;
    }

	public class VarValPair
	{
		public string Variable = "";
		public string Value = "";

		public VarValPair()
		{
			return;
		}

		public VarValPair(string vr, string vl)
		{
			Variable = vr;
			Value = vl;

			return;
		}

		public bool GetBool()
		{
			bool b;
			bool.TryParse(Value, out b);
			return b;
		}

		public int GetInt()
		{
			int i;
			int.TryParse(Value, out i);
			return i;
		}

		public float GetFloat()
		{
			float f;
			float.TryParse(Value, out f);
			return f;
		}

		public string GetString()
		{
			return Value;
		}
	}

	public abstract class BaseInfo
	{
		public string Name = "";
		public string Source = "";
	}

    public class AbilityInfo : BaseInfo
    {
        public string Abbreviation = "";

        public AbilityInfo(string a, string n)
        {
            Name = n;
            Abbreviation = a;

            return;
        }
    }

    public class SkillInfo : BaseInfo
    {
        public string BaseAbility = "";

        public SkillInfo(string ba, string n)
        {
            Name = n;
            BaseAbility = ba;

            return;
        }
    }

    public struct PrerequisiteInfo
    {
        public string Type;
        public string Value;
        public List<string> CompoundHints;
    }

    public class TableInfo : BaseInfo
    {
        public List<string> Headers = new List<string>();
        public List<float> HeaderSizes = new List<float>();
        public List<string> Cells = new List<string>();

        public TableInfo()
        {
            return;
        }

        public void AddHeader(string h, float s)
        {
            Headers.Add(h);
            if (s < 0.0f) HeaderSizes.Add(0.0f);
            else if (s > 1.0f) HeaderSizes.Add(1.0f);
            else HeaderSizes.Add(s);

            return;
        }

        //public float RenderOn(Graphics g, Font TextFont, float LeftMargin, float RightMargin, float SY)
        //{
        //    float oy = SY;

        //    return oy;
        //}
    }

    public class FeatureInfo : BaseInfo
    {
        public string Type = "";
        public string Level = "";
        public string Description = "";
        public string OptionGroup = "";

        public void ReadData(XmlReader reader)
        {
            Level = reader.GetAttribute("level");
            if (Level == null) Level = "";
            Type = reader.GetAttribute("type");
            if (Type == null) Type = "";
            OptionGroup = reader.GetAttribute("optiongroup");
            if (OptionGroup == null) OptionGroup = "";
            if ((Type == "text") || (Type == "skill") || (Type == "speed") || (Type == "defemse") || (Type == "initiative") || (Type == "tierhp") || (Type == "untrained"))
            {
                Name = reader.GetAttribute("name");
                if (Name == null) Name = "";
                Description = reader.Value.Replace("[rn]", "\r\n");
                if (Description == null) Description = "";
            }
            else
            {
                Name = reader.Value;
                if (Name == null) Name = "";
            }

            return;
        }
    }

    public class RaceInfo : BaseInfo
    {
        public List<string> Races = new List<string>();
        public new string Name
        {
            get
            {
                if (Races.Count > 0) return Races[0];
                else return "";
            }
            set
            {
                if (Races.Count == 0) Races.Add(value);
                else Races[0] = value;
            }
        }
        public List<string> AbilityBonuses = new List<string>();
        public string Size = "";
        public int Speed = 0;
        public string Vision = "";
        public List<string> Languages = new List<string>();
        public List<string> SkillBonuses = new List<string>();
        public List<string> DefenseBonuses = new List<string>();
        public List<FeatureInfo> RaceFeatures = new List<FeatureInfo>();
    }

    public class ClassInfo : BaseInfo
    {
        public List<string> ArmorProficiencies = new List<string>();
        public List<string> ShieldProficiencies = new List<string>();
        public List<string> DefenseBonuses = new List<string>();
        public int HPBase = 0;
        public int HPLevel = 0;
        public int Surges = 0;
        public int InitialOptionalSkills = 0;
        public List<string> TrainedSkills = new List<string>();
        public List<string> ClassSkills = new List<string>();
        public List<FeatureInfo> Features = new List<FeatureInfo>();
    }

    public class DeityInfo : BaseInfo
    {
        public string Alignment = "";
    }

    public class FeatureSetInfo : BaseInfo
    {
        public List<FeatureInfo> Features = new List<FeatureInfo>();
        public List<PrerequisiteInfo> Prerequisites = new List<PrerequisiteInfo>();

        public void AddPrerequisite(string t, string v, List<string> chs)
        {
            PrerequisiteInfo pi;
            pi.Type = t;
            pi.Value = v;
            pi.CompoundHints = chs;
            Prerequisites.Add(pi);
            return;
        }
    }

    public class PowerInfo : BaseInfo
    {
        public string Class = "";
        public string Type = "";
        public string Level = "";
        public string Flavor = "";
        public string Frequency = "";
        public List<string> Properties = new List<string>();
        public string ActionTime = "";
        public string ActionType = "";
        public string ActionRange = "";
        public string Leveling = "";
		public List<VarValPair> Data = new List<VarValPair>();

        public PowerInfo()
        {
            return;
        }

        public void AddData(string h, string t)
        {
            Data.Add(new VarValPair(h, t));

            return;
        }
    }

    public class FeatInfo : FeatureSetInfo
    {
        public string Tier = "";
		public List<VarValPair> Data = new List<VarValPair>();

        public FeatInfo()
        {
            return;
        }

        public void AddData(string h, string t)
        {
			Data.Add(new VarValPair(h, t));
            return;
        }
    }

    public class RitualInfo : BaseInfo
    {
        public string Flavor = "";
		public List<VarValPair> Properties = new List<VarValPair>();
		public List<VarValPair> Data = new List<VarValPair>();
        public List<TableInfo> Tables = new List<TableInfo>();

        public RitualInfo()
        {
            return;
        }

        public void AddProperty(string h, string t)
        {
			Properties.Add(new VarValPair(h, t));
            return;
        }

        public void AddData(string h, string t)
        {
			Data.Add(new VarValPair(h, t));
            return;
        }

        public string GetProperty(string ph)
        {
			VarValPair vvp = Properties.Find(v => v.Variable == ph);
			if (vvp == null) return "";
			else return vvp.Value;
        }
    }

	public class EquipmentInfo : BaseInfo
	{
		public List<string> Categories = new List<string>();
		public List<VarValPair> Properties = new List<VarValPair>();

		public EquipmentInfo()
		{
			return;
		}

		public VarValPair GetProperty(string s)
		{
			VarValPair vvp = Properties.Find(v => v.Variable == s);
			return vvp;
		}

		public bool GetBoolProperty(string s)
		{
			VarValPair vvp = GetProperty(s);
			if (vvp == null) return false;
			return vvp.GetBool();
		}

		public int GetIntProperty(string s)
		{
			VarValPair vvp = GetProperty(s);
			if (vvp == null) return 0;
			return vvp.GetInt();
		}

		public float GetFloatProperty(string s)
		{
			VarValPair vvp = GetProperty(s);
			if (vvp == null) return 0;
			return vvp.GetFloat();
		}

		public string GetStringProperty(string s)
		{
			VarValPair vvp = GetProperty(s);
			if (vvp == null) return "";
			return vvp.GetString();
		}

		public List<string> GetListProperty(string s)
		{
			List<string> l = (from p in Properties
							  where p.Variable == s
							  select p.GetString()).ToList();

			return l;
		}

		public void AddProperty(string var, string val)
		{
			VarValPair vvp = new VarValPair();
			vvp.Variable = var;
			vvp.Value = val;
			Properties.Add(vvp);

			return;
		}
	}

	public class ModuleInfo : BaseInfo
	{
		public string Author = "";
		public string Version = "";
		public SF data = SF.newdata;
	}

	public enum MergeStyle { MERGE_NOCHECK, MERGE_KEEP, MERGE_OVERWRITE }

    public class SF
    {
        public string AppName = "Character Sheet v0.8a";
		public List<ModuleInfo> Modules = new List<ModuleInfo>();
        public List<AbilityInfo> Abilities = new List<AbilityInfo>();
        public List<SkillInfo> Skills = new List<SkillInfo>();
        public List<RaceInfo> Races = new List<RaceInfo>();
        public List<ClassInfo> Classes = new List<ClassInfo>();
        public List<string> Alignments = new List<string>();
        public List<DeityInfo> Deities = new List<DeityInfo>();
        public List<FeatureSetInfo> Paragons = new List<FeatureSetInfo>();
        public List<FeatureSetInfo> Epics = new List<FeatureSetInfo>();
        public List<PowerInfo> Powers = new List<PowerInfo>();
        public List<FeatInfo> Feats = new List<FeatInfo>();
        public List<RitualInfo> Rituals = new List<RitualInfo>();
		public List<EquipmentInfo> Equipment = new List<EquipmentInfo>();
        public Node Root = new Node("root", null);
        //public charsheet MainForm = null;
        private bool bStrictMode = true;
        private int StrictID = 0;
        public int UpdateID = 0;
        //private PowerManagerNode featmgr = null;

        private SF()
        {
            return;
        }

		public static SF newdata
		{
			get { return new SF(); }
		}

        private static SF g_data = new SF();
		public static SF data => g_data;

		public List<BaseInfo> GetAnotB(List<BaseInfo> a, List<BaseInfo> b)
		{
			List<BaseInfo> c = new List<BaseInfo>();

			foreach (BaseInfo bi in a)
				if (null == b.Find(x => x.Name == bi.Name)) c.Add(bi);

			return c;
		}

		public List<BaseInfo> GetAinB(List<BaseInfo> a, List<BaseInfo> b)
		{
			List<BaseInfo> c = new List<BaseInfo>();

			foreach (BaseInfo bi in a)
				if (null != b.Find(x => x.Name == bi.Name)) c.Add(bi);

			return c;
		}

		public string CheckConflictsWith(SF td)
		{
			string s = "";

			foreach (AbilityInfo ai in Abilities)
				if (null != td.Abilities.Find(x => x.Name == ai.Name))
					s += "Ability : " + ai.Name + "\r\n";

			foreach (SkillInfo si in Skills)
				if (null != td.Skills.Find(x => x.Name == si.Name))
					s += "Skill : " + si.Name + "\r\n";

			foreach (RaceInfo ri in Races)
				if (null != td.Races.Find(x => x.Name == ri.Name))
					s += "Race : " + ri.Name + "\r\n";

			foreach (ClassInfo ci in Classes)
				if (null != td.Classes.Find(x => x.Name == ci.Name))
					s += "Class : " + ci.Name + "\r\n";

			foreach (string a in Alignments)
				if (null != td.Alignments.Find(x => x == a))
					s += "Alignment : " + a + "\r\n";

			foreach (DeityInfo di in Deities)
				if (null != td.Deities.Find(x => x.Name == di.Name))
					s += "Deity : " + di.Name + "\r\n";

			foreach (FeatureSetInfo pi in Paragons)
				if (null != td.Paragons.Find(x => x.Name == pi.Name))
					s += "Paragon Path : " + pi.Name + "\r\n";

			foreach (FeatureSetInfo ei in Epics)
				if (null != td.Epics.Find(x => x.Name == ei.Name))
					s += "Epic Destiny : " + ei.Name + "\r\n";

			foreach (PowerInfo pi in Powers)
				if (null != td.Powers.Find(x => x.Name == pi.Name))
					s += "Power : " + pi.Name + "\r\n";

			foreach (FeatInfo fi in Feats)
				if (null != td.Feats.Find(x => x.Name == fi.Name))
					s += "Feat : " + fi.Name + "\r\n";

			foreach (RitualInfo ri in Rituals)
				if (null != td.Rituals.Find(x => x.Name == ri.Name))
					s += "Ritual : " + ri.Name + "\r\n";

			foreach (EquipmentInfo ei in Equipment)
				if (null != td.Equipment.Find(x => x.Name == ei.Name))
					s += "Equipment : " + ei.Name + "\r\n";

			return s;
		}

		public void MergeWith(SF td, MergeStyle ms)
		{
			if (ms == MergeStyle.MERGE_NOCHECK)
			{
				Abilities.AddRange(td.Abilities);
				Skills.AddRange(td.Skills);
				Races.AddRange(td.Races);
				Classes.AddRange(td.Classes);
				Alignments.AddRange(td.Alignments);
				Deities.AddRange(td.Deities);
				Paragons.AddRange(td.Paragons);
				Epics.AddRange(td.Epics);
				Powers.AddRange(td.Powers);
				Feats.AddRange(td.Feats);
				Rituals.AddRange(td.Rituals);
				Equipment.AddRange(td.Equipment);
			}
			else
			{
				foreach (AbilityInfo ai in td.Abilities)
				{
					AbilityInfo tai = Abilities.Find(x => x.Name == ai.Name);
					if (tai == null) Abilities.Add(ai);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Abilities[Abilities.IndexOf(tai)] = ai;
				}
				foreach (SkillInfo si in td.Skills)
				{
					SkillInfo tsi = Skills.Find(x => x.Name == si.Name);
					if (tsi == null) Skills.Add(si);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Skills[Skills.IndexOf(tsi)] = si;
				}
				foreach (RaceInfo ri in td.Races)
				{
					RaceInfo tri = Races.Find(x => x.Name == ri.Name);
					if (tri == null) Races.Add(ri);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Races[Races.IndexOf(tri)] = ri;
				}
				foreach (ClassInfo ci in td.Classes)
				{
					ClassInfo tci = Classes.Find(x => x.Name == ci.Name);
					if (tci == null) Classes.Add(ci);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Classes[Classes.IndexOf(tci)] = ci;
				}
				foreach (string a in td.Alignments)
				{
					string ta = Alignments.Find(x => x == a);
					if (ta == null) Alignments.Add(a);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Alignments[Alignments.IndexOf(ta)] = a;
				}
				foreach (DeityInfo di in td.Deities)
				{
					DeityInfo tdi = Deities.Find(x => x.Name == di.Name);
					if (tdi == null) Deities.Add(di);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Deities[Deities.IndexOf(tdi)] = di;
				}
				foreach (FeatureSetInfo pi in td.Paragons)
				{
					FeatureSetInfo tpi = Paragons.Find(x => x.Name == pi.Name);
					if (tpi == null) Paragons.Add(pi);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Paragons[Paragons.IndexOf(tpi)] = pi;
				}
				foreach (FeatureSetInfo ei in td.Epics)
				{
					FeatureSetInfo tei = Epics.Find(x => x.Name == ei.Name);
					if (tei == null) Epics.Add(ei);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Epics[Epics.IndexOf(tei)] = ei;
				}
				foreach (PowerInfo pi in td.Powers)
				{
					PowerInfo tpi = Powers.Find(x => x.Name == pi.Name);
					if (tpi == null) Powers.Add(pi);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Powers[Powers.IndexOf(tpi)] = pi;
				}
				foreach (FeatInfo fi in td.Feats)
				{
					FeatInfo tfi = Feats.Find(x => x.Name == fi.Name);
					if (tfi == null) Feats.Add(fi);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Feats[Feats.IndexOf(tfi)] = fi;
				}
				foreach (RitualInfo ri in td.Rituals)
				{
					RitualInfo tri = Rituals.Find(x => x.Name == ri.Name);
					if (tri == null) Rituals.Add(ri);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Rituals[Rituals.IndexOf(tri)] = ri;
				}
				foreach (EquipmentInfo ei in td.Equipment)
				{
					EquipmentInfo tei = Equipment.Find(x => x.Name == ei.Name);
					if (tei == null) Equipment.Add(ei);
					else if (ms == MergeStyle.MERGE_OVERWRITE) Equipment[Equipment.IndexOf(tei)] = ei;
				}
			}
			
			return;
		}

		public bool ReplaceModule(ModuleInfo nmi)
		{
			ModuleInfo omi = Modules.Find(x => x.Name == nmi.Name);
			if (omi == null) return false;
			// first merge new by overwrite existing 
			MergeWith(nmi.data, MergeStyle.MERGE_OVERWRITE);
			// now remove whatever content in omi wasn't overwritten
			foreach (AbilityInfo ai in omi.data.Abilities)
			{
				AbilityInfo tai = nmi.data.Abilities.Find(x => x.Name == ai.Name);
				if (tai == null) Abilities.Remove(ai);
			}
			foreach (SkillInfo si in omi.data.Skills)
			{
				SkillInfo tsi = nmi.data.Skills.Find(x => x.Name == si.Name);
				if (tsi == null) Skills.Remove(si);
			}
			foreach (RaceInfo ri in omi.data.Races)
			{
				RaceInfo tri = nmi.data.Races.Find(x => x.Name == ri.Name);
				if (tri == null)
				{
					Node n = Root.FindNode("Race", null);
					if (ri.Name == n.Value) (n.V as Picker).SelectedIndex = -1;
					Races.Remove(ri);
				}
			}
			foreach (ClassInfo ci in omi.data.Classes)
			{
				ClassInfo tci = nmi.data.Classes.Find(x => x.Name == ci.Name);
				if (tci == null)
				{
					Node n = Root.FindNode("Class", null);
					if (ci.Name == n.Value) (n.V as Picker).SelectedIndex = -1;
					Classes.Remove(ci);
				}
			}
			foreach (string a in omi.data.Alignments)
			{
				string ta = nmi.data.Alignments.Find(x => x == a);
				if (ta == null) Alignments.Remove(a);
			}
			foreach (DeityInfo di in omi.data.Deities)
			{
				DeityInfo tdi = nmi.data.Deities.Find(x => x.Name == di.Name);
				if (tdi == null) Deities.Remove(di);
			}
			foreach (FeatureSetInfo pi in omi.data.Paragons)
			{
				FeatureSetInfo tpi = nmi.data.Paragons.Find(x => x.Name == pi.Name);
				if (tpi == null)
				{
					Node n = Root.FindNode("ParagonPath", null);
					if (pi.Name == n.Value) (n.V as Picker).SelectedIndex = -1;
					Paragons.Remove(pi);
				}
			}
			foreach (FeatureSetInfo ei in omi.data.Epics)
			{
				FeatureSetInfo tei = nmi.data.Epics.Find(x => x.Name == ei.Name);
				if (tei == null)
				{
					Node n = Root.FindNode("EpicDestiny", null);
					if (ei.Name == n.Value) (n.V as Picker).SelectedIndex = -1;
					Epics.Remove(ei);
				}
			}
			foreach (PowerInfo pi in omi.data.Powers)
			{
				PowerInfo tpi = nmi.data.Powers.Find(x => x.Name == pi.Name);
				if (tpi == null)
				{
					/*PowerManagerNode awp = Root.FindNode("At-WillPowers", null) as PowerManagerNode;
					PowerManagerNode ep = Root.FindNode("EncounterPowers", null) as PowerManagerNode;
					PowerManagerNode dp = Root.FindNode("DailyPowers", null) as PowerManagerNode;
					PowerManagerNode up = Root.FindNode("UtilityPowers", null) as PowerManagerNode;
					if (awp.HasValue(pi.Name, "")) awp.RemovePower(pi.Name, false);
					if (ep.HasValue(pi.Name, "")) ep.RemovePower(pi.Name, false);
					if (dp.HasValue(pi.Name, "")) dp.RemovePower(pi.Name, false);
					if (up.HasValue(pi.Name, "")) up.RemovePower(pi.Name, false);*/
					Powers.Remove(pi);
				}
			}
			foreach (FeatInfo fi in omi.data.Feats)
			{
				FeatInfo tfi = nmi.data.Feats.Find(x => x.Name == fi.Name);
				if (tfi == null)
				{
					/*PowerManagerNode f = Root.FindNode("Feats", null) as PowerManagerNode;
					if (f.HasValue(fi.Name, "")) f.RemovePower(fi.Name, false);
					Feats.Remove(fi);*/
				}
			}
			foreach (RitualInfo ri in omi.data.Rituals)
			{
				RitualInfo tri = nmi.data.Rituals.Find(x => x.Name == ri.Name);
				if (tri == null)
				{
					/*PowerManagerNode r = Root.FindNode("Rituals", null) as PowerManagerNode;
					if (r.HasValue(ri.Name, "")) r.RemovePower(ri.Name, false);
					Rituals.Remove(ri);*/
				}
			}
			foreach (EquipmentInfo ei in omi.data.Equipment)
			{
				EquipmentInfo tei = nmi.data.Equipment.Find(x => x.Name == ei.Name);
				if (tei == null) Equipment.Remove(ei);
			}

			return true;
		}

		public void UnloadModule(string mn)
		{
			ModuleInfo mi = Modules.Find(m => m.Name == mn);
			if (mi == null) return;

			// fully unload race being used
			Node n = Root.FindNode("Race", null);
			foreach (RaceInfo ri in Races)
			{
				if ((ri.Source == mi.Name) && (ri.Name == n.Value))
				{
					(n.V as Picker).SelectedIndex = -1;
					break;
				}
			}
			Races.RemoveAll(x => x.Source == mi.Name);
			// fully unload class being used
			n = Root.FindNode("Class", null);
			foreach (ClassInfo ci in Classes)
			{
				if ((ci.Source == mi.Name) && (ci.Name == n.Value))
				{
					(n.V as Picker).SelectedIndex = -1;
					break;
				}
			}
			Classes.RemoveAll(x => x.Source == mi.Name);
			// fully unload paragon path being used
			n = Root.FindNode("ParagonPath", null);
			foreach (FeatureSetInfo fsi in Paragons)
			{
				if ((fsi.Source == mi.Name) && (fsi.Name == n.Value))
				{
					(n.V as Picker).SelectedIndex = -1;
					break;
				}
			}
			Paragons.RemoveAll(x => x.Source == mi.Name);
			// fully unload epic destiny being used
			n = Root.FindNode("EpicDestiny", null);
			foreach (FeatureSetInfo fsi in Epics)
			{
				if ((fsi.Source == mi.Name) && (fsi.Name == n.Value))
				{
					(n.V as Picker).SelectedIndex = -1;
					break;
				}
			}
			Epics.RemoveAll(x => x.Source == mi.Name);
			Abilities.RemoveAll(x => x.Source == mi.Name);
			Skills.RemoveAll(x => x.Source == mi.Name);
			Deities.RemoveAll(x => x.Source == mi.Name);
			// fully unload powers being used
			/*PowerManagerNode awp = Root.FindNode("At-WillPowers", null) as PowerManagerNode;
			PowerManagerNode ep = Root.FindNode("EncounterPowers", null) as PowerManagerNode;
			PowerManagerNode dp = Root.FindNode("DailyPowers", null) as PowerManagerNode;
			PowerManagerNode up = Root.FindNode("UtilityPowers", null) as PowerManagerNode;*/
			foreach (PowerInfo pi in Powers)
			{
				if (pi.Source == mi.Name)
				{
					/*if (awp.HasValue(pi.Name, "")) awp.RemovePower(pi.Name, false);
					if (ep.HasValue(pi.Name, "")) ep.RemovePower(pi.Name, false);
					if (dp.HasValue(pi.Name, "")) dp.RemovePower(pi.Name, false);
					if (up.HasValue(pi.Name, "")) up.RemovePower(pi.Name, false);*/
				}
			}
			Powers.RemoveAll(x => x.Source == mi.Name);
			// fully unload feats being used
			n = Root.FindNode("Feats", null);
			foreach (FeatInfo fi in Feats)
			{
				//if ((fi.Source == mi.Name) && (n as PowerManagerNode).HasValue(fi.Name, "")) (n as PowerManagerNode).RemovePower(fi.Name, false);
			}
			Feats.RemoveAll(x => x.Source == mi.Name);
			// fully unload rituals being used
			n = Root.FindNode("Rituals", null);
			foreach (RitualInfo ri in Rituals)
			{
				//if ((ri.Source == mi.Name) && (n as PowerManagerNode).HasValue(ri.Name, "")) (n as PowerManagerNode).RemovePower(ri.Name, false);
			}
			Rituals.RemoveAll(x => x.Source == mi.Name);
			// need to implement fully unloading equipment being used
			Equipment.RemoveAll(x => x.Source == mi.Name);
		}

        public bool StrictMode
        {
            get { return bStrictMode; }
            set { Root.SetStrictMode(++StrictID, value); bStrictMode = value; }
        }

        public bool MeetsLevelQualification(string l)
        {
            int cl = 0;
            int.TryParse(Root.FindNode("Level", null).Value, out cl);
            if (cl == 0) return true;
            int rl = 0;
            int.TryParse(l, out rl);
            return (cl >= rl);
        }

        public bool MeetsRaceQualification(string r)
        {
            RaceNode rn = Root.FindNode("Race", null) as RaceNode;
            if (rn == null) return true;
            if (rn.Races.Count == 0) return true;
            for (int i = 0; i < rn.Races.Count; i++)
                if (rn.Races[i] == r) return true;
            return false;
        }

        public bool MeetsRaceFeatureQualification(string rf)
        {
            if (rf == "") return true;
            // search all classes and their features for a match
            for (int i = 0; i < Races.Count; i++)
            {
                for (int j = 0; j < Races[i].RaceFeatures.Count; j++)
                {
                    // only features currently used can qualify for matching
                    if (IsUsingFeature("RaceFeatures", Races[i].RaceFeatures[j]))
                    {
                        if (rf == Races[i].RaceFeatures[j].Name) return true;
                    }
                }
            }

            return false;
        }

        public bool MeetsClassQualification(string c)
        {
            ClassNode cn = Root.FindNode("Class", null) as ClassNode;
            if ((cn == null) || (cn.Classes.Count == 0)) return true;
            string s = "";
            s = cn.Classes.Find(sc => c == sc);
            return (c == s);
        }

        public bool MeetsClassFeatureQualification(string f)
        {
            if (f == "") return true;
            // search all classes and their features for a match
            foreach (ClassInfo ci in Classes)
            {
				foreach (FeatureInfo fi in ci.Features)
                {
                    // only features currently used can qualify for matching
                    if (IsUsingFeature("Features", fi) && (f == fi.Name)) return true;
                }
            }

            return false;
        }

        public bool MeetsAbilityQualification(string a)
        {
            string s = a.Substring(0, 3).ToUpper();
            Node n = Root.FindNode("Total" + s, null);
            if (n == null) return true;
            int i = 0;
            int.TryParse(n.Value, out i);
            // special case - 0 means not set yet, so we should ok his
            if (i == 0) return true;
            s = a.Substring(4);
            int ia = 0;
            int.TryParse(s, out ia);
            return (i >= ia);
        }

        public bool MeetsTrainedQualification(string t)
        {
            Node n = null;
            n = Root.FindNode(t + "Trained", null);
            if (n == null) return false;
            return (n.Value == "5");
        }

        public bool MeetsWorshipQualification(string d)
        {
            Node n = Root.FindNode("Deity", null);
            if (n == null) return false;
            if (n.Value == "") return true;
            return (d == n.Value);
        }

        public bool MeetsFeatQualification(string f)
        {
			/*if (featmgr == null)
			{
				featmgr = Root.FindNode("Feats", null) as PowerManagerNode;
				if (featmgr == null) return false;
			}
			if (f != "multiclass") return featmgr.HasValue(f, "");
            else
            {
                for (int i = 0; i < featmgr.Powers.Count; i++)
                    if (featmgr.Powers[i].IndexOf("Multiclass") > -1) return true;
                return false;
            }*/
			return true;
        }

        public bool MeetsPrerequisites(List<PrerequisiteInfo> Prereqs)
        {
            int j = 0, k = 0;
            List<string> values = new List<string>();
            for (int i = 0; i < Prereqs.Count; i++)
            {
                // this is a custom field that we really don't know how to parse
                if (Prereqs[i].Type == "text") continue;
                // this is how we support or'd requirements
                values.Clear();
                j = 0;
                if (-1 == Prereqs[i].Value.IndexOf(" or ", 0))
                {
                    values.Add(Prereqs[i].Value);
                }
                else
                {
                    while (j != -1)
                    {
                        k = Prereqs[i].Value.IndexOf(" or ", j);
                        if (k == -1)
                        {
                            values.Add(Prereqs[i].Value.Substring(j));
                            j = k;
                        }
                        else
                        {
                            values.Add(Prereqs[i].Value.Substring(j, k - j));
                            j = k + 4;
                        }
                    }
                }
                if (values.Count == 0) return false;

                if (Prereqs[i].Type == "level")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsLevelQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "race")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsRaceQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "racial power")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsRaceFeatureQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "class")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsClassQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "class feature")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsClassFeatureQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "ability")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsAbilityQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "trained")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsTrainedQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "worship")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsWorshipQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "feat")
                {
                    for (j = 0; j < values.Count; j++)
                        if (MeetsFeatQualification(values[j])) break;
                    if (j == values.Count) return false;
                }
                else if (Prereqs[i].Type == "compound")
                {
                    for (j = 0; j < values.Count; j++)
                    {
                        if ((Prereqs[i].CompoundHints[j] == "level") && MeetsLevelQualification(values[j])) break;
                        else if ((Prereqs[i].CompoundHints[j] == "ability") && MeetsAbilityQualification(values[j])) break;
                        else if ((Prereqs[i].CompoundHints[j] == "feat") && MeetsFeatQualification(values[j])) break;
                        else if ((Prereqs[i].CompoundHints[j] == "trained") && MeetsTrainedQualification(values[j])) break;
                        else if ((Prereqs[i].CompoundHints[j] == "worship") && MeetsWorshipQualification(values[j])) break;
                    }
                    if (j == values.Count) return false;
                }
            }

            return true;
        }

        public bool IsQualifiedForFeat(FeatInfo fi)
        {
            if (fi == null) return false;
            /*if (featmgr == null)
            {
                featmgr = Root.FindNode("Feats", null) as PowerManagerNode;
                if (featmgr == null) return false;
            }
            if (featmgr.HasValue(fi.Name, "")) return false;*/
            List<string> values = new List<string>();
            int j = 0, k = -1;
            string s = "";
            // enforce proper tier access
            int.TryParse(Root.FindNode("Level", null).Value, out j);
            if (j > 0)
            {
                if ((fi.Tier == "Epic") && (j < 21)) return false;
                else if ((fi.Tier == "Paragon") && (j < 11)) return false;
            }

            return MeetsPrerequisites(fi.Prerequisites);
        }

        public bool IsUsingFeature(string FeatureType, FeatureInfo cfi)
        {
            string source = "";

            if (cfi.Type == "text") source = FeatureType;
            else if (cfi.Type == "feat") source = "Feats";
            else if (cfi.Type == "ritual") source = "Rituals";
            else if (cfi.Type == "power")
            {
                PowerInfo pi = Powers.Find(delegate(PowerInfo p) { return p.Name == cfi.Name; });
                if (pi == null) return false;
                if (pi.Type == "Utility") source = "UtilityPowers";
                else source = pi.Frequency + "Powers";
            }
            else if (cfi.Type == "class")
            {
                ClassNode cn = SF.data.Root.FindNode("Class", null) as ClassNode;
                if (cn == null) return false;
                for (int i = 1; i < cn.Classes.Count; i++)
                {
                    if (cfi.Name == cn.Classes[i]) return true;
                }
                return false;
            }

			/*PowerManagerNode n = Root.FindNode(source, null) as PowerManagerNode;
            string s = n.Powers.Find(delegate(string fn) { return fn == cfi.Name; });
            if (s == cfi.Name) return true;
            else return false;*/
			return true;
        }

        public void GiveFeature(string FeatureType, FeatureInfo cfi)
        {
           /* if ((cfi.Type == "text") && (FeatureType != "")) (Root.FindNode(FeatureType, null) as PowerManagerNode).InsertPower(0, cfi.Name, cfi.Name, true);
            else if (cfi.Type == "feat") (Root.FindNode("Feats", null) as PowerManagerNode).InsertPower(-1, cfi.Name, cfi.Name, true);
            else if (cfi.Type == "ritual") (Root.FindNode("Rituals", null) as PowerManagerNode).InsertPower(-1, cfi.Name, cfi.Name, true);
            else if (cfi.Type == "power")
            {
                PowerInfo pi = SF.data.Powers.Find(p => p.Name == cfi.Name);
                if (pi == null) return;
                if (pi.Type == "Utility") (Root.FindNode("UtilityPowers", null) as PowerManagerNode).InsertPower(-1, cfi.Name, cfi.Name, true);
                else (Root.FindNode(pi.Frequency + "Powers", null) as PowerManagerNode).InsertPower(-1, cfi.Name, cfi.Name, true);
            }
            else */if (cfi.Type == "class")
            {
                if (IsUsingFeature("", cfi)) return;
                ClassNode cn = Root.FindNode("Class", null) as ClassNode;
                if (cn == null) return;
                cn.Classes.Add(cfi.Name);
            }
            else if ((cfi.Type == "skill") || (cfi.Type == "speed") || (cfi.Type == "defense") || (cfi.Type == "initiative"))
            {
                NumberTextNode ntn = SF.data.Root.FindNode(cfi.Name + "MiscMod", null) as NumberTextNode;
                if (ntn == null) return;
                int i = 0;
                int.TryParse(ntn.Value, out i);
                int v = 0;
                int.TryParse(cfi.Description, out v);
                i += v;
                ntn.Value = i.ToString();
            }
            else if (cfi.Type == "sense")
            {
                StringListNode sln = SF.data.Root.FindNode("ExtraSenses", null) as StringListNode;
                if (sln == null) return;
                if (!sln.HasString(cfi.Name)) sln.AddString(cfi.Name, true);
            }
            else if (cfi.Type == "trained")
            {
                SkillTrainedNode stn = SF.data.Root.FindNode(cfi.Name + "Trained", null) as SkillTrainedNode;
                if (stn == null) return;
                stn.Value = "1";
            }
			else if (cfi.Type == "untrained")
			{
				UntrainedSkillBonusNode usbn;
				foreach (SkillInfo si in Skills)
				{
					usbn = Root.FindNode(si.Name + "UntrainedBonus", null) as UntrainedSkillBonusNode;
					if (usbn == null) continue;
					int v = 0;
					int.TryParse(cfi.Description, out v);
                    usbn.Bonus += v;
                    usbn.Update(++SF.data.UpdateID);
				}
			}

            return;
        }

        public void RemoveFeature(string FeatureType, FeatureInfo cfi)
        {
            /*if (cfi.Type == "text") (Root.FindNode(FeatureType, null) as PowerManagerNode).RemovePower(cfi.Name, false);
            else if (cfi.Type == "feat") (Root.FindNode("Feats", null) as PowerManagerNode).RemovePower(cfi.Name, false);
            else if (cfi.Type == "ritual") (Root.FindNode("Rituals", null) as PowerManagerNode).RemovePower(cfi.Name, false);
            else if (cfi.Type == "power")
            {
                // we have to hit all of them since we don't know which the power falls under
                (Root.FindNode("At-WillPowers", null) as PowerManagerNode).RemovePower(cfi.Name, false);
                (Root.FindNode("EncounterPowers", null) as PowerManagerNode).RemovePower(cfi.Name, false);
                (Root.FindNode("DailyPowers", null) as PowerManagerNode).RemovePower(cfi.Name, false);
                (Root.FindNode("UtilityPowers", null) as PowerManagerNode).RemovePower(cfi.Name, false);
            }
            else */if (cfi.Type == "class")
            {
                ClassNode cn = SF.data.Root.FindNode("Class", null) as ClassNode;
                if (cn == null) return;
                for (int i = 1; i < cn.Classes.Count; i++)
                    if (cn.Classes[i] == cfi.Name) cn.Classes.RemoveAt(i--);
            }
            else if ((cfi.Type == "skill") || (cfi.Type == "speed") || (cfi.Type == "defense") || (cfi.Type == "initiative"))
            {
                NumberTextNode ntn = SF.data.Root.FindNode(cfi.Name + "MiscMod", null) as NumberTextNode;
                if (ntn == null) return;
                int i = 0;
                int.TryParse(ntn.Value, out i);
                int v = 0;
                int.TryParse(cfi.Description, out v);
                i -= v;
                ntn.Value = i.ToString();
            }
            else if (cfi.Type == "sense")
            {
                StringListNode sln = SF.data.Root.FindNode("ExtraSenses", null) as StringListNode;
                if (sln == null) return;
                sln.RemoveString(cfi.Name, true);
            }
            else if (cfi.Type == "trained")
            {
                SkillTrainedNode stn = SF.data.Root.FindNode(cfi.Name + "Trained", null) as SkillTrainedNode;
                if (stn == null) return;
                stn.Value = "0";
            }
			else if (cfi.Type == "untrained")
			{
				UntrainedSkillBonusNode usbn;
				foreach (SkillInfo si in Skills)
				{
					usbn = Root.FindNode(si.Name + "UntrainedBonus", null) as UntrainedSkillBonusNode;
					if (usbn == null) continue;
					int v = 0;
					int.TryParse(cfi.Description, out v);
					usbn.Bonus -= v;
					usbn.Update(++SF.data.UpdateID);
				}
			}

            return;
        }
    }
}
