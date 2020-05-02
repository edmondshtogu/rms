using System;

namespace RMS.Messages
{
    /// <summary>
    /// <see cref="DecoratorOrderAttribute"/> class
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public sealed class DecoratorOrderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratorOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public DecoratorOrderAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; }
    }
}
