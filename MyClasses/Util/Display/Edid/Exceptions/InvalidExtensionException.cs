using System;

namespace AMD.Util.Display.Edid.Exceptions
{
    internal class InvalidExtensionException : Exception
    {
        public InvalidExtensionException(string message) : base(message)
        {
        }
    }
}