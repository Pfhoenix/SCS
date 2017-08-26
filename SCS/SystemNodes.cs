using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace SCS2
{
	public class RaceNode : PickerNode
	{
		public List<string> Races = new List<string>();

		public RaceNode(string n, Picker p) : base(n, p) { }

		protected override void UpdateMe()
		{
			string r = Value;
			if (r == "")
			{
				Races = new List<string>();
				return;
			}

			for (int i = 0; i < SF.data.Races.Count; i++)
			{
				if (SF.data.Races[i].Races[0] == r)
				{
					Races = SF.data.Races[i].Races;
					break;
				}
			}
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class ClassNode : PickerNode
	{
		public List<string> Classes = new List<string>();

		public ClassNode(string n, Picker p) : base(n, p) { }

		protected override void UpdateMe()
		{
			string c = Value;
			if (c == "")
			{
				Classes.Clear();
				return;
			}

			if (Classes.Count == 0) Classes.Add(c);
			else Classes[0] = c;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class AbilityModNode : NumberTextNode
	{
		public AbilityModNode(string n, Entry e) : base(n, e) { }

		protected override void UpdateMe()
		{
			base.UpdateMe();

			int i = int.Parse(Value);

			i = -5 + i / 2;

			_Entry.Text = i.ToString();
		}
	}

	public class HalfLevelNode : NumberAddNode
	{
		public HalfLevelNode() : base("HalfLevel") { }

		protected override void UpdateMe()
		{
			base.UpdateMe();

			if (i < 0) i = 0;
			i = (i / 2);
		}
	}

	public class ParagonPathNode : PickerNode
	{
		private bool bUpdating = false;

		public ParagonPathNode(string n, Picker p) : base(n, p) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count < 1) return;

			if (bUpdating) return;
			bUpdating = true;

			// enforce level
			int l = 0;
			int.TryParse(Inputs[0].Value, out l);

			string cp = (_Picker.SelectedItem == null ? "" : _Picker.SelectedItem.ToString());
			_Picker.Items.Clear();
			_Picker.Items.Add("");
			if ((l == 0) || (l > 10))
			{
				foreach (FeatureSetInfo pi in SF.data.Paragons)
					if (SF.data.MeetsPrerequisites(pi.Prerequisites))
					{
						_Picker.Items.Add(pi.Name);
						if (cp == pi.Name) _Picker.SelectedItem = cp;
					}
			}

			bUpdating = false;
		}
	}

	public class EpicDestinyNode : PickerNode
	{
		private bool bUpdating = false;

		public EpicDestinyNode(string n, Picker p) : base(n, p) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count < 1) return;

			if (bUpdating) return;
			bUpdating = true;

			// enforce level
			int l = 0;
			int.TryParse(Inputs[0].Value, out l);

			string cp = (_Picker.SelectedItem == null ? "" : _Picker.SelectedItem.ToString());
			_Picker.Items.Clear();
			_Picker.Items.Add("");
			if ((l == 0) || (l > 10))
			{
				foreach (FeatureSetInfo pi in SF.data.Epics)
					if (SF.data.MeetsPrerequisites(pi.Prerequisites))
					{
						_Picker.Items.Add(pi.Name);
						if (cp == pi.Name) _Picker.SelectedItem = cp;
					}
			}

			bUpdating = false;
		}
	}

	public class SkillTrainedNode : CheckNode
	{
		protected bool bProcess = true;
		protected bool ClassTrained = false;

		public SkillTrainedNode(string n, CheckBox ccb) : base(n, ccb) { }

		protected override void UpdateMe()
		{
			if (!bProcess) return;
			if (Inputs.Count < 2) return;
			bProcess = false;
			string s = Name.Substring(0, Name.IndexOf("Trained"));
			// determine if c is a valid class, and if so, determine if our associated skill (s) is trained or a class skill (or both)
			int j = 0;
			ClassInfo ci = SF.data.Classes.Find(c => c.Name == Inputs[1].Value);
			if (ci != null)
			{
				if (ci.TrainedSkills.Exists(ts => ts == s))
				{
					_CB.IsChecked = true;
					ClassTrained = true;
				}
				else if ((ci.ClassSkills.Exists(ts => ts == s)) && (Inputs[0].Value != "0"))
				{
					_CB.IsChecked = true;
					ClassTrained = true;
				}
				else if (ClassTrained)
				{
					_CB.IsChecked = false;
					ClassTrained = false;
				}
			}
			else
			{
				if (ClassTrained) _CB.IsChecked = false;
				ClassTrained = false;
			}

			bProcess = true;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
			return;
		}

		public override string Value
		{
			get
			{
				if (_CB == null) return "0";
				if (_CB.IsChecked) return "5";
				else return "0";
			}
		}
	}

	public class ClassSkillTrainedNode : CheckNode
	{
		protected bool bProcess = true;
		protected string CurClass = "";

		public ClassSkillTrainedNode(string n, CheckBox ccb) : base(n, ccb)
		{
			return;
		}

		protected override void UpdateMe()
		{
			if (!bProcess) return;
			if (Inputs.Count == 0) return;

			if (Inputs[0].Value != CurClass)
			{
				CurClass = Inputs[0].Value;
				bProcess = false;
				_CB.IsChecked = false;
				bProcess = true;
			}

			return;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
			return;
		}
	}

	public class ClassHPBaseNode : Node
	{
		private int HPBase = 0;

		public ClassHPBaseNode(string n) : base(n, null) { }

		public override string Value
		{
			get { return HPBase.ToString(); }
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;
			HPBase = 0;
			string s = Inputs[0].Value;
			ClassInfo ci = SF.data.Classes.Find(c => c.Name == s);
			if (ci != null) HPBase = ci.HPBase;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class ClassHPLevelNode : Node
	{
		private int HPLevel = 0;

		public ClassHPLevelNode(string n) : base(n, null) { }

		public override string Value
		{
			get { return HPLevel.ToString(); }
		}

		protected override void UpdateMe()
		{
			base.UpdateMe();
			if (Inputs.Count == 0) return;
			HPLevel = 0;
			string s = Inputs[0].Value;
			ClassInfo ci = SF.data.Classes.Find(c => c.Name == s);
			if (ci != null) HPLevel = ci.HPLevel;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class ClassSurgesNode : Node
	{
		private int Surges = 0;

		public ClassSurgesNode(string n) : base(n, null) { }

		public override string Value
		{
			get { return Surges.ToString(); }
		}

		protected override void UpdateMe()
		{
			base.UpdateMe();
			if (Inputs.Count == 0) return;
			Surges = 0;
			string s = Inputs[0].Value;
			ClassInfo ci = SF.data.Classes.Find(c => c.Name == s);
			if (ci != null) Surges = ci.Surges;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class ClassDefenseBonusNode : Node
	{
		private string Defense = "";
		private int Bonus = 0;

		public ClassDefenseBonusNode(string n, string d) : base(n, null)
		{
			Defense = d;
			return;
		}

		public override string Value
		{
			get { return Bonus.ToString(); }
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			Bonus = 0;
			string s = Inputs[0].Value;
			ClassInfo ci = SF.data.Classes.Find(c => c.Name == s);
			if (ci != null)
			{
				int ss = -1;
				for (int j = 0; j < ci.DefenseBonuses.Count; j++)
				{
					ss = ci.DefenseBonuses[j].IndexOf(Defense);
					if (ss > 0)
					{
						int.TryParse(ci.DefenseBonuses[j].Substring(0, ss - 1), out Bonus);
						return;
					}
				}
			}

			return;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
			return;
		}
	}

	/*public class ClassProficiencyNode : Node
	{
		private List<string> Profs = new List<string>();

		public ClassProficiencyNode() : base("", null)
		{
			return;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			// find our current class
			ClassNode n = SF.data.Root.FindNode("Class", null) as ClassNode;
			if ((n == null) || (n.Classes.Count == 0)) return;
			PowerManagerNode feats = SF.data.Root.FindNode("Feats", null) as PowerManagerNode;
			if (feats == null) return;
			ClassInfo ci = null;
			ci = SF.data.Classes.Find(c => c.Name == n.Classes[0]);
			if ((ci == null) || (ci.Name == "") || (ci.Name == null)) return;
			// build single list of armor and shield proficiencies
			List<string> all = new List<string>();
			for (int i = 0; i < ci.ArmorProficiencies.Count; i++)
				all.Add("Armor Proficiency (" + ci.ArmorProficiencies[i] + ")");
			for (int i = 0; i < ci.ShieldProficiencies.Count; i++)
				all.Add("Shield Proficiency (" + ci.ShieldProficiencies[i] + ")");
			// remove all current proficiency feats and insert new list
			for (int i = 0; i < Profs.Count; i++) feats.RemovePower(Profs[i], false);
			for (int i = 0; i < all.Count; i++) feats.InsertPower(i, all[i], all[i], true);
			Profs.Clear();
			Profs.AddRange(all);

			return;
		}

		protected override void PostLoadMe()
		{
			if (Inputs.Count == 0) return;

			// find our current class
			ClassNode n = SF.data.Root.FindNode("Class", null) as ClassNode;
			if ((n == null) || (n.Classes.Count == 0)) return;
			PowerManagerNode feats = SF.data.Root.FindNode("Feats", null) as PowerManagerNode;
			if (feats == null) return;
			ClassInfo ci = null;
			ci = SF.data.Classes.Find(c => c.Name == n.Classes[0]);
			if ((ci == null) || (ci.Name == "") || (ci.Name == null)) return;
			// build single list of armor and shield proficiencies
			List<string> all = new List<string>();
			for (int i = 0; i < ci.ArmorProficiencies.Count; i++)
				all.Add("Armor Proficiency (" + ci.ArmorProficiencies[i] + ")");
			for (int i = 0; i < ci.ShieldProficiencies.Count; i++)
				all.Add("Shield Proficiency (" + ci.ShieldProficiencies[i] + ")");
			Profs.Clear();
			Profs.AddRange(all);
		}
	}*/

	/*public class ClassFeaturesNode : Node
	{
		private string CurClass = "";

		public ClassFeaturesNode() : base("", null) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			// find the current class
			Node n = Inputs[0];
			if (n == null) return;
			PowerManagerNode features = SF.data.Root.FindNode("Features", null) as PowerManagerNode;
			if (features == null) return;
			ClassInfo cci = null, pci = null;
			// get currently selected class info
			cci = SF.data.Classes.Find(c => c.Name == n.Value);
			if ((cci != null) && ((cci.Name == null) || (cci.Name == "") || (cci.Name == CurClass))) return;
			// get previously selected class info
			pci = SF.data.Classes.Find(c => c.Name == CurClass);
			// remove all previous class features
			if (pci != null)
			{
				foreach (FeatureInfo c in pci.Features)
					SF.data.RemoveFeature("Features", c);
			}
			// add all current class features
			if (cci != null)
			{
				foreach (FeatureInfo c in cci.Features)
					if (c.OptionGroup == "") SF.data.GiveFeature("Features", c);

				CurClass = cci.Name;
			}
			else CurClass = "";
		}

		protected override void PostLoadMe()
		{
			if (Inputs.Count > 0) CurClass = Inputs[0].Value;
		}
	}*/

	/*public class ParagonFeaturesNode : Node
	{
		private string CurParagon = "";

		public ParagonFeaturesNode() : base("", null) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			// find the current class
			Node n = Inputs[0];
			if (n == null) return;
			PowerManagerNode features = SF.data.Root.FindNode("Features", null) as PowerManagerNode;
			if (features == null) return;
			FeatureSetInfo cpi = null, ppi = null;
			// get currently selected paragon info
			cpi = SF.data.Paragons.Find(p => p.Name == n.Value);
			// we allow reissuing of features from a paragon path already selected because level needs to be taken into account
			if ((cpi != null) && ((cpi.Name == null) || (cpi.Name == ""))) return;
			// get previously selected paragon info
			ppi = SF.data.Paragons.Find(p => p.Name == CurParagon);
			// remove all previous class features
			if (ppi != null)
			{
				foreach (FeatureInfo c in ppi.Features)
					SF.data.RemoveFeature("Features", c);
			}
			// add all current class features
			if (cpi != null)
			{
				foreach (FeatureInfo c in cpi.Features)
					if ((c.OptionGroup == "") && (SF.data.MeetsLevelQualification(c.Level))) SF.data.GiveFeature("Features", c);

				CurParagon = cpi.Name;
			}
			else CurParagon = "";
		}

		protected override void PostLoadMe()
		{
			if (Inputs.Count > 0) CurParagon = Inputs[0].Value;
		}
	}*/

	/*public class EpicFeaturesNode : Node
	{
		private string CurEpic = "";

		public EpicFeaturesNode() : base("", null) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			// find the current class
			Node n = Inputs[0];
			if (n == null) return;
			PowerManagerNode features = SF.data.Root.FindNode("Features", null) as PowerManagerNode;
			if (features == null) return;
			FeatureSetInfo cpi = null, ppi = null;
			// get currently selected paragon info
			cpi = SF.data.Epics.Find(p => p.Name == n.Value);
			// we allow reissuing of features from a paragon path already selected because level needs to be taken into account
			if ((cpi != null) && ((cpi.Name == null) || (cpi.Name == ""))) return;
			// get previously selected paragon info
			ppi = SF.data.Epics.Find(p => p.Name == CurEpic);
			// remove all previous class features
			if (ppi != null)
			{
				foreach (FeatureInfo c in ppi.Features)
					SF.data.RemoveFeature("Features", c);
			}
			// add all current class features
			if (cpi != null)
			{
				foreach (FeatureInfo c in cpi.Features)
					if ((c.OptionGroup == "") && (SF.data.MeetsLevelQualification(c.Level))) SF.data.GiveFeature("Features", c);

				CurEpic = cpi.Name;
			}
			else CurEpic = "";
		}

		protected override void PostLoadMe()
		{
			if (Inputs.Count > 0) CurEpic = Inputs[0].Value;
		}
	}*/

	/*public class RaceFeaturesNode : Node
	{
		private string CurRace = "";

		public RaceFeaturesNode() : base("", null) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			// find the current class
			Node n = Inputs[0];
			if (n == null) return;
			PowerManagerNode features = SF.data.Root.FindNode("RaceFeatures", null) as PowerManagerNode;
			if (features == null) return;
			RaceInfo cri = null, pri = null;
			// get currently selected class info
			cri = SF.data.Races.Find(r => r.Name == n.Value);
			if ((cri != null) && ((cri.Name == null) || (cri.Name == "") || (cri.Name == CurRace))) return;
			// get previously selected class info
			pri = SF.data.Races.Find(r => r.Name == CurRace);
			// remove all previous class features
			if (pri != null)
			{
				foreach (FeatureInfo c in pri.RaceFeatures)
					SF.data.RemoveFeature("RaceFeatures", c);
			}
			// add all current class features
			if (cri != null)
			{
				foreach (FeatureInfo c in cri.RaceFeatures)
					if (c.OptionGroup == "") SF.data.GiveFeature("RaceFeatures", c);

				CurRace = cri.Name;
			}
			else CurRace = "";
		}

		protected override void PostLoadMe()
		{
			if (Inputs.Count > 0) CurRace = Inputs[0].Value;
		}
	}*/

	public class RaceSizeNode : TextNode
	{
		public RaceSizeNode(string n, Entry e) : base(n, e) { }

		protected override void UpdateMe()
		{
			base.UpdateMe();
			if (Inputs.Count == 0) return;
			string s = Inputs[0].Value;
			_Entry.Text = "";
			for (int i = 0; i < SF.data.Races.Count; i++)
			{
				if (SF.data.Races[i].Name == s)
				{
					_Entry.Text = SF.data.Races[i].Size;
					return;
				}
			}
		}
	}

	public class RaceAbilityBonusNode : Node
	{
		private string Ability = "";
		private int Bonus = 0;

		public RaceAbilityBonusNode(string n, string a) : base(n, null)
		{
			Ability = a;
		}

		public override string Value
		{
			get { return Bonus.ToString(); }
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			Bonus = 0;
			string s = Inputs[0].Value;
			RaceInfo ri = SF.data.Races.Find(r => r.Name == s);
			if (ri != null)
			{
				int ss = -1;
				for (int j = 0; j < ri.AbilityBonuses.Count; j++)
				{
					ss = ri.AbilityBonuses[j].IndexOf(Ability);
					if (ss > 0)
					{
						int.TryParse(ri.AbilityBonuses[j].Substring(0, ss - 1), out Bonus);
						return;
					}
				}
			}
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class RaceOptionalAbilityBonusNode : TextNode
	{
		protected string CurRace = "";

		public RaceOptionalAbilityBonusNode(string n, Entry e) : base(n, e) { }

		protected override void UpdateMe()
		{
			base.UpdateMe();

			if (Inputs.Count == 0) return;

			if (Inputs[0].Value != CurRace)
			{
				CurRace = Inputs[0].Value;
				_Entry.Text = "0";
			}
		}

		protected override void PostLoadMe()
		{
			base.PostLoadMe();

			if (Inputs.Count == 0) return;

			CurRace = Inputs[0].Value;
		}
	}

	public class RaceSkillBonusNode : Node
	{
		private string Skill = "";
		private int Bonus = 0;

		public RaceSkillBonusNode(string n, string sk) : base(n, null)
		{
			Skill = sk;
		}

		public override string Value
		{
			get { return Bonus.ToString(); }
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			Bonus = 0;
			string s = Inputs[0].Value;
			RaceInfo ri = SF.data.Races.Find(r => r.Name == s);
			if (ri != null)
			{
				int ss = -1;
				for (int j = 0; j < ri.SkillBonuses.Count; j++)
				{
					ss = ri.SkillBonuses[j].IndexOf(Skill);
					if (ss > 0)
					{
						int.TryParse(ri.SkillBonuses[j].Substring(0, ss - 1), out Bonus);
						return;
					}
				}
			}
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class RaceDefenseBonusNode : Node
	{
		private string Defense = "";
		private int Bonus = 0;

		public RaceDefenseBonusNode(string n, string d) : base(n, null)
		{
			Defense = d;
		}

		public override string Value
		{
			get { return Bonus.ToString(); }
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			Bonus = 0;
			string s = Inputs[0].Value;
			RaceInfo ri = SF.data.Races.Find(r => r.Name == s);
			if (ri != null)
			{
				int ss = -1;
				for (int j = 0; j < ri.DefenseBonuses.Count; j++)
				{
					ss = ri.DefenseBonuses[j].IndexOf(Defense);
					if (ss > 0)
					{
						int.TryParse(ri.DefenseBonuses[j].Substring(0, ss - 1), out Bonus);
						return;
					}
				}
			}
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class RaceLanguageNode : Node
	{
		private string Languages = "";

		public RaceLanguageNode(string n) : base(n, null) { }

		public override string Value
		{
			get { return Languages; }
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			Languages = "";

			string s = Inputs[0].Value;
			RaceInfo ri = SF.data.Races.Find(r => r.Name == s);
			if (ri != null)
			{
				for (int j = 0; j < ri.Languages.Count; j++)
				{
					if (j != 0) Languages += ",";
					Languages += ri.Languages[j];
				}
			}
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class RaceSpeedNode : Node
	{
		private int speed = 0;

		public RaceSpeedNode(string n) : base(n, null) { }

		public override string Value
		{
			get { return speed.ToString(); }
		}

		protected override void UpdateMe()
		{
			base.UpdateMe();
			if (Inputs.Count == 0) return;
			string s = Inputs[0].Value;
			RaceInfo ri = SF.data.Races.Find(r => r.Name == s);
			if (ri != null) speed = ri.Speed;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class RaceVisionNode : Node
	{
		private string vision = "";

		public RaceVisionNode(string n) : base(n, null) { }

		public override string Value
		{
			get { return vision; }
		}

		protected override void UpdateMe()
		{
			base.UpdateMe();
			if (Inputs.Count == 0) return;
			string s = Inputs[0].Value;
			RaceInfo ri = SF.data.Races.Find(r => r.Name == s);
			if (ri != null) vision = ri.Vision;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}

	public class SpecialSensesNode : TextNode
	{
		public SpecialSensesNode(string n, Entry e) : base(n, e) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;
			List<string> Senses = new List<string>();
			StringListNode sln;
			for (int i = 0; i < Inputs.Count; i++)
			{
				if ((Inputs[i].Value != "") && (Inputs[i].Value != "Normal")) Senses.Add(Inputs[i].Value);
				else
				{
					sln = Inputs[i] as StringListNode;
					if (sln != null)
						foreach (string s in sln.List)
							Senses.Add(s);
				}
			}
			if (Senses.Count == 0) _Entry.Text = "";
			else
			{
				_Entry.Text = "";
				for (int i = 0; i < Senses.Count; i++)
				{
					if (_Entry.Text != "") _Entry.Text += ", ";
					_Entry.Text += Senses[i];
				}
			}
		}
	}

	public class LanguagesNode : TextNode
	{
		public LanguagesNode(string n, Entry e) : base(n, e) { }

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;

			string[] l = Inputs[0].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			_Entry.Text = "";
			for (int i = 0; i < l.Length; i++)
			{
				if (i != 0) _Entry.Text += ", ";
				_Entry.Text += l[i];
			}
			if (_Entry.Text != "") _Entry.Text += Environment.NewLine;
		}
	}

	/*public class PowerManagerNode : Node
	{
		protected custom_listbox clb = null;
		public List<string> Powers = new List<string>();
		public List<string> Added = new List<string>();
		public List<string> Removed = new List<string>();

		public PowerManagerNode(string n, custom_listbox cclb) : base(n, cclb)
		{
			clb = cclb;
			return;
		}

		protected override void UpdateMe()
		{
			Added.Clear();
			Removed.Clear();
			// we need to compare Powers against what's currently listed in clb
			int i = 0, j = 0;
			// it's necessary to point out that additions to clb happen strictly at the end while deletions can be anywhere
			// therefore, if we get a mismatch, it's because something was deleted
			// when the two lists don't match, and clb.Items.Count > Powers.Count, we clearly have additions
			while ((i < Powers.Count) && (i < clb.Items.Count))
			{
				// power was removed
				if (Powers[i] != (clb.Items[i] as custom_listitem).text)
				{
					Removed.Add(Powers[i]);
					Powers.RemoveAt(i);
				}
				else i++;
			}
			// power was removed from the end
			if (Powers.Count > clb.Items.Count)
			{
				while (Powers.Count != clb.Items.Count)
				{
					Removed.Add(Powers[Powers.Count - 1]);
					Powers.RemoveAt(Powers.Count - 1);
				}
			}
			// power was added to the end
			else if (Powers.Count < clb.Items.Count)
			{
				for (i = Powers.Count; i < clb.Items.Count; i++)
				{
					Added.Add((clb.Items[i] as custom_listitem).text);
					Powers.Add((clb.Items[i] as custom_listitem).text);
				}
			}

			return;
		}

		protected override void PostLoadMe()
		{
			if (clb == null) return;

			Powers.Clear();
			for (int i = 0; i < clb.Items.Count; i++)
				Powers.Add((clb.Items[i] as custom_listitem).text);

			return;
		}

		public bool HasValue(string t, string v)
		{
			if (clb == null) return false;
			if ((t == "") && (v == "")) return false;

			for (int i = 0; i < clb.Items.Count; i++)
			{
				if ((t != "") && ((clb.Items[i] as custom_listitem).text == t)) return true;
				if ((v != "") && ((clb.Items[i] as custom_listitem).value == v)) return true;
			}

			return false;
		}

		public virtual void InsertPower(int p, string t, string v, bool b)
		{
			if (clb == null) return;
			if (p == -1) clb.AddItem(t, v, false, b);
			else clb.InsertItem(p, t, v, false, b);

			return;
		}

		public int GetIndexFor(string s)
		{
			if (clb == null) return -1;

			for (int i = 0; i < clb.Items.Count; i++)
				if ((clb.Items[i] as custom_listitem).text == s) return i;

			return -1;
		}

		public virtual void RemovePower(string s, bool b)
		{
			int i = GetIndexFor(s);
			if (i == -1) return;
			clb.Items.RemoveAt(i);

			return;
		}
	}*/

	/*public class FeatManagerNode : PowerManagerNode
	{
		public FeatManagerNode(string n, custom_listbox cclb) : base(n, cclb)
		{
			return;
		}

		protected override void UpdateMe()
		{
			base.UpdateMe();

			FeatInfo fi;

			// ensure added feats get processed properly
			foreach (string s in Added)
			{
				fi = SF.data.Feats.Find(f => f.Name == s);
				if (fi == null) continue;
				foreach (FeatureInfo ffi in fi.Features)
					SF.data.GiveFeature("", ffi);
			}

			// ensure removed feats get processed properly
			foreach (string s in Removed)
			{
				fi = SF.data.Feats.Find(f => f.Name == s);
				if (fi == null) continue;
				foreach (FeatureInfo ffi in fi.Features)
					SF.data.RemoveFeature("", ffi);
			}

			return;
		}

		protected override void PostLoadMe()
		{
			// ensure Powers is populated
			base.PostLoadMe();

			// get a list of all currently used feats that have features that need processing
			var fts = from f in SF.data.Feats
					  where Powers.Exists(n => n == f.Name)
					  select (from ft in f.Features
							  where ft.Type == "untrained"
							  select ft).ToList();

			if (fts.Count() == 0) return;
			List<UntrainedSkillBonusNode> usbns = new List<UntrainedSkillBonusNode>();
			foreach (SkillInfo si in SF.data.Skills)
			{
				UntrainedSkillBonusNode usbn = SF.data.Root.FindNode(si.Name + "UntrainedBonus", null) as UntrainedSkillBonusNode;
				if (usbn != null) usbns.Add(usbn);
			}
			int i = 0, j = 0;
			foreach (var ft in fts)
				foreach (var f in ft)
				{
					if (f.Type == "untrained")
					{
						int.TryParse(f.Description, out i);
						foreach (UntrainedSkillBonusNode u in usbns)
							u.Bonus += i;
					}
				}

			return;
		}
	}*/

	/*public class FeatTierHPNode : Node
	{
		protected int hp;

		public FeatTierHPNode(string n) : base(n, null)
		{
			return;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count < 2) return;
			FeatManagerNode fmn = Inputs[0] as FeatManagerNode;
			if (fmn == null) return;
			Node ln = Inputs[1];
			if (ln == null) return;
			hp = 0;
			int m = 0;
			int.TryParse(ln.Value, out m);
			if (m == 0) return;
			m = (m - 1) / 10 + 1;
			if (m > 3) m = 3;
			int ti = 0;
			// scan through all current feats looking for those with "tierhp" features
			var feats = SF.data.Feats.FindAll(f => fmn.Powers.Exists(e => e == f.Name) && f.Features.Exists(e => e.Type == "tierhp"));
			foreach (var f in feats)
			{
				var fi = f.Features.FindAll(ft => ft.Type == "tierhp");
				foreach (var fit in fi)
				{
					int.TryParse(fit.Description, out ti);
					hp += ti * m;
				}
			}

			return;
		}

		public override string Value
		{
			get
			{
				return hp.ToString();
			}
			set
			{
				return;
			}
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
			return;
		}
	}*/

	// change this to operate on a single skill, getting the bonus value set by feature code
	public class UntrainedSkillBonusNode : Node
	{
		protected string Skill = "";
		protected bool Trained = false;
		public int Bonus = 0;
		List<SkillInfo> Current = new List<SkillInfo>();

		public UntrainedSkillBonusNode(string n, string s) : base(n, null)
		{
			Skill = s;
		}

		public override string Value
		{
			get
			{
				if (Trained) return "0";
				else return Bonus.ToString();
			}
			set { }
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count < 2) return;

			Trained = Inputs[0].Value != "0";
		}

		protected override void ClearMe()
		{
			Bonus = 0;
		}

		protected override void PostLoadMe()
		{
			UpdateMe();
		}
	}
}
