using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace GMF.Tags
{
	[System.Serializable]
	public sealed class TaggedContainerController
	{
		[field: SerializeField]
		public TagsIdCollection GeneralTags { get; private set; }

		public void Update()
		{

			Debug.Log($"Update {string.Join(" ", GeneralTags.GetAsTags().Select(arg => arg.ToString()))}");
		}
	}

	public interface ITagsCollector
	{
		public List<TagsIdCollection> GetAllTags();
	}

	public interface ITaggedContainer
	{
		public TaggedContainerController TaggedContainer { get; }

		public List<TagsIdCollection> GetAllTags()
		{
			var taged = new List<ITaggedContainer>();
			var fields = GetAllFields(this.GetType());

			foreach (var field in fields)
			{
				var disable = field.FieldType.GetCustomAttribute<DisableGeneralTagsAttribute>() != null;
				if (disable) continue;

				var fieldValue = field.GetValue(this);
				if (fieldValue == null) continue;


				if (typeof(ITaggedContainer).IsAssignableFrom(field.FieldType))
				{
					taged.Add(fieldValue as ITaggedContainer);
				}
				// Check if the field is a collection of ITagged
				else if (typeof(IEnumerable<ITaggedContainer>).IsAssignableFrom(field.FieldType))
				{
					var collection = fieldValue as IEnumerable<ITaggedContainer>;
					foreach (var item in collection)
					{
						if (item != null && typeof(ITaggedContainer).IsAssignableFrom(item.GetType()))
						{
							taged.Add(item);
						}
					}
				}
			}
			return taged.SelectMany(arg => arg.GetAllTags()).ToList();
		}

		IEnumerable<FieldInfo> GetAllFields(Type type)
		{
			if (type == null)
				return Enumerable.Empty<FieldInfo>();

			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			List<FieldInfo> fields = new List<FieldInfo>(type.GetFields(flags));

			Type baseType = type.BaseType;
			while (baseType != null)
			{
				fields.AddRange(baseType.GetFields(flags));
				baseType = baseType.BaseType;
			}

			return fields;
		}

	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class DisableGeneralTagsAttribute : Attribute
	{

		public DisableGeneralTagsAttribute()
		{

		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class StopDeepSearchAttribute : Attribute
	{

	}

	public static class ReflectionHelper
	{
		public static List<T> FindFieldsOfType<T>(object obj)
		{
			List<T> result = new List<T>();
			HashSet<object> visited = new HashSet<object>(new ReferenceEqualityComparer());
			FindFieldsOfTypeRecursive(obj, result, visited);
			return result;
		}

		private static void FindFieldsOfTypeRecursive<T>(object obj, List<T> result, HashSet<object> visited)
		{
			if (obj == null || visited.Contains(obj))
				return;

			// Добавляем объект в посещенные
			visited.Add(obj);

			Type objType = obj.GetType();

			// Получаем все поля, включая поля базовых классов
			foreach (FieldInfo field in GetAllFields(objType))
			{
				if (field.GetCustomAttribute<StopDeepSearchAttribute>() != null)
					continue;

				object fieldValue = field.GetValue(obj);

				if (fieldValue == null)
					continue;

				// Если поле имеет указанный тип T, добавляем его в список
				if (fieldValue is T)
				{
					result.Add((T)fieldValue);
				}
				// Если поле является коллекцией, проверяем каждый элемент коллекции
				else if (fieldValue is IEnumerable enumerable && !(fieldValue is string))
				{
					foreach (var item in enumerable)
					{
						if (item != null && item is T)
						{
							result.Add((T)item);
						}
						else
						{
							// Рекурсивно углубляемся, если элемент не является T
							FindFieldsOfTypeRecursive(item, result, visited);
						}
					}
				}
				else if (!field.FieldType.IsValueType && field.FieldType != typeof(string))
				{
					// Рекурсивно углубляемся в объекты других типов, которые не являются примитивными типами или строками
					FindFieldsOfTypeRecursive(fieldValue, result, visited);
				}
			}
		}

		private static IEnumerable<FieldInfo> GetAllFields(Type type)
		{
			List<FieldInfo> fields = new List<FieldInfo>();

			while (type != null && type != typeof(object))
			{
				fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				type = type.BaseType;
			}

			return fields;
		}
	}

	public class ReferenceEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object x, object y)
		{
			return ReferenceEquals(x, y);
		}

		public int GetHashCode(object obj)
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
		}
	}

}