using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace UI.Core.Extensions;

/// <summary>
/// Extension methods for IEnumerable class.
/// </summary>
public static class EnumerableExtension
{
    /// <param name="items">The source collection.</param>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    extension<TEntity>(IEnumerable<TEntity> items)
    {
        /// <summary>
        /// Convert give collection of entity objects to a DataTable object.
        /// </summary>
        /// <returns>The DataTable object.</returns>
        public DataTable ToDataTable()
        {
            var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var dt = new DataTable();

            var names = new List<string>();

            foreach (var property in properties)
            {
                var result = property.GetCustomAttribute(typeof(NotMappedAttribute));
                if (result is NotMappedAttribute)
                {
                    continue;
                }

                if (property.PropertyType is { IsValueType: true, IsGenericType: true, Name: "Nullable`1" })
                {
                    dt.Columns.Add(new DataColumn(property.Name, property.PropertyType.GenericTypeArguments[0]));
                }
                else
                {
                    dt.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                names.Add(property.Name);
            }

            foreach (var item in items)
            {
                DataRow dataRow = dt.NewRow();
                foreach (string name in names)
                {
                    var pi = item.GetType().GetProperty(name);
                    object val = pi?.GetValue(item, null);

                    // Special case for double values that are NaN or Infinity - convert them all to nulls
                    if (val is double?)
                    {
                        double d = (double)val;
                        if (double.IsNaN(d) || double.IsInfinity(d))
                        {
                            val = null;
                        }
                    }

                    dataRow[name] = val ?? DBNull.Value;
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        /// <summary>
        /// Convert given collection of items to ObservableCollection.
        /// </summary>
        /// <returns>The observable collection of items.</returns>
        public ObservableCollection<TEntity> ToObservableCollection()
        {
            var collection = new ObservableCollection<TEntity>();
            foreach (var item in items)
            {
                collection.Add(item);
            }

            return collection;
        }

        /// <summary>
        /// Convert hierarchical data structure to a flat list.
        /// </summary>
        /// <param name="childrenSelector">The selector function.</param>
        /// <returns>New collection that is flat representation of hierarchical input data.</returns>
        public IEnumerable<TEntity> Flatten(Func<TEntity, IEnumerable<TEntity>> childrenSelector)
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

        public Task ForEachAsync(Func<TEntity, Task> body, int limit = 10)
        {
            return Task.WhenAll(Partitioner.Create(items)
                .GetPartitions(limit)
                .Select(partition => Task.Run(async () =>
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            await body(partition.Current);
                        }
                    }
                })));
        }
    }
}