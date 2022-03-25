using System;

namespace AMD.Util.Display.Edid.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of missing production date information
    /// </summary>
    public class ManufactureDateMissingException : Exception
    {
        internal ManufactureDateMissingException(string message) : base(message)
        {
        }
    }
}