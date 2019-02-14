using System.Windows;
using System.Windows.Documents;

namespace AMD.Util.View.WPF.UserControls
{
  internal class TearableTabSharedHelper
  {
    internal bool AllowTabDrag { get; set; }
    internal Point AdornerStartPoint { get; set; }

    private AdornerLayer adornerLayer;
    private TearableTabControlAdorner tabControlAdorner;

    private static TearableTabSharedHelper instance;
    internal static TearableTabSharedHelper Instance
    {
      get
      {
        if (null == instance)
        {
          instance = new TearableTabSharedHelper();
        }
        return instance;
      }
    }

    internal TearableTabSharedHelper()
    {
    }

    internal void StartDrag(UIElement adornerElementTarget, UIElement itemToDrag)
    {
      if (AllowTabDrag)
      {
        SetAdornerLayer(adornerElementTarget, itemToDrag);

        DragDrop.DoDragDrop((itemToDrag as FrameworkElement).Parent, itemToDrag, DragDropEffects.All);

        ClearAdornerLayer();
      }
    }

    internal void SetAdornerLayer(UIElement adornerElementTarget, UIElement itemToDrag)
    {
      if (null != adornerLayer)
      {
        ClearAdornerLayer();
      }
      adornerLayer = AdornerLayer.GetAdornerLayer(adornerElementTarget);
      tabControlAdorner = new TearableTabControlAdorner(adornerElementTarget, itemToDrag, 0.8);
      adornerLayer.Add(tabControlAdorner);
    }

    internal void ClearAdornerLayer()
    {
      if (null != tabControlAdorner)
      {
        adornerLayer.Remove(tabControlAdorner);
        adornerLayer = null;
      }
    }

    internal void UpdateAdornerPosition(double x, double y)
    {
      if (null != tabControlAdorner)
      {
        tabControlAdorner.UpdatePosition(new Point(x, y));
      }
    }

    internal bool IsAdornerElement(DependencyObject parent)
    {
      return tabControlAdorner.AdornedElement == parent;
    }
  }
}
