using System;

namespace AMD.Util.Display.Edid.Exceptions
{
    internal class InvalidDescriptorException : Exception
    {
        internal InvalidDescriptorException(string message) : base(message)
        {
        }
    }
}