using System;
using System.Collections.Generic;
using UI.Core.Interfaces;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace UI.Core
{
    /// <summary>
    /// Wrapper class for Unity container.
    /// </summary>
    public class DiContainer : IDiContainer
    {
        private static DiContainer _this;
        private IUnityContainer _container = new UnityContainer();

        /// <summary>
        /// Initializes a new instance of the <see cref="DiContainer"/> class.        
        /// </summary>
        protected DiContainer()
        {            
            Configure();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiContainer"/> class.        
        /// </summary>
        /// <param name="configurationSection">The configuration section.</param>
        protected DiContainer(object configurationSection)
        {
            Configure();
        }

        public IUnityContainer Parent => _container.Parent;

        public IEnumerable<IContainerRegistration> Registrations { get; }

        /// <summary>
        /// Returns the instance of the container.
        /// </summary>
        /// <returns>The DiContainer object.</returns>
        public static DiContainer GetInstance()
        {
            return _this ?? (_this = new DiContainer());
        }

        public void RegisterType<TFrom, TTo>(params InjectionMember[] injectionMembers)
            where TTo : TFrom
        {
            _container.RegisterType<TFrom, TTo>(injectionMembers);
        }

        public T Resolve<T>(params object[][] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return _container.Resolve<T>(CreateOverrides(items));
        }

        public T Resolve<T>(Dictionary<string, object> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return _container.Resolve<T>(CreateOverrides(items));
        }

        public T Resolve<T>(string name, params object[][] items)
        {
            return _container.Resolve<T>(name, CreateOverrides(items));
        }

        public T TryResolve<T>(string name, params object[][] items)
        {
            return _container.IsRegistered<T>(name) ? Resolve<T>(name, items) : default(T);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IUnityContainer RegisterType(Type registeredType, Type mappedToType, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        public IUnityContainer RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            throw new NotImplementedException();
        }

        public IUnityContainer RegisterFactory(Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type t, string name, params ResolverOverride[] resolverOverrides)
        {
            return _container.Resolve(t, name, resolverOverrides);
        }

        public IEnumerable<object> ResolveAll(Type t, params ResolverOverride[] resolverOverrides)
        {
            return _container.ResolveAll(t, resolverOverrides);
        }

        public object BuildUp(Type t, object existing, string name, params ResolverOverride[] resolverOverrides)
        {
            return _container.BuildUp(t, existing, name, resolverOverrides);
        }

        public IUnityContainer AddExtension(UnityContainerExtension extension)
        {
            return _container.AddExtension(extension);
        }

        public object Configure(Type configurationInterface)
        {
            return _container.Configure(configurationInterface);
        }

        public IUnityContainer CreateChildContainer()
        {
            return _container.CreateChildContainer();
        }

        private static ResolverOverride[] CreateOverrides(object[][] values)
        {
            if (values == null)
            {
                return null;
            }

            var arguments = new ResolverOverride[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Length != 2)
                {
                    throw new ArgumentException("Parameter is not specified correctly");
                }

                arguments[i] = new ParameterOverride(string.Empty + values[i][0], values[i][1]);
            }

            return arguments;
        }

        private static ResolverOverride[] CreateOverrides(Dictionary<string, object> items)
        {
            if (items == null)
            {
                return null;
            }

            var arguments = new List<ResolverOverride>();
            foreach (var item in items)
            {
                arguments.Add(new ParameterOverride(item.Key, item.Value));
            }

            return arguments.ToArray();
        }

        private void Configure()
        {
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _container.Dispose();
                _container = null;
            }
        }
    }
}
