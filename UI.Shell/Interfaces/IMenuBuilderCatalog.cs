using System;
using System.Collections.Generic;

namespace UI.Shell.Interfaces
{
    public interface IMenuBuilderCatalog
    {
        IEnumerable<Type> DiscoverViewTypes();
    }
}