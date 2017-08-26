using System;
using System.ComponentModel;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SCS2;

[assembly: ExportRenderer(typeof(SCS2.CheckBox), typeof(SCS2.Droid.CheckboxRenderer))]
namespace SCS2.Droid
{
	public class CheckboxRenderer : ViewRenderer<SCS2.CheckBox, Android.Widget.CheckBox>
	{
		private Android.Widget.CheckBox checkBox;

		protected override void OnElementChanged(ElementChangedEventArgs<SCS2.CheckBox> e)
		{
			base.OnElementChanged(e);
			var model = e.NewElement;
			checkBox = new Android.Widget.CheckBox(Context);
			checkBox.Tag = this;
			CheckboxPropertyChanged(model, null);
			checkBox.SetOnClickListener(new ClickListener(model));
			SetNativeControl(checkBox);
		}

		private void CheckboxPropertyChanged(SCS2.CheckBox model, String propertyName)
		{
			if (propertyName == null || CheckBox.IsCheckedProperty.PropertyName == propertyName)
			{
				checkBox.Checked = model.IsChecked;
			}

			if (propertyName == null || CheckBox.ColorProperty.PropertyName == propertyName)
			{
				int[][] states =
				{
					new int[] { Android.Resource.Attribute.StateEnabled }, // enabled
                    new int[] { Android.Resource.Attribute.StateEnabled }, // disabled
                    new int[] { Android.Resource.Attribute.StateChecked }, // unchecked
                    new int[] { Android.Resource.Attribute.StatePressed }  // pressed
                };
				var checkBoxColor = (int)model.Color.ToAndroid();
				int[] colors =
				{
					checkBoxColor,
					checkBoxColor,
					checkBoxColor,
					checkBoxColor
				};
				var myList = new Android.Content.Res.ColorStateList(states, colors);
				checkBox.ButtonTintList = myList;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (checkBox != null)
			{
				base.OnElementPropertyChanged(sender, e);

				CheckboxPropertyChanged((CheckBox)sender, e.PropertyName);
			}
		}

		public class ClickListener : Java.Lang.Object, IOnClickListener
		{
			private CheckBox _CheckBox;
			public ClickListener(CheckBox cb)
			{
				_CheckBox = cb;
			}
			public void OnClick(Android.Views.View v)
			{
				_CheckBox.IsChecked = !_CheckBox.IsChecked;
			}
		}
	}
}
