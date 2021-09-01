using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls.TearableTabs
{
  public class TearableTabControlAdorner : Adorner
  {
    private UIElement child = null;
    private Point position;

    /// <summary>
    /// Initializes a new instance of DragVisualAdorner.
    /// </summary>
    /// <param name="adornedElement">The element being adorned.</param>
    /// <param name="size">The size of the adorner.</param>
    /// <param name="brush">A brush to with which to paint the adorner.</param>
    public TearableTabControlAdorner(UIElement adornedElement, UIElement elementToShow, double adornerOpacity)
      : base(adornedElement)
    {
      Size size = elementToShow.RenderSize;
      Rectangle rect = new Rectangle();
      rect.Fill = new VisualBrush(elementToShow);
      rect.Width = size.Width;
      rect.Height = size.Height;
      rect.Opacity = adornerOpacity;
      rect.IsHitTestVisible = false;
      this.child = rect;
    }

    #region Force Layout system
    //make sure that the layout system knows of the element
    protected override Size MeasureOverride(Size constraint)
    {
      this.child.Measure(constraint);
      return this.child.DesiredSize;
    }

    //make sure that the layout system knows of the element
    protected override Size ArrangeOverride(Size finalSize)
    {
      this.child.Arrange(new Rect(finalSize));
      return finalSize;
    }
    #endregion

    #region Force the visual to show
    //return the visual that we want to display
    protected override System.Windows.Media.Visual GetVisualChild(int index)
    {
      return this.child;
    }

    //return the count of the visuals
    protected override int VisualChildrenCount
    {
      get { return 1; }
    }

    /// <summary>
    /// Override.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
      GeneralTransformGroup result = new GeneralTransformGroup();
      result.Children.Add(base.GetDesiredTransform(transform));
      //result.Children.Add(new TranslateTransform(-100, 0));
      result.Children.Add(new TranslateTransform(position.X, position.Y));
      return result;
    }
    #endregion

    //updates the position of the adorner
    public void UpdatePosition(Point point)
    {
      position = point;
      AdornerLayer parentLayer = Parent as AdornerLayer;
      if (parentLayer != null)
      {
        parentLayer.Update(AdornedElement);
      }
    }

    #region Helpers
    //create a clone of the element being dragged
    private static ContentControl CreateClone(UIElement element)
    {
      ContentControl control = new ContentControl();
      ContentControl element1 = element as ContentControl;
      if (element1 != null)
      {
        control.Content = element1.Content;
        control.ContentTemplate = element1.ContentTemplate;
      }
      ContentPresenter element2 = element as ContentPresenter;
      if (element2 != null)
      {
        control.Content = element2.Content;
        control.ContentTemplate = element2.ContentTemplate;
      }

      return control;
    }
    #endregion
  }
}
