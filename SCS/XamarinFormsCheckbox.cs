using System;
using Xamarin.Forms;

namespace SCS2
{
	public class CheckBox : View
	{
		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsChecked", typeof(bool), typeof(CheckBox), false, propertyChanged: (s, o, n) => { (s as CheckBox).OnChecked(new EventArgs()); });
		public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(CheckBox), defaultValue: Color.Default);

		public bool IsChecked
		{
			get
			{
				return (bool)GetValue(IsCheckedProperty);
			}
			set
			{
				SetValue(IsCheckedProperty, value);
			}
		}

		public Color Color
		{
			get
			{
				return (Color)GetValue(ColorProperty);
			}
			set
			{
				SetValue(ColorProperty, value);
			}
		}

		public event EventHandler Checked;

		protected virtual void OnChecked(EventArgs e)
		{
			Checked?.Invoke(this, e);
		}
	}
}
