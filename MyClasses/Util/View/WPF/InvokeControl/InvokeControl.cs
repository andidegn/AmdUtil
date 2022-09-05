using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace AMD.Util.View.WPF.InvokeControl
{
	public class InvokeControl
	{
		private Control sender;
		private Dispatcher Dispatcher;

		public InvokeControl(Control sender)
		{
			this.sender = sender;
			this.Dispatcher = sender.Dispatcher;
		}

		public bool InvokeRequired
		{
			get
			{
				return !Dispatcher.Thread.Equals(Thread.CurrentThread);
			}
    }

    /// <summary>
    /// Gets the brush in the WPF resources with the specific key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Brush GetBrush(String key)
    {
      Brush b;
      object res = Application.Current.TryFindResource(key);
      b = res as SolidColorBrush;
      if (b == null)
      {
        b = res as LinearGradientBrush;
      }
      if (b == null)
      {
        b = res as DrawingBrush;
      }
      if (b == null)
      {
        b = res as ImageBrush;
      }
      return b;
    }

    #region Button
    private delegate void DGSetButtonState(bool state, params Button[] Buttons);
		public void SetButtonState(bool state, params Button[] Buttons)
		{
			if (!Dispatcher.Thread.Equals(Thread.CurrentThread))
			{
				Dispatcher.Invoke(new DGSetButtonState(SetButtonState), DispatcherPriority.Render, state, Buttons);
				return;
			}
			foreach (var item in Buttons)
			{
				item.IsEnabled = state;
			}
		}

		private delegate void DGSetButtonContent(object content, params Button[] Buttons);
		public void SetButtonContent(object content, params Button[] Buttons)
		{
			if (!Dispatcher.Thread.Equals(Thread.CurrentThread))
			{
				Dispatcher.Invoke(new DGSetButtonContent(SetButtonContent), DispatcherPriority.Render, content, Buttons);
				return;
			}
			foreach (var item in Buttons)
			{
				item.Content = content;
			}
		}

    public void SetButtonVisibility(Visibility visibility, params Button[] Buttons)
    {
      if (InvokeRequired)
      {
        Dispatcher.Invoke(() => SetButtonVisibility(visibility, Buttons));
        return;
      }
      foreach (var item in Buttons)
      {
        item.Visibility = visibility;
      }
    }
		#endregion // Button

		#region Control
		private delegate void DGSetControlState(bool state, params Control[] Controls);
		public void SetControlState(bool state, params Control[] Controls)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetControlState(SetControlState), DispatcherPriority.Render, state, Controls);
				return;
			}
			foreach (var item in Controls)
			{
				item.IsEnabled = state;
			}
		}

		private delegate bool DGGetControlState(Control control);
		public bool GetControlState(Control control)
		{
			if (InvokeRequired)
			{
				return (bool)Dispatcher.Invoke(new DGGetControlState(GetControlState), DispatcherPriority.Render, control);
			}
			return control.IsEnabled;
		}

		private delegate void DGSetControlVisibility(Visibility visibility, params Control[] Controls);
		public void SetControlVisibility(Visibility visibility, params Control[] Controls)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetControlVisibility(SetControlVisibility), DispatcherPriority.Render, visibility, Controls);
				return;
			}
			foreach (var item in Controls)
			{
				item.Visibility = visibility;
			}
		}

		private delegate Visibility DGGetControlVisibility(Control control);
		public Visibility GetControlVisibility(Control control)
		{
			if (InvokeRequired)
			{
				return (Visibility)Dispatcher.Invoke(new DGGetControlVisibility(GetControlVisibility), DispatcherPriority.Render, control);
			}
			return control.Visibility;
		}

		private delegate void DGSetActive(Control ctrl);
		public void SetActive(Control ctrl)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetActive(SetActive), DispatcherPriority.Render, ctrl);
				return;
			}
			ctrl.Focus();
		}

		private delegate void DGSetControlTemplate(ControlTemplate template, params Control[] Controls);
		public void SetControlTemplate(ControlTemplate template, params Control[] Controls)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetControlTemplate(SetControlTemplate), DispatcherPriority.Render, template, Controls);
				return;
			}
			foreach (var item in Controls)
			{
				item.Template = template;
			}
		}
		#endregion // Control

		#region ComboBox
		private delegate object DGGetSelectedIndex(ComboBox cbb);
		public object GetSelectedIndex(ComboBox cbb)
		{
			if (InvokeRequired)
			{
				return Dispatcher.Invoke(new DGGetSelectedIndex(GetSelectedIndex), DispatcherPriority.Render, cbb);
			}
			return cbb.SelectedItem;
		}
    #endregion // ComboBox

		#region Panel
		private delegate void DGSetBackgrund(Panel panel, Brush brush);
		public void SetBackgrund(Panel panel, Brush brush)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetBackgrund(SetBackgrund), DispatcherPriority.Render, panel, brush);
				return;
			}
			panel.Background = brush;
		}

		private delegate void DGSetPanelVisibility(Visibility visibility, params Panel[] Panels);
		public void SetPanelVisibility(Visibility visibility, params Panel[] Panels)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetPanelVisibility(SetPanelVisibility), DispatcherPriority.Render, visibility, Panels);
				return;
			}
			foreach (var item in Panels)
			{
				item.Visibility = visibility;
			}
		}

		private delegate Visibility DGGetPanelVisibility(Panel Panel);
		public Visibility GetPanelVisibility(Panel panel)
		{
			if (InvokeRequired)
			{
				return (Visibility)Dispatcher.Invoke(new DGGetPanelVisibility(GetPanelVisibility), DispatcherPriority.Render, panel);
			}
			return panel.Visibility;
		}

		private delegate void DGSetPanelState(bool state, params Panel[] Panels);
		public void SetPanelState(bool state, params Panel[] Panels)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetPanelState(SetPanelState), DispatcherPriority.Render, state, Panels);
				return;
			}
			foreach (var item in Panels)
			{
				item.IsEnabled = state;
			}
		}

		private delegate bool DGGetPanelState(Panel Panel);
		public bool GetPanelState(Panel panel)
		{
			if (InvokeRequired)
			{
				return (bool)Dispatcher.Invoke(new DGGetPanelState(GetPanelState), DispatcherPriority.Render, panel);
			}
			return panel.IsEnabled;
		}
		#endregion // Panel

		#region RadioButton
		private delegate void DGSetRadioButtonChecked(bool? state, params RadioButton[] radioButtons);
		public void SetRadioButtonChecked(bool? state, params RadioButton[] radioButtons)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetRadioButtonChecked(SetRadioButtonChecked), DispatcherPriority.Render, state, radioButtons);
				return;
			}
			foreach (var item in radioButtons)
			{
				item.IsChecked = state;
			}
		}

		private delegate bool? DGGetRadioButtonChecked(RadioButton radioButton);
		public bool? GetRadioButtonChecked(RadioButton radioButton)
		{
			if (InvokeRequired)
			{
				return (bool?)Dispatcher.Invoke(new DGGetRadioButtonChecked(GetRadioButtonChecked), DispatcherPriority.Render, radioButton);
			}
			return radioButton.IsChecked;
		}
    #endregion // RadioButton

    #region Slider
    private delegate void DGSetSliderValue(double value, params Slider[] sliders);
    public void SetSliderValue(double value, params Slider[] sliders)
    {
      if (InvokeRequired)
      {
        Dispatcher.Invoke(new DGSetSliderValue(SetSliderValue), DispatcherPriority.Render, value, sliders);
        return;
      }
      foreach (var slider in sliders)
      {
        slider.Value = value;
      }
    }

    private delegate void DGSetSliderMinimum(double Minimum, params Slider[] sliders);
    public void SetSliderMinimum(double Minimum, params Slider[] sliders)
    {
      if (InvokeRequired)
      {
        Dispatcher.Invoke(new DGSetSliderMinimum(SetSliderMinimum), DispatcherPriority.Render, Minimum, sliders);
        return;
      }
      foreach (var slider in sliders)
      {
        slider.Minimum = Minimum;
      }
    }

    private delegate void DGSetSliderMaximum(double Maximum, params Slider[] sliders);
    public void SetSliderMaximum(double Maximum, params Slider[] sliders)
    {
      if (InvokeRequired)
      {
        Dispatcher.Invoke(new DGSetSliderMaximum(SetSliderMaximum), DispatcherPriority.Render, Maximum, sliders);
        return;
      }
      foreach (var slider in sliders)
      {
        slider.Value = Maximum;
      }
    }

    private delegate double DGGetSliderValue(Slider slider);
    public double GetSliderValue(Slider slider)
    {
      if (InvokeRequired)
      {
        return (double)Dispatcher.Invoke(new DGGetSliderValue(GetSliderValue), DispatcherPriority.Render, slider); ;
      }
      return slider.Value;
    }

    private delegate double DGGetSliderMinimum(Slider slider);
    public double GetSliderMinimum(Slider slider)
    {
      if (InvokeRequired)
      {
        return (double)Dispatcher.Invoke(new DGGetSliderMinimum(GetSliderMinimum), DispatcherPriority.Render, slider); ;
      }
      return slider.Minimum;
    }

    private delegate double DGGetSliderMaximum(Slider slider);
    public double GetSliderMaximum(Slider slider)
    {
      if (InvokeRequired)
      {
        return (double)Dispatcher.Invoke(new DGGetSliderMaximum(GetSliderMaximum), DispatcherPriority.Render, slider); ;
      }
      return slider.Maximum;
    }
    #endregion // Slider

    #region TextBlock
    private delegate void DGSetTextBlockText(String text, params TextBlock[] TextBlock);
		public void SetTextBlockText(String text, params TextBlock[] TextBlock)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetTextBlockText(SetTextBlockText), DispatcherPriority.Render, text, TextBlock);
				return;
			}
			foreach (var item in TextBlock)
			{
				item.Text = text;
			}
		}

		private delegate String DGGetTextBlockText(TextBlock TextBlock);
		public String GetTextBlockText(TextBlock TextBlock)
		{
			if (InvokeRequired)
			{
				return Dispatcher.Invoke(new DGGetTextBlockText(GetTextBlockText), DispatcherPriority.Render, TextBlock) as String;
			}
			return TextBlock.Text;
		}
		#endregion// TextBlock

		#region TextBox
		private delegate void DGSetTextBoxText(String text, params TextBox[] textBox);
		public void SetTextBoxText(String text, params TextBox[] textBox)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetTextBoxText(SetTextBoxText), DispatcherPriority.Render, text, textBox);
				return;
			}
			foreach (var item in textBox)
			{
				item.Text = text;
			}
		}

		private delegate String DGGetTextBoxText(TextBox textBox);
		public String GetTextBoxText(TextBox textBox)
		{
			if (InvokeRequired)
			{
				return Dispatcher.Invoke(new DGGetTextBoxText(GetTextBoxText), DispatcherPriority.Render, textBox) as String;
			}
			return textBox.Text;
		}

		private delegate void DGSetTextBoxTextColor(Brush color, params TextBox[] textBox);
		public void SetTextBoxTextColor(Brush color, params TextBox[] textBox)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetTextBoxTextColor(SetTextBoxTextColor), DispatcherPriority.Render, color, textBox);
				return;
			}
			foreach (var item in textBox)
			{
				item.Foreground = color;
			}
    }

    private delegate void DGSetTextBoxBackgroundColor(Brush color, params TextBox[] textBox);
    public void SetTextBoxBackgroundColor(Brush color, params TextBox[] textBox)
    {
      if (InvokeRequired)
      {
        Dispatcher.Invoke(new DGSetTextBoxBackgroundColor(SetTextBoxBackgroundColor), DispatcherPriority.Render, color, textBox);
        return;
      }
      foreach (var item in textBox)
      {
        item.Background = color;
      }
    }

    private delegate void DGSelectAllTextBox(TextBox textBox);
		public void SelectAllTextBox(TextBox textBox)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSelectAllTextBox(SelectAllTextBox), DispatcherPriority.Render, textBox);
				return;
			}
			textBox.SelectAll();
		}
		#endregion // TextBox

		#region ToggleButton
		private delegate bool? DGGetToggleButtonChecked(ToggleButton tb);
		public bool? GetToggleButtonChecked(ToggleButton tb)
		{
			if (InvokeRequired)
			{
				return (bool?)Dispatcher.Invoke(new DGGetToggleButtonChecked(GetToggleButtonChecked), DispatcherPriority.Render, tb);
			}
			return tb.IsChecked;
		}

		private delegate void DGSetToggleButtonChecked(bool? isChecked, params ToggleButton[] tb);
		public void SetToggleButtonChecked(bool? isChecked, params ToggleButton[] tb)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetToggleButtonChecked(SetToggleButtonChecked), DispatcherPriority.Render, isChecked, tb);
				return;
			}
			foreach (var item in tb)
			{
				item.IsChecked = isChecked;
			}
		}
    #endregion // ToggleButton

    #region MenuItem
    private delegate bool DGGetMenuItemChecked(MenuItem mi);
    public bool GetMenuItemChecked(MenuItem mi)
    {
      if (InvokeRequired)
      {
        return (bool)Dispatcher.Invoke(new DGGetMenuItemChecked(GetMenuItemChecked), DispatcherPriority.Render, mi);
      }
      return mi.IsChecked;
    }

    private delegate void DGSetMenuItemChecked(bool isChecked, params MenuItem[] mi);
    public void SetMenuItemChecked(bool isChecked, params MenuItem[] mi)
    {
      if (InvokeRequired)
      {
        Dispatcher.Invoke(new DGSetMenuItemChecked(SetMenuItemChecked), DispatcherPriority.Render, isChecked, mi);
        return;
      }
      foreach (var item in mi)
      {
        item.IsChecked = isChecked;
      }
    }
    #endregion // MenuItem

    #region TreeView
    private delegate void DGAddTreeViewItem(TreeView tv, params TreeViewItem[] tvi);
    public void AddTreeViewItem(TreeView tv, params TreeViewItem[] tvi)
    {
      if (InvokeRequired)
      {
        Dispatcher.Invoke(new DGAddTreeViewItem(AddTreeViewItem), tv, tvi);
        return;
      }
      foreach (var item in tvi)
      {
        tv.Items.Add(item);
      }
    }
    #endregion // TreeView

    #region Window
    private delegate void DGSetWindowTitle(String title, params Window[] windows);
		public void SetWindowTitle(String title, params Window[] windows)
		{
			if (InvokeRequired)
			{
				Dispatcher.Invoke(new DGSetWindowTitle(SetWindowTitle), DispatcherPriority.Render, title, windows);
				return;
			}
			foreach (var item in windows)
			{
				item.Title = title;
			}
		}
		#endregion // Window
	}
}
