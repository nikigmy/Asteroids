using System;

namespace Utils
{
    /// <summary>
    ///     Extends the basic object class with event operations.
    /// </summary>
    public static class ObjectEventExtensions
    {
        public static void Raise<T>(this object sender, EventHandler<T> handler, T args) where T : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }

        public static void Raise(this object sender, EventHandler handler)
        {
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }
    }
}