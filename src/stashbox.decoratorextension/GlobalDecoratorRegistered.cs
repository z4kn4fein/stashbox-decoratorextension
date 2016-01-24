using System;

namespace Stashbox.Extensions
{
    /// <summary>
    /// Represents a global decorator registered message.
    /// </summary>
    public class GlobalDecoratorRegistered
    {
        /// <summary>
        /// The type from parameter.
        /// </summary>
        public Type TypeFrom { get; set; }

        /// <summary>
        /// The type of the decorator.
        /// </summary>
        public Type DecoratorType { get; set; }
    }
}
