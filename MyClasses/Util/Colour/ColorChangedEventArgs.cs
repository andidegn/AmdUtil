using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace AMD.Util.Colour
{
  public class ColorChangedEventArgs : EventArgs
  {
    /// <summary>
    /// The original color
    /// </summary>
    public Color OriginalColor { get; private set; }
    /// <summary>
    /// The selected color
    /// </summary>
    public Color SelectedColor { get; private set; }

    /// <summary>
    /// Instantiates and sets original and selected color
    /// </summary>
    /// <param name="originalColor"></param>
    /// <param name="selectedColor"></param>
    public ColorChangedEventArgs(Color originalColor, Color selectedColor)
    {
      this.OriginalColor = originalColor;
      this.SelectedColor = selectedColor;
    }
  }

  public class BrushChangedEventArgs : EventArgs
  {
    /// <summary>
    /// The original Brush
    /// </summary>
    public Brush OriginalBrush { get; private set; }
    /// <summary>
    /// The selected Brush
    /// </summary>
    public Brush SelectedBrush { get; private set; }

    /// <summary>
    /// Instantiates and sets original and selected Brush
    /// </summary>
    /// <param name="originalBrush"></param>
    /// <param name="selectedBrush"></param>
    public BrushChangedEventArgs(Brush originalBrush, Brush selectedBrush)
    {
      this.OriginalBrush = originalBrush;
      this.SelectedBrush = selectedBrush;
    }
  }
}
