using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Utility.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static void Swap<T>(this OptimizedObservableCollection<T> collection, T obj1, T obj2)
        {
            if (!(collection.Contains(obj1) && collection.Contains(obj2))) return;
            var indexes = new List<int> { collection.IndexOf(obj1), collection.IndexOf(obj2) };
            if (indexes[0] == indexes[1]) return;
            indexes.Sort();
            var values = new List<T> { collection[indexes[0]], collection[indexes[1]] };
            collection.RemoveAt(indexes[1]);
            collection.RemoveAt(indexes[0]);
            collection.Insert(indexes[0], values[1]);
            collection.Insert(indexes[1], values[0]);
        }

        /// <summary>
        /// Creates PropertyChangedEventArgs
        /// </summary>
        /// <param name="propertyExpression">Expression to make 
        /// PropertyChangedEventArgs out of</param>
        /// <returns>PropertyChangedEventArgs</returns>
        public static PropertyChangedEventArgs CreateArgs<T>(Expression<Func<T, Object>> propertyExpression)
        {
            return new PropertyChangedEventArgs(GetPropertyName<T>(propertyExpression));
        }

        /// <summary>
        /// Creates PropertyChangedEventArgs
        /// </summary>
        /// <param name="propertyExpression">Expression to make 
        /// PropertyChangedEventArgs out of</param>
        /// <returns>PropertyChangedEventArgs</returns>
        public static string GetPropertyName<T>(Expression<Func<T, Object>> propertyExpression)
        {
            var lambda = propertyExpression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;

            return propertyInfo.Name;
        }
    }

    public class OptimizedObservableCollection<T> : ObservableCollection<T>
    {
        private bool suppressOnCollectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizedObservableCollection{T}"/> class.
        /// </summary>
        public OptimizedObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizedObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="items">The collection from which the items are copied.</param>
        public OptimizedObservableCollection(IEnumerable<T> items)
            : base(items)
        {
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Any())
            {
                try
                {
                    suppressOnCollectionChanged = true;
                    foreach (var item in items)
                    {
                        Add(item);
                    }
                }
                finally
                {
                    suppressOnCollectionChanged = false;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Any())
            {
                try
                {
                    suppressOnCollectionChanged = true;
                    foreach (var item in items)
                    {
                        Remove(item);
                    }
                }
                finally
                {
                    suppressOnCollectionChanged = false;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public void RemoveAll()
        {
            RemoveRange(Items.ToList());
        }

        public void ReplaceWith(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            ReplaceWith(new T[] { item });
        }

        /// <summary>
        /// Replaces the current <see cref="OptimizedObservableCollection{T}"/> instance items with the ones specified in the items collection, raising a single <see cref="NotifyCollectionChangedAction.Reset"/> event.
        /// </summary>
        /// <param name="items">The collection from which the items are copied.</param>
        /// <exception cref="ArgumentNullException">The items list is null.</exception>
        public void ReplaceWith(IEnumerable<T> items)
        {
            Items.Clear();

            AddRange(items);
        }

        /// <summary>
        /// Switches the current <see cref="OptimizedObservableCollection{T}"/> instance items with the ones specified in the items collection, raising the minimum required change events.
        /// </summary>
        /// <param name="items">The collection from which the items are copied.</param>
        /// <exception cref="ArgumentNullException">The items list is null.</exception>
        public void SwitchTo(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            var itemIndex = 0;
            var count = Count;

            foreach (var item in items)
            {
                if (itemIndex >= count)
                {
                    Add(item);
                }
                else if (!Equals(this[itemIndex], item))
                {
                    this[itemIndex] = item;
                }

                itemIndex++;
            }

            while (count > itemIndex)
            {
                this.RemoveAt(--count);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!suppressOnCollectionChanged)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
