using System.Collections.Generic;
using Unity;
using Unity.Injection;

namespace UI.Core.Interfaces
{
    /// <summary>
    /// Wrapper for Unity container.
    /// </summary>
    public interface IDiContainer : IUnityContainer
    {
        /// <summary>Register a type mapping with the container.</summary>
        /// <remarks>
        /// <para>
        /// This method is used to tell the container that when asked for type <typeparamref name="TFrom" />,
        /// actually return an instance of type <typeparamref name="TTo" />. This is very useful for
        /// getting instances of interfaces.
        /// </para>
        /// <para>
        /// This overload registers a default mapping and transient lifetime.
        /// </para>
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="T:System.Type" /> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="T:System.Type" /> that will actually be returned.</typeparam>        
        /// <param name="injectionMembers">Injection configuration objects.</param>        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        void RegisterType<TFrom, TTo>(params InjectionMember[] injectionMembers)
            where TTo : TFrom;

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to get from the container.</typeparam>        
        /// <param name="items">An array of name/value pairs meant to be used as constructor arguments for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        T Resolve<T>(params object[][] items);

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to get from the container.</typeparam>        
        /// /// <param name="name">The registered object's name.</param>
        /// <param name="items">An array of name/value pairs meant to be used as constructor arguments for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        T Resolve<T>(string name, params object[][] items);

        /// <summary>
        /// Attempt to resolve an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to get from the container.</typeparam>        
        /// <param name="name">The registered object's name.</param>
        /// <param name="items">An array of name/value pairs meant to be used as constructor arguments for the resolve call.</param>
        /// <returns>The retrieved object or null if couldn't be created.</returns>
        T TryResolve<T>(string name, params object[][] items);

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to get from the container.</typeparam>        
        /// <param name="items">A collection of name/value pairs meant to be used as constructor arguments for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        T Resolve<T>(Dictionary<string, object> items);
    }
}
