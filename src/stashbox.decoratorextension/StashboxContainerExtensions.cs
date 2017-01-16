using Sendstorm.Infrastructure;
using Stashbox.Extensions;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents the extension methods for <see cref="IStashboxContainer"/> and <see cref="IRegistrationContext"/>
    /// </summary>
    public static class StashboxContainerExtensions
    {
        /// <summary>
        /// Registers a global decorator to the <see cref="IStashboxContainer"/>
        /// </summary>
        /// <typeparam name="TFrom">The interface type.</typeparam>
        /// <typeparam name="TDecorator">The decorator type.</typeparam>
        /// <param name="container"><see cref="IStashboxContainer"/></param>
        /// <returns><see cref="IDependencyRegistrator"/></returns>
        public static IDependencyRegistrator RegisterDecorator<TFrom, TDecorator>(this IStashboxContainer container) where TDecorator : TFrom
        {   
            object publisher;
            if (!container.ContainerContext.Bag.TryGet(DecoratorExtension.MessagePublisherKey, out publisher))
                return container;
            var messagePublisher = (IMessagePublisher)publisher;
            messagePublisher.Broadcast(new GlobalDecoratorRegistered
            {
                DecoratorType = typeof(TDecorator),
                TypeFrom = typeof(TFrom)
            });

            return container;
        }

        /// <summary>
        /// Registers a decorator directly to a registration.
        /// </summary>
        /// <typeparam name="TDecorator">The decorator type.</typeparam>
        /// <param name="context"><see cref="IRegistrationContext"/></param>
        /// <returns><see cref="IRegistrationContext"/></returns>
        public static IRegistrationContext DecorateWith<TDecorator>(this IRegistrationContext context)
        {
            object publisher;
            if (!context.ContainerContext.Bag.TryGet(DecoratorExtension.MessagePublisherKey, out publisher))
                return context;
            var messagePublisher = (IMessagePublisher)publisher;
            messagePublisher.Broadcast(new ExplicitDecoratorRegistered
            {
                DecoratorType = typeof(TDecorator),
                TypeTo = context.TypeTo,
                TypeFrom = context.TypeFrom
            });

            return context;
        }
    }
}
