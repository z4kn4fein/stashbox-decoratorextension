using System;

namespace Stashbox.Extensions
{
    /// <summary>
    /// Represents an explicit decorator registered message.
    /// </summary>
    public class ExplicitDecoratorRegistered
    {
        /// <summary>
        /// The type to parameter.
        /// </summary>
        public Type TypeTo { get; set; }

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
