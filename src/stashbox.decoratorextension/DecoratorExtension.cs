using Sendstorm;
using Sendstorm.Infrastructure;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Extensions
{
    internal class DecoratorEntry
    {
        public Type DecoratorTarget { get; set; }
        public string DecoratorRegistrationName { get; set; }
    }

    /// <summary>
    /// Represents a decorator container extension
    /// </summary>
    public class DecoratorExtension : IPostBuildExtension, IMessageReceiver<GlobalDecoratorRegistered>, IMessageReceiver<ExplicitDecoratorRegistered>
    {
        internal const string MessagePublisherKey = "79B155F6-4068-4996-93DE-2FB17521824B";

        private IStashboxContainer container;
        private readonly IMessagePublisher messagePublisher;
        private readonly ConcurrentKeyValueStore<Type, LinkedList<DecoratorEntry>> decoratorStore;
        private readonly object syncObject = new object();

        /// <summary>
        /// Constructs a <see cref="DecoratorExtension"/>
        /// </summary>
        public DecoratorExtension()
        {
            this.decoratorStore = new ConcurrentKeyValueStore<Type, LinkedList<DecoratorEntry>>();
            this.messagePublisher = new MessagePublisher();
            this.messagePublisher.Subscribe<GlobalDecoratorRegistered>(this);
            this.messagePublisher.Subscribe<ExplicitDecoratorRegistered>(this);
        }

        /// <summary>
        /// Initializes the extension. This method will be called by the <see cref="IStashboxContainer"/> right after the extension registration.
        /// </summary>
        /// <param name="containerContext">The context object of the <see cref="IStashboxContainer"/></param>
        public void Initialize(IContainerContext containerContext)
        {
            containerContext.Bag.Add(MessagePublisherKey, this.messagePublisher);
            this.container = containerContext.Container.BeginScope();
        }

        /// <summary>
        /// Executes the post build operation on an already built object by the the <see cref="IStashboxContainer"/>
        /// </summary>
        /// <param name="instance">The instance to extend.</param>
        /// <param name="containerContext">The context object of the <see cref="IStashboxContainer"/></param>
        /// <param name="resolutionInfo">The resolution info provided by the <see cref="IStashboxContainer"/></param>
        /// <param name="resolveType">The resolve type provided by the <see cref="IStashboxContainer"/></param>
        /// <param name="injectionParameters">The injection parameters provided by the <see cref="IStashboxContainer"/></param>
        /// <returns>The extended object.</returns>
        public object PostBuild(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
           Type resolveType, InjectionParameter[] injectionParameters = null)
        {
            var type = instance.GetType();
            LinkedList<DecoratorEntry> decorators;
            if (!this.decoratorStore.TryGet(resolveType, out decorators)) return instance;

            lock (this.syncObject)
            {
                instance = decorators.Where(
                    decoratorEntry =>
                        type == decoratorEntry.DecoratorTarget ||
                        decoratorEntry.DecoratorTarget.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                    .Aggregate(instance,
                        (current, decoratorEntry) =>
                        {
                            var factory = this.container.ResolveFactory(resolveType, decoratorEntry.DecoratorRegistrationName, resolveType);
                            return factory.DynamicInvoke(current);
                        });
            }

            return instance;
        }

        /// <summary>
        /// Cleans up the decorator extension.
        /// </summary>
        public void CleanUp()
        {
            this.container.Dispose();
            this.messagePublisher.UnSubscribe<GlobalDecoratorRegistered>(this);
            this.messagePublisher.UnSubscribe<ExplicitDecoratorRegistered>(this);
        }

        /// <summary>
        /// Creates a copy for the <see cref="IStashboxContainer"/>
        /// </summary>
        /// <returns>The copy of the extension.</returns>
        public IContainerExtension CreateCopy()
        {
            return new DecoratorExtension();
        }

        /// <summary>
        /// Executes when a global decorator is registered to the <see cref="IStashboxContainer"/>
        /// </summary>
        /// <param name="message">The global decorator registered message.</param>
        public void Receive(GlobalDecoratorRegistered message)
        {
            var entry = new DecoratorEntry
            {
                DecoratorTarget = message.TypeFrom,
                DecoratorRegistrationName = message.DecoratorType.Name
            };

            this.decoratorStore.AddOrUpdate(message.TypeFrom, () =>
            {
                var list = new LinkedList<DecoratorEntry>();
                list.AddFirst(entry);
                return list;
            }, (oldValaue, newValue) =>
            {
                lock (this.syncObject)
                {
                    oldValaue.AddLast(entry);
                    return oldValaue;
                }
            });

            this.container.RegisterType(message.TypeFrom, message.DecoratorType, entry.DecoratorRegistrationName);
        }

        /// <summary>
        /// Executes when an explicit decorator is registered to the <see cref="IStashboxContainer"/>
        /// </summary>
        /// <param name="message">The explicit decorator registered message.</param>
        public void Receive(ExplicitDecoratorRegistered message)
        {
            var entry = new DecoratorEntry
            {
                DecoratorTarget = message.TypeTo,
                DecoratorRegistrationName = message.DecoratorType.Name
            };

            this.decoratorStore.AddOrUpdate(message.TypeFrom, () =>
            {
                var list = new LinkedList<DecoratorEntry>();
                list.AddFirst(entry);
                return list;
            }, (oldValaue, newValue) =>
            {
                lock (this.syncObject)
                {
                    oldValaue.AddLast(entry);
                    return oldValaue;
                }
            });

            this.container.RegisterType(message.TypeFrom, message.DecoratorType, entry.DecoratorRegistrationName);
        }
    }
}
