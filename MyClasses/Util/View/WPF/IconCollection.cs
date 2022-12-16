using AMD.Util.Collections.Dictionary;
using AMD.Util.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AMD.Util.View.WPF
{
  public class TemplateNameAttribute : Attribute
  {
    public string Name { get; set; }
    public TemplateNameAttribute(string name)
    {
      Name = name;
    }
  }

  public class IconCollection
  {
    public enum Icon
    {
      [TemplateName("ctCrossInRedCircle")]
      CrossInRedCircle,

      [TemplateName("ctCrossInYellowTriangle")]
      CrossInYellowTriangle,

      [TemplateName("ctCheckInGreenCircle")]
      CheckInGreenCircle,

      [TemplateName("ctCheckInGrayCircle")]
      CheckInGrayCircle,

      [TemplateName("ctExclamationInGrayCircle")]
      ExclamationInGrayCircle,

      [TemplateName("ctExclamationInYellowTriangle")]
      ExclamationInYellowTriangle,

      [TemplateName("ctExclamationInGreenTriangle")]
      ExclamationInGreenTriangle,

      [TemplateName("ctHourglassInBlueCircle")]
      HourGlassInBlueCircle,

      [TemplateName("ctLineInGrayCircle")]
      LineInGrayCircle,

      [TemplateName("ctCogInPurpleCircle")]
      CogInPurpleCircle,

      [TemplateName("ctPadLockInOrangeCircle")]
      PadLockInOrangeCircle,

      [TemplateName("ctIInRedCircle")]
      iInRedCircle,

      [TemplateName("ctMInRedCircle")]
      MInRedCircle,

      [TemplateName("ctVInRedCircle")]
      VInRedCircle,

      [TemplateName("ctRInRedCircle")]
      RInRedCircle,

      [TemplateName("ctVInOrangeCircle")]
      VInOrangeCircle,

      [TemplateName("ctYellowNoteIcon")]
      YellowNote,

      [TemplateName("ctYellowNoteWithTape")]
      YellowNoteWithTape
    }

    private static IconCollection instance;

    public static IconCollection Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new IconCollection();
        }
        return instance;
      }
    }

    public ControlTemplate this[Icon icon]
    {
      get
      {
        return Application.Current.TryFindResource(icon.GetAttribute<TemplateNameAttribute>().Name) as ControlTemplate;
      }
    }

    private IconCollection()
    {

    }
  }
}
