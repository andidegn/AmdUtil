using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AMD.Util.View.WPF.Helper
{
  public sealed class PropertyChangedNotifier : DependencyObject, IDisposable
  {
    #region Member Variables
    private WeakReference _propertySource;
    #endregion // Member Variables

    #region Constructor
    public PropertyChangedNotifier(DependencyObject propertySource, string path)
    : this(propertySource, new PropertyPath(path))
    {
    }
    public PropertyChangedNotifier(DependencyObject propertySource, DependencyProperty property)
    : this(propertySource, new PropertyPath(property))
    {
    }
    public PropertyChangedNotifier(DependencyObject propertySource, PropertyPath property)
    {
      if (null == propertySource)
      {
        throw new ArgumentNullException(nameof(propertySource));
      }

      this._propertySource = new WeakReference(propertySource);
      Binding binding = new Binding();
      binding.Path = property ?? throw new ArgumentNullException(nameof(property));
      binding.Mode = BindingMode.OneWay;
      binding.Source = propertySource;
      BindingOperations.SetBinding(this, ValueProperty, binding);
    }
    #endregion // Constructor

    #region PropertySource
    public DependencyObject PropertySource
    {
      get
      {
        try
        {
          // note, it is possible that accessing the target property
          // will result in an exception so i’ve wrapped this check
          // in a try catch
          return this._propertySource.IsAlive
          ? this._propertySource.Target as DependencyObject
          : null;
        }
        catch
        {
          return null;
        }
      }
    }
    #endregion // PropertySource

    #region Value
    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value),
    typeof(object), typeof(PropertyChangedNotifier), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      PropertyChangedNotifier notifier = (PropertyChangedNotifier)d;
      notifier.ValueChanged?.Invoke(notifier, EventArgs.Empty);
    }

    /// <summary>
    /// Returns/sets the value of the property
    /// </summary>
    /// <seealso cref="ValueProperty"/>
    [Description("Returns / sets the value of the property")]
    [Category("Behavior")]
    [Bindable(true)]
    public object Value
    {
      get
      {
        return this.GetValue(PropertyChangedNotifier.ValueProperty);
      }
      set
      {
        this.SetValue(PropertyChangedNotifier.ValueProperty, value);
      }
    }
    #endregion //Value

    #region Events
    public event EventHandler ValueChanged;
    #endregion // Events

    #region IDisposable Members
    public void Dispose()
    {
      BindingOperations.ClearBinding(this, ValueProperty);
    }
    #endregion
  }
}
