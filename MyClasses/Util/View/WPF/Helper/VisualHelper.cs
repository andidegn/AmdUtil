using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AMD.Util.View.WPF.Helper
{
  public static class VisualHelper
  {
    public static DependencyObject GetChildDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
    {
      //Look in every branch inside to find the object
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startObject); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(startObject, i);
        if (type.IsInstanceOfType(child))
        {
          return child;
        }
        else
        {
          child = GetChildDependencyObjectFromVisualTree(VisualTreeHelper.GetChild(startObject, i), type);
          if (type.IsInstanceOfType(child))
          {
            return child;
          }
        }
      }

      return null;
    }

    public static DependencyObject GetChildDependencyObjectFromVisualTree(DependencyObject startObject, String name)
    {
      //Look in every branch inside to find the object
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startObject); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(startObject, i);
        if (typeof(Control).IsInstanceOfType(child) && (child as Control).Name == name)
        {
          return child;
        }
        else
        {
          child = GetChildDependencyObjectFromVisualTree(VisualTreeHelper.GetChild(startObject, i), name);
          if (typeof(Control).IsInstanceOfType(child) && (child as Control).Name == name)
          {
            return child;
          }
        }
      }

      return null;
    }

    public static DependencyObject GetChildDependencyObjectFromVisualTree(DependencyObject startObject, Type type, String name)
    {
      //Look in every branch inside to find the object
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startObject); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(startObject, i);
        if (type.IsInstanceOfType(child) && (child as Control)?.Name == name)
        {
          return child;
        }
        else
        {
          child = GetChildDependencyObjectFromVisualTree(VisualTreeHelper.GetChild(startObject, i), name);
          if (type.IsInstanceOfType(child) && (child as Control)?.Name == name)
          {
            return child;
          }
        }
      }

      return null;
    }
  }
}
