using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMF
{
	/// <summary>
	/// Provides utility methods for generating random values.
	/// </summary>
	public static class Randoms
	{
		static System.Random defaultRandomGen;
		static ConcurrentDictionary<int, System.Random> bgThreadGens;
		static bool copyingForThreads = false;
		static int bgThreadGenSeed;

		static Randoms()
		{
			defaultRandomGen = new System.Random();
			bgThreadGens = new ConcurrentDictionary<int, System.Random>();
		}

		/// <summary>
		/// Gets a floating point number that is greater than or equal to 0.0, and less than 1.0.
		/// </summary>
		public static float Value => GetValue();

		/// <summary>
		/// Enables copying the random generator for background threads.
		/// </summary>
		public static void EnableCopyingForBackgroundThreads()
		{
			copyingForThreads = true;
			bgThreadGenSeed = System.DateTime.Now.Second;
			SetSeed(bgThreadGenSeed);
		}

		/// <summary>
		/// Disables copying the random generator for background threads.
		/// </summary>
		public static void DisableCopyingForBackgroundThreads()
		{
			copyingForThreads = false;
			bgThreadGens.Clear();
		}

		/// <summary>
		/// Sets the seed for the random generator.
		/// </summary>
		/// <returns>The seed used.</returns>
		public static int SetSeed()
		{
			int seed = System.DateTime.Now.Second;
			SetSeed(seed);
			return seed;
		}

		/// <summary>
		/// Sets the seed for the random generator.
		/// </summary>
		/// <param name="seed">The seed to use.</param>
		public static void SetSeed(int seed)
		{
			defaultRandomGen = new System.Random(seed);
		}

		/// <summary>
		/// Returns a random boolean value.
		/// </summary>
		/// <returns>True or false, randomly.</returns>
		public static bool Bool()
		{
			return GetRandomGen().NextDouble() > 0.5f;
		}

		/// <summary>
		/// Returns a random integer between the specified float range.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>A random integer between the specified range.</returns>
		public static int IntFromFloat(float min, float max)
		{
			var rnd = Range(min, max);
			rnd += 0.5f;
			return Mathf.FloorToInt(rnd);
		}

		/// <summary>
		/// Returns a random float between the specified range.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>A random float between the specified range.</returns>
		public static float Range(float min, float max)
		{
			return Mathf.Lerp(min, max, (float)GetRandomGen().NextDouble());
		}

		/// <summary>
		/// Returns a random integer between the specified range.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>A random integer between the specified range.</returns>
		public static int Range(int min, int max)
		{
			return GetRandomGen().Next(min, max);
		}

		static float GetValue()
		{
			return (float)GetRandomGen().NextDouble();
		}

		static System.Random GetRandomGen()
		{
			var currentThread = System.Threading.Thread.CurrentThread;
			if (!copyingForThreads || !currentThread.IsBackground)
			{
				return defaultRandomGen;
			}
			else
			{
				int threadId = currentThread.ManagedThreadId;
				if (!bgThreadGens.TryGetValue(threadId, out var threadRandomGen))
				{
					threadRandomGen = new System.Random(bgThreadGenSeed);
					if (!bgThreadGens.TryAdd(threadId, threadRandomGen))
					{
						throw new System.Exception("Thread failed to add new random number generator!");
					}
				}
				return threadRandomGen;
			}
		}

		/// <summary>
		/// Determines if a random event with the specified chance is successful.
		/// </summary>
		/// <param name="chance">The chance of success, between 0 and 1.</param>
		/// <returns>True if the event is successful, otherwise false.</returns>
		public static bool SuccessfulChance(float chance)
		{
			return Value <= chance;
		}
	}

	/// <summary>
	/// Represents an item with an associated probability.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	public class ItemProbPair<T>
	{
		public T value;
		public float prob;
		public float pos;
	}

	/// <summary>
	/// A collection of items with associated probabilities for random selection.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	public class RandomExt<T> : IEnumerable<ItemProbPair<T>>
	{
		List<ItemProbPair<T>> list = new List<ItemProbPair<T>>();

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			list.Clear();
		}

		/// <summary>
		/// Adds an item with the specified probability to the collection.
		/// </summary>
		/// <param name="value">The item to add.</param>
		/// <param name="prob">The probability of the item.</param>
		public void Add(T value, float prob)
		{
			list.Add(new ItemProbPair<T>
			{
				value = value,
				prob = prob
			});
		}

		/// <summary>
		/// Sets the probability of an existing item in the collection.
		/// </summary>
		/// <param name="value">The item.</param>
		/// <param name="prob">The new probability.</param>
		public void Set(T value, float prob)
		{
			var item = list.Find(x => x.value.Equals(value));
			item.prob = prob;
		}

		/// <summary>
		/// Imports items with probabilities from a dictionary.
		/// </summary>
		/// <param name="dict">The dictionary to import from.</param>
		public void Import(Dictionary<T, float> dict)
		{
			foreach (var pair in dict)
			{
				Add(pair.Key, pair.Value);
			}
		}

		/// <summary>
		/// Updates the probabilities of items in the collection based on a dictionary.
		/// </summary>
		/// <param name="dict">The dictionary to update from.</param>
		public void Update(Dictionary<T, float> dict)
		{
			foreach (var item in list)
			{
				if (dict.TryGetValue(item.value, out float prob))
				{
					item.prob = prob;
				}
			}
		}

		/// <summary>
		/// Recalculates the positions for each item based on their probabilities.
		/// </summary>
		public void Recalc()
		{
			if (list.Count == 0)
			{
				return;
			}

			list[0].pos = list[0].prob;
			for (int i = 1; i < list.Count; i++)
			{
				list[i].pos = list[i - 1].pos + list[i].prob;
			}
			list.Sort((x, y) => x.pos.CompareTo(y.pos));
		}

		/// <summary>
		/// Gets the maximum position value in the collection.
		/// </summary>
		/// <returns>The maximum position value.</returns>
		public float MaxPos()
		{
			if (list.Count == 0)
			{
				return 0;
			}
			return list.Last().pos;
		}

		/// <summary>
		/// Gets the item at the specified position.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>The item at the specified position.</returns>
		public T ValueAtPos(float pos)
		{
			return list[FindIndex(pos)].value;
		}

		/// <summary>
		/// Finds the index of the item at the specified position.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>The index of the item at the specified position.</returns>
		public int FindIndex(float pos)
		{
			if (list.Count == 0)
			{
				return 0;
			}
			int lo = 0;
			int hi = list.Count - 1;
			if (list[lo].pos > pos)
			{
				return 0;
			}
			if (list[hi].pos < pos)
			{
				return list.Count - 1;
			}
			while (lo < hi)
			{
				int mid = (lo + hi) / 2;
				if (pos > list[mid].pos)
				{
					lo = mid + 1;
				}
				else
				{
					hi = mid;
				}
			}
			return lo;
		}

		public IEnumerator<ItemProbPair<T>> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	/// <summary>
	/// A randomizer for selecting weighted items.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	public class WeightRandomizer<T>
	{
		[System.Serializable]
		public struct WeightedItem
		{
			public float weight;
			public T item;

			public WeightedItem(float weight, T item)
			{
				this.weight = weight;
				this.item = item;
			}
		}

		List<WeightedItem> options = new List<WeightedItem>();

		/// <summary>
		/// Adds options from a dictionary of weights.
		/// </summary>
		/// <param name="dict">The dictionary of weights.</param>
		public void AddOptions(Dictionary<T, float> dict)
		{
			foreach (var kvp in dict)
			{
				options.Add(new WeightedItem(kvp.Value, kvp.Key));
			}
		}

		/// <summary>
		/// Adds options from a dictionary of weights.
		/// </summary>
		/// <param name="dict">The dictionary of weights.</param>
		public void AddOptions(Dictionary<T, int> dict)
		{
			foreach (var kvp in dict)
			{
				options.Add(new WeightedItem(kvp.Value, kvp.Key));
			}
		}

		/// <summary>
		/// Adds options from a collection of weighted items.
		/// </summary>
		/// <param name="collection">The collection of weighted items.</param>
		public void AddOptions(ICollection<WeightedItem> collection)
		{
			foreach (var item in collection)
			{
				options.Add(item);
			}
		}

		/// <summary>
		/// Adds a weighted option.
		/// </summary>
		/// <param name="weight">The weight of the item.</param>
		/// <param name="item">The item.</param>
		public void AddOption(int weight, T item)
		{
			options.Add(new WeightedItem(weight, item));
		}

		/// <summary>
		/// Adds a weighted option.
		/// </summary>
		/// <param name="weight">The weight of the item.</param>
		/// <param name="item">The item.</param>
		public void AddOption(float weight, T item)
		{
			options.Add(new WeightedItem(weight, item));
		}

		/// <summary>
		/// Adds a weighted option.
		/// </summary>
		/// <param name="item">The weighted item.</param>
		public void AddOption(WeightedItem item)
		{
			options.Add(item);
		}

		/// <summary>
		/// Gets a weighted random option.
		/// </summary>
		/// <returns>A weighted random option.</returns>
		public T GetOption()
		{
			float totalWeight = options.Sum(item => item.weight);
			float result = Randoms.Range(0, totalWeight);

			foreach (var item in options)
			{
				totalWeight -= item.weight;
				if (totalWeight < result)
				{
					return item.item;
				}
			}

			return options.Count > 0 ? options.First().item : default;
		}

		/// <summary>
		/// Pops a weighted random option.
		/// </summary>
		/// <returns>The popped weighted random option.</returns>
		public T PopOption()
		{
			float totalWeight = options.Sum(item => item.weight);
			float result = Randoms.Range(0, totalWeight);

			foreach (var item in options)
			{
				totalWeight -= item.weight;
				if (totalWeight < result)
				{
					options.Remove(item);
					return item.item;
				}
			}

			return options.Count > 0 ? options.First().item : default;
		}
	}
}
