using System.Collections.Generic;
using Xamarin.Forms;

namespace SCS2
{
	public class Node
	{
		public string Name;
		protected List<Node> Inputs = new List<Node>();
		protected List<Node> Outputs = new List<Node>();
		private int ProcessID = 0;
		//		private int UpdateID = 0;
		private int StrictID = 0;
		public View V;

		public Node(string n, View v)
		{
			Name = n;
			V = v;
		}

		public virtual string Value { get; set; }

		public Node FindNode(string sn, View fv)
		{
			if ((sn == "") && (fv == null)) return null;

			// first scan to see if one of our outputs is being looked for
			for (int i = 0; i < Outputs.Count; i++)
			{
				if ((sn != "") && (Outputs[i].Name == sn)) return Outputs[i];
				if ((fv != null) && (Outputs[i].V == fv)) return Outputs[i];
			}

			// nope, tell all outputs to look
			Node n = null;
			for (int i = 0; i < Outputs.Count; i++)
			{
				n = Outputs[i].FindNode(sn, fv);
				if (n != null) return n;
			}

			return null;
		}

		public void LinkToIn(Node n)
		{
			if (n == null) return;
			RegisterInput(n);
			n.RegisterOutput(this);
		}

		public void LinkToOut(Node n)
		{
			RegisterOutput(n);
			n.RegisterInput(this);
		}

		public void RegisterInput(Node n)
		{
			// ensure we aren't duplicating entries
			for (int i = 0; i < Inputs.Count; i++)
			{
				if (Inputs[i] == n) return;
			}

			Inputs.Add(n);
		}

		public void RegisterOutput(Node n)
		{
			// ensure we aren't duplicating entries
			for (int i = 0; i < Outputs.Count; i++)
			{
				if (Outputs[i] == n) return;
			}

			Outputs.Add(n);
		}

		public void Validate(int pid, List<string> Output)
		{
			// we've already processed, no need to do it again
			if (ProcessID == pid) return;

			// all inputs must be validated before we can do anything reliable
			for (int i = 0; i < Inputs.Count; i++)
				Inputs[i].Validate(pid, Output);

			// this check ensures that we don't get re-entrant validation from an input that we told to validate that then told us to validate
			if (ProcessID == pid) return;

			ProcessID = pid;
			ValidateMe(Output);
			for (int i = 0; i < Outputs.Count; i++)
				Outputs[i].Validate(pid, Output);
		}

		protected virtual void ValidateMe(List<string> Output) { }

		public void Update(int uid)
		{
			// this tells us whether or not any updating has been done
			//if (UpdateID == uid) return;

			//UpdateID = uid;
			UpdateMe();
			foreach (Node n in Outputs) n.Update(uid);
		}

		protected virtual void UpdateMe() { }

		public void Clear()
		{
			ClearMe();
			foreach (Node n in Outputs) n.Clear();
		}

		protected virtual void ClearMe() { }

		public void PostLoad()
		{
			PostLoadMe();
			foreach (Node n in Outputs) n.PostLoad();
		}

		protected virtual void PostLoadMe() { }

		public void SetStrictMode(int sid, bool sm)
		{
			if (StrictID == sid) return;

			StrictID = sid;
			SetStrictModeMe(sm);
			for (int i = 0; i < Outputs.Count; i++)
				SetStrictMode(sid, sm);
		}

		protected virtual void SetStrictModeMe(bool sm) { }
	}

	public class TextNode : Node
	{
		protected Entry _Entry = null;
		protected bool bReadOnly = false;

		public TextNode(string n, Entry e) : base(n, e)
		{
			_Entry = e;
			bReadOnly = _Entry.InputTransparent;

			return;
		}

		protected override void SetStrictModeMe(bool sm)
		{
			if (sm) _Entry.InputTransparent = bReadOnly;
			else _Entry.InputTransparent = false;

			return;
		}

		public override string Value
		{
			get
			{
				if (_Entry == null) return "";
				return _Entry.Text;
			}

			set
			{
				if (_Entry != null) _Entry.Text = value;
			}
		}
	}

	public class PickerNode : Node
	{
		protected Picker _Picker = null;

		public PickerNode(string n, Picker pp) : base(n, pp)
		{
			_Picker = pp;
			return;
		}

		public override string Value
		{
			get
			{
				if (_Picker == null) return "";
				if (_Picker.SelectedIndex == -1) return "";
				return _Picker.SelectedItem.ToString();
			}
		}

		public bool HasValue(string v)
		{
			if (_Picker == null) return false;
			if (string.IsNullOrWhiteSpace(v)) return false;

			for (int i = 0; i < _Picker.Items.Count; i++)
				if (_Picker.Items[i].ToString() == v) return true;

			return false;
		}
	}

	public class StringListNode : Node
	{
		public List<string> List = new List<string>();

		public StringListNode(string n) : base(n, null) { }

		public void AddString(string s, bool b)
		{
			List.Add(s);
			if (b) Update(++SF.data.UpdateID);
		}

		public bool HasString(string s)
		{
			string t = List.Find(ts => ts == s);
			return (t == s);
		}

		public void RemoveString(string s, bool b)
		{
			for (int i = 0; i < List.Count; i++)
				if (List[i] == s) List.RemoveAt(i--);

			if (b) Update(++SF.data.UpdateID);
		}

		protected override void ClearMe()
		{
			List.Clear();
		}
	}

	public class CheckNode : Node
	{
		protected CheckBox _CB = null;

		public CheckNode(string n, CheckBox cb) : base(n, cb)
		{
			_CB = cb;
		}

		public override string Value
		{
			get
			{
				if (_CB == null) return "0";
				if (_CB.IsChecked) return "1";
				else return "0";
			}

			set
			{
				if (_CB == null) return;
				if (value == "0") _CB.IsChecked = false;
				else _CB.IsChecked = true;
			}
		}
	}

	public class NumberTextNode : TextNode
	{
		public NumberTextNode(string n, Entry e) : base(n, e) { }

		protected override void UpdateMe()
		{
			base.UpdateMe();

			if (_Entry == null) return;
			if (Inputs.Count == 0) return;
			if (Inputs[0] == SF.data.Root) return;

			int i = 0;
			for (int it = 0; it < Inputs.Count; it++)
			{
				int itt = 0;
				int.TryParse(Inputs[it].Value, out itt);
				i += itt;
			}

			_Entry.Text = i.ToString();
		}
	}

	public class NumberNode : Node
	{
		protected int i = 0;

		public NumberNode(string n) : base(n, null) { }

		public override string Value
		{
			get { return i.ToString(); }
		}
	}

	public class NumberAddNode : NumberNode
	{
		public NumberAddNode(string n) : base(n) { }

		protected override void UpdateMe()
		{
			i = 0;
			for (int it = 0; it < Inputs.Count; it++)
			{
				int itt = 0;
				int.TryParse(Inputs[it].Value, out itt);
				i += itt;
			}
		}
	}

	public class NumberMaxNode : NumberNode
	{
		private int max = 0;

		public NumberMaxNode(string n, int m) : base(n)
		{
			max = m;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;
			int it = 0;
			i = max;
			for (int itt = 0; itt < Inputs.Count; itt++)
			{
				int.TryParse(Inputs[itt].Value, out it);
				if (it > i) i = it;
			}
		}
	}

	public class NumberMinNode : NumberNode
	{
		private int min = 0;

		public NumberMinNode(string n, int m) : base(n)
		{
			min = m;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;
			int it = 0;
			i = min;
			for (int itt = 0; itt < Inputs.Count; itt++)
			{
				int.TryParse(Inputs[itt].Value, out it);
				if (itt < i) i = itt;
			}
		}
	}

	public class NumberClampNode : NumberNode
	{
		private int min = 0, max = 0;

		public NumberClampNode(int n, int x) : base("")
		{
			min = n;
			max = x;
			if (min > max) min = max;
			if (max < min) max = min;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count == 0) return;
			int.TryParse(Inputs[0].Value, out i);
			if (i < min) i = min;
			if (i > max) i = max;
		}
	}

	public class NumberMultiplyNode : NumberNode
	{
		public NumberMultiplyNode(string n) : base(n) { }

		protected override void UpdateMe()
		{
			i = 0;
			if (Inputs.Count < 2) return;
			i = 1;
			for (int it = 0; it < Inputs.Count; it++)
			{
				int itt = 0;
				int.TryParse(Inputs[it].Value, out itt);
				i *= itt;
			}
		}
	}

	public class NumberAddByNode : NumberNode
	{
		private int AddBy = 0;

		public NumberAddByNode(int ab) : base("")
		{
			AddBy = ab;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count < 1) return;
			int.TryParse(Inputs[0].Value, out i);
			i += AddBy;
		}
	}

	public class NumberSubtractByNode : NumberNode
	{
		private int SubBy = 0;

		public NumberSubtractByNode(int sb) : base("")
		{
			SubBy = sb;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count < 1) return;
			int.TryParse(Inputs[0].Value, out i);
			i -= SubBy;
		}
	}

	public class NumberMultiplyByNode : NumberNode
	{
		private int MulBy = 0;

		public NumberMultiplyByNode(int mb) : base("")
		{
			MulBy = mb;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count < 1) return;
			int.TryParse(Inputs[0].Value, out i);
			i += MulBy;
		}
	}

	public class NumberDivideByNode : NumberNode
	{
		private int DivBy = 0;

		public NumberDivideByNode(int db) : base("")
		{
			DivBy = db;
		}

		protected override void UpdateMe()
		{
			if (Inputs.Count < 1) return;
			int.TryParse(Inputs[0].Value, out i);
			i /= DivBy;
		}
	}
}
