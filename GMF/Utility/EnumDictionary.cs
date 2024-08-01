using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GMF.Utility;
using UnityEngine;

namespace GMF.Collections
{
	/// <summary>
	/// A serializable dictionary for enum keys, supporting Unity's serialization system.
	/// </summary>
	/// <typeparam name="T">The enum type used as keys.</typeparam>
	/// <typeparam name="T2">The type of the values.</typeparam>
	[Serializable]
	public class EnumDictionary<T, T2> : IEnumerable<KeyValuePair<T, T2>>, ISerializationCallbackReceiver where T : Enum
	{
		[SerializeField]
		List<Pair> list = new List<Pair>();

		Dictionary<T, T2> dict = new Dictionary<T, T2>();

		[Serializable]
		class Pair
		{
			[ReadOnly]
			public T key;
			public T2 value;
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EnumDictionary()
		{
			InitValues();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other">The dictionary to copy.</param>
		public EnumDictionary(EnumDictionary<T, T2> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other), "Other dictionary cannot be null.");
			}

			list = new List<Pair>(other.list);
			dict = new Dictionary<T, T2>(other.dict);
		}

		/// <summary>
		/// Initializes the dictionary with default values based on the enum type.
		/// </summary>
		void InitValues()
		{
			foreach (T t in Enum.GetValues(typeof(T)))
			{
				if (!list.Any(arg => arg.key.Equals(t)))
				{
					list.Add(new Pair { key = t, value = default });
				}
			}

			list.RemoveAll(arg => !Enum.IsDefined(typeof(T), arg.key));

			dict.Clear();
			foreach (var pair in list)
			{
				dict[pair.key] = pair.value;
			}
		}

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get or set.</param>
		/// <returns>The value associated with the specified key.</returns>
		/// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when the key is not found in the dictionary.</exception>
		public T2 this[T key] {
			get {
				if (key == null)
				{
					throw new ArgumentNullException(nameof(key), "Key cannot be null.");
				}

				if (!dict.ContainsKey(key))
				{
					throw new KeyNotFoundException($"Key '{key}' not found in dictionary.");
				}

				return dict[key];
			}
			set {
				if (key == null)
				{
					throw new ArgumentNullException(nameof(key), "Key cannot be null.");
				}

				dict[key] = value;
			}
		}

		/// <summary>
		/// Called before serialization. Updates the list based on the dictionary.
		/// </summary>
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			foreach (var pair in dict)
			{
				var item = list.FirstOrDefault(arg => Equals(arg.key, pair.Key));
				if (item != null)
				{
					item.value = pair.Value;
				}
			}
		}

		/// <summary>
		/// Called after deserialization. Initializes the dictionary values.
		/// </summary>
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			InitValues();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the dictionary.
		/// </summary>
		/// <returns>An enumerator for the dictionary.</returns>
		public IEnumerator<KeyValuePair<T, T2>> GetEnumerator()
		{
			return dict.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return dict.GetEnumerator();
		}

		/// <summary>
		/// Returns the dictionary as a new object.
		/// </summary>
		/// <returns>A new dictionary with the same key-value pairs.</returns>
		public Dictionary<T, T2> AsDictionary()
		{
			return new Dictionary<T, T2>(dict);
		}

		/// <summary>
		/// Returns a string representation of the dictionary.
		/// </summary>
		/// <returns>A string that represents the dictionary.</returns>
		public override string ToString()
		{
			return string.Join("\n", dict.Select(item => $"{item.Key}:{item.Value}"));
		}
	}
}
