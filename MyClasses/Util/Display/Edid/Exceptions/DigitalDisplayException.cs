using System;

namespace AMD.Util.Display.Edid.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of display being a digital display
    /// </summary>
    public class DigitalDisplayException : Exception
    {
        internal DigitalDisplayException(string message) : base(message)
        {
        }
    }
}