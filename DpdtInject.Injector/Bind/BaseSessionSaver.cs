﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Bind
{
    public abstract class BaseSessionSaver
    {
        /// <summary>
        /// Fixes the current session to a permanent storage.
        /// DO NOT RAISE AN EXCEPTION HERE!
        /// </summary>
        public abstract void FixSessionSafely(
            in string fullClassName,
            in string memberName,
            in double takenInSeconds,
            Exception? exception,
            in object[]? arguments
            );

    }
}