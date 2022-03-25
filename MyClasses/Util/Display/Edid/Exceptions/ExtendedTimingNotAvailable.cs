﻿using System;

namespace AMD.Util.Display.Edid.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of missing extended timing information
    /// </summary>
    public class ExtendedTimingNotAvailable : Exception
    {
        internal ExtendedTimingNotAvailable(string message) : base(message)
        {
        }
    }
}