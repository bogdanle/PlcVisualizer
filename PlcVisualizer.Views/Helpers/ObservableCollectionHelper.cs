using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PlcVisualizer.Views.Helpers;

public static class ObservableCollectionHelper
{
    extension<T>(IEnumerable<T> items)
    {
        public ObservableCollection<T> ToObservableCollection()
        {
            var oc = new ObservableCollection<T>();
            foreach (var item in items)
            {
                oc.Add(item);
            }

            return oc;
        }

        public IEnumerable<T> Flatten(Func<T, IEnumerable<T>> childrenSelector)
        {
            if (items != null)
            {
                foreach (var item in items)
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