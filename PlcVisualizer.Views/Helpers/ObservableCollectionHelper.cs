using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PlcVisualizer.Views.Helpers
{
    public static class ObservableCollectionHelper
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            var oc = new ObservableCollection<T>();
            foreach (var item in items)
            {
                oc.Add(item);
            }

            return oc;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    yield return item;
                    foreach (var child in childrenSelector(item).Flatten(childrenSelector))
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
