using System;
using Microsoft.Practices.Unity;

namespace BP.Diadem.Common
{
    /// <summary>
    /// Extension methods for ContainerRegistrations class.
    /// </summary>
    internal static class ContainerRegistrationsExtension
    {
        /// <summary>
        /// Helper functions to output container registration in user-friendly format.
        /// </summary>
        /// <param name="registration">The ContainerRegistration object.</param>
        /// <returns>The formatted string.</returns>
        public static string GetMappingAsString(this ContainerRegistration registration)
        {
            var r = registration.RegisteredType;
            string regType = r.Name + GetGenericArgumentsList(r);

            var m = registration.MappedToType;
            string mapTo = m.Name + GetGenericArgumentsList(m);

            string regName = registration.Name ?? "[default]";

            string lifetime = registration.LifetimeManagerType.Name;
            if (mapTo != regType)
            {
                mapTo = " -> " + mapTo;
            }
            else
            {
                mapTo = string.Empty;
            }

            lifetime = lifetime.Substring(0, lifetime.Length - "LifetimeManager".Length);
            return $"+ {regType}{mapTo}  '{regName}'  {lifetime}";
        }

        private static string GetGenericArgumentsList(Type type)
        {
            if (type.GetGenericArguments().Length == 0)
            {
                return string.Empty;
            }

            string arglist = string.Empty;
            bool first = true;
            foreach (Type t in type.GetGenericArguments())
            {
                arglist += first ? t.Name : ", " + t.Name;
                first = false;
                if (t.GetGenericArguments().Length > 0)
                {
                    arglist += GetGenericArgumentsList(t);
                }
            }

            return "<" + arglist + ">";
        }
    }
}
