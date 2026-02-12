using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UI.Shell.Interfaces;

namespace UI.Shell.MenuBuilder
{
    public class MenuBuilderCatalog : IMenuBuilderCatalog
    {
        public IEnumerable<Type> DiscoverViewTypes()
        {
            string fullPath = Assembly.GetExecutingAssembly().Location;

            string[] allFiles = Directory.GetFiles(Path.GetDirectoryName(fullPath), "UI*.dll");

            var files = allFiles.ToList();

            var assemblies = new List<Assembly>();
            foreach (string file in files)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            var list = new List<Type>();

            /*
            var types = AllClasses.FromAssemblies(assemblies);

            foreach (var type in types.Where(t => t.IsClass && (t.BaseType == typeof(UserControl) || t.BaseType == typeof(Infrastructure.Controls.UserControl))))
            {
                list.Add(type);
            }
            */

            return list;
        }
    }
}