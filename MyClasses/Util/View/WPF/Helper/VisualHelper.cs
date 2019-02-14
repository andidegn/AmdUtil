using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AMD.Util.View.WPF.Helper
{
  /// <summary>
  /// Helper class for visual properties in WPF
  /// </summary>
  public static class VisualHelper
  {
    /// <summary>
    /// Gets the child of the startObject of a certian type 
    /// </summary>
    /// <param name="startObject"></param>
    /// <param name="type"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets the child of the startObject with a specific name
    /// </summary>
    /// <param name="startObject"></param>
    /// <param name="name"></param>
    /// <returns></returns>
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

    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
      if (depObj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
          if (child != null && child is T)
          {
            yield return (T)child;
          }

          foreach (T childOfChild in FindVisualChildren<T>(child))
          {
            yield return childOfChild;
          }
        }
      }
    }

    public static FrameworkElement FindVisualChild<T>(FrameworkElement depObj, String name) where T : DependencyObject
    {
      if (depObj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
          FrameworkElement child = VisualTreeHelper.GetChild(depObj, i) as FrameworkElement;
          if (child != null && child is T && child.Name == name)
          {
            return child;
          }
        }
      }
      return null;
    }

    public static FrameworkElement FindVisualChild<T>(FrameworkElement depObj, FrameworkElement objToFind) where T : DependencyObject
    {
      if (depObj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
          FrameworkElement child = VisualTreeHelper.GetChild(depObj, i) as FrameworkElement;
          if (child != null && child is T && child == objToFind)
          {
            return child;
          }
        }
      }
      return null;
    }
  }
}
