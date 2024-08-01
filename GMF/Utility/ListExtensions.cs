using System;
using System.Collections.Generic;

namespace GMF.Collections
{
	/// <summary>
	/// Extension methods for IList.
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Removes all null values from the list.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the list.</typeparam>
		/// <param name="source">The list to remove null values from.</param>
		public static void RemoveNullValues<T>(this IList<T> source) where T : class
		{
			while (source.Remove(null)) { }
		}

		/// <summary>
		/// Picks a random element from the list.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the list.</typeparam>
		/// <param name="source">The list to pick a random element from.</param>
		/// <param name="rnd">An optional random number generator.</param>
		/// <returns>A randomly selected element from the list.</returns>
		public static T PickRandom<T>(this IList<T> source, System.Random rnd = null)
		{
			if (source.Count == 0)
				return default(T);

			int i = rnd?.Next(0, source.Count) ?? GMF.Randoms.Range(0, source.Count);

			return source[i];
		}

		/// <summary>
		/// Removes and returns a random element from the list.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the list.</typeparam>
		/// <param name="source">The list to remove a random element from.</param>
		/// <param name="rnd">An optional random number generator.</param>
		/// <returns>The randomly removed element from the list.</returns>
		public static T PopRandom<T>(this IList<T> source, System.Random rnd = null)
		{
			if (source.Count == 0)
				return default(T);

			int i = rnd?.Next(0, source.Count) ?? GMF.Randoms.Range(0, source.Count);
			T value = source[i];
			source.RemoveAt(i);

			return value;
		}

		/// <summary>
		/// Shuffles the elements in the list.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the list.</typeparam>
		/// <param name="list">The list to shuffle.</param>
		/// <param name="rnd">An optional random number generator.</param>
		public static void Shuffle<T>(this IList<T> list, System.Random rnd = null)
		{
			int index = list.Count;
			while (index > 1)
			{
				index--;
				int newPos = rnd?.Next(0, index + 1) ?? GMF.Randoms.Range(0, index + 1);

				(list[newPos], list[index]) = (list[index], list[newPos]);
			}
		}


		public static void Add<T>(this T[] array, T item, out T[] result)
		{
			if (array is null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			Array.Resize(ref array, array.Length + 1);
			array[^1] = item;
			result = array;
		}

		public static void Remove<T>(this T[] array, T item, out T[] result)
		{
			if (array == null) throw new ArgumentNullException(nameof(array));

			int index = Array.IndexOf(array, item);
			if (index == -1)
			{
				result = array;
				return;
			}

			for (int i = index; i < array.Length - 1; i++)
			{
				array[i] = array[i + 1];
			}

			Array.Resize(ref array, array.Length - 1);
			result = array;

		}
	}
}
