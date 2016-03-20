using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JXWPFToolkit.Helpers
{
	public static class Extensions
	{
		public static IEnumerable<T> Descendants<T>(this DependencyObject parent) where T : DependencyObject
		{
			if (parent == null)
				yield break;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				if (child is T)
				{
					yield return (T)child;
				}

				foreach (T grandChild in Descendants<T>(child))
				{
					yield return grandChild;
				}
			}
		}

		public static bool IsValid(this DependencyObject obj)
		{
			return !Validation.GetHasError(obj) && !obj.Descendants<DependencyObject>().Any(Validation.GetHasError);
		}

		public static bool EqualsInvariant(this string source, string value1, params string[] values)
		{
			if (values == null)
				return (source.Equals(value1, StringComparison.InvariantCultureIgnoreCase));

			return values.Any(x => x.Equals(source, StringComparison.InvariantCultureIgnoreCase));
		}

		///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
		///<param name="items">The enumerable to search.</param>
		///<param name="predicate">The expression to test the items against.</param>
		///<returns>The index of the first matching item, or -1 if no items match.</returns>
		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			if (items == null) throw new ArgumentNullException("items");
			if (predicate == null) throw new ArgumentNullException("predicate");

			int retVal = 0;
			foreach (var item in items)
			{
				if (predicate(item)) return retVal;
				retVal++;
			}
			return -1;
		}
		///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
		///<param name="items">The enumerable to search.</param>
		///<param name="item">The item to find.</param>
		///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
		public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }
	}
}
