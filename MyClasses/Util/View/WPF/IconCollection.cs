using System;
using System.Windows;
using System.Windows.Controls;

namespace AMD.Util.View.WPF
{
    public class IconCollection
    {
        public String ICON_CROSS { get { return "ctCrossInRedCircle"; } }
        public String ICON_CHECK { get { return "ctCheckInGreenCircle"; } }
        public String ICON_HOURGLASS { get { return "ctHourglassInBlueCircle"; } }
        public String ICON_LINE { get { return "ctLineInGrayCircle"; } }
        public String ICON_COG { get { return "ctCogInPurpleCircle"; } }

        public ControlTemplate CtCross;
        public ControlTemplate CtCheck;
        public ControlTemplate CtCog;
        public ControlTemplate CtLine;
        public ControlTemplate CtHourGlass;

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

        private IconCollection()
        {
            CtCross = Application.Current.TryFindResource(ICON_CROSS) as ControlTemplate;
            CtCheck = Application.Current.TryFindResource(ICON_CHECK) as ControlTemplate;
            CtCog = Application.Current.TryFindResource(ICON_COG) as ControlTemplate;
            CtLine = Application.Current.TryFindResource(ICON_LINE) as ControlTemplate;
            CtHourGlass = Application.Current.TryFindResource(ICON_HOURGLASS) as ControlTemplate;
        }
    }
}
