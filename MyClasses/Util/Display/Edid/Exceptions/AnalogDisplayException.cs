using System;

namespace AMD.Util.Display.Edid.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of display being an analog display
    /// </summary>
    public class AnalogDisplayException : Exception
    {
        internal AnalogDisplayException(string message) : base(message)
        {
        }
    }
}