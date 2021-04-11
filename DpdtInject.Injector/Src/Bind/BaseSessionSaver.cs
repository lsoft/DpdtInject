using System;

namespace DpdtInject.Injector.Src.Bind
{
    public abstract class BaseSessionSaver
    {
        /// <summary>
        /// Fixes the current session to a permanent storage.
        /// DO NOT RAISE AN EXCEPTION HERE!
        /// </summary>
        public abstract Guid StartSessionSafely(
            in string fullClassName,
            in string memberName,
            in object[]? arguments
            );

        /// <summary>
        /// Fixes the current session to a permanent storage.
        /// DO NOT RAISE AN EXCEPTION HERE!
        /// </summary>
        public abstract void FixSessionSafely(
            in Guid sessionGuid,
            in double takenInSeconds,
            Exception? exception
            );

    }
}
