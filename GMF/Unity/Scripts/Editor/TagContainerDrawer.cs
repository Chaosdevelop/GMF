using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GMF.Tags;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TaggedContainerController))]
public class TagContainerDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		//EditorGUI.BeginChangeCheck();

		//EditorGUI.PropertyField(position, property, label, true);

		var tags = TagManager.GetAllTags();
		string[] tagNames = tags.Select(t => (t as Tag).GetGroupedName(50)).ToArray();
		uint[] tagIds = tags.Select(t => t.Id).ToArray();

		int removeIndex = -1;
		var removerect = new Rect(position);
		removerect.y += EditorGUIUtility.singleLineHeight;
		removeIndex = EditorGUI.Popup(removerect, "Remove General Tag", removeIndex, tagNames);

		if (removeIndex >= 0)
		{
			var tagremove = TagManager.GetTagById(tagIds[removeIndex]);
			var parent = GetParentObject(property);
			deepValues.Clear();
			RemoveTag(parent, tagremove);
			EditorUtility.SetDirty(property.serializedObject.targetObject);
		}

		int addIndex = -1;
		var addrect = new Rect(position);
		addIndex = EditorGUI.Popup(addrect, "Add General Tag", addIndex, tagNames);

		if (addIndex >= 0)
		{
			var tagadd = TagManager.GetTagById(tagIds[addIndex]);
			var parent = GetParentObject(property);

			deepValues.Clear();
			AddTag(parent, tagadd);
			EditorUtility.SetDirty(property.serializedObject.targetObject);
		}

	}

	private static IEnumerable<FieldInfo> GetAllFields(Type type)
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

	HashSet<object> deepValues = new HashSet<object>();

	void AddTag(object parent, ITag tag)
	{
		if (deepValues.Contains(parent)) return;
		deepValues.Add(parent);

		var fields = GetAllFields(parent.GetType());

		foreach (var field in fields)
		{
			var disable = field.FieldType.GetCustomAttribute<DisableGeneralTagsAttribute>() != null;
			if (disable) continue;

			var fieldValue = field.GetValue(parent);
			if (fieldValue == null) continue;

			// Check if the field is of type ITagged
			if (typeof(ITagged).IsAssignableFrom(field.FieldType))
			{
				var method = typeof(ITagged).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				method?.Invoke(fieldValue, new object[] { tag });
				AddTag(fieldValue, tag);
			}
			// Check if the field is a collection of ITagged
			else if (typeof(IEnumerable<ITagged>).IsAssignableFrom(field.FieldType))
			{
				var collection = fieldValue as IEnumerable<ITagged>;
				foreach (var item in collection)
				{
					if (item != null && typeof(ITagged).IsAssignableFrom(item.GetType()))
					{
						var method = typeof(ITagged).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						method?.Invoke(item, new object[] { tag });
						AddTag(item, tag);
					}
				}
			}
			/*
						if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
						{
							var collection = fieldValue as IEnumerable;
							foreach (var item in collection)
							{
								AddTag(item, tag);
							}
						}
						else
						{
							AddTag(fieldValue, tag);
						}*/


		}
	}
	void RemoveTag(object parent, ITag tag)
	{
		if (deepValues.Contains(parent)) return;
		deepValues.Add(parent);

		var fields = GetAllFields(parent.GetType());

		foreach (var field in fields)
		{
			var disable = field.FieldType.GetCustomAttribute<DisableGeneralTagsAttribute>() != null;
			if (!disable)
			{
				var fieldValue = field.GetValue(parent);
				if (fieldValue != null)
				{
					// Check if the field is of type ITagged
					if (typeof(ITagged).IsAssignableFrom(field.FieldType))
					{
						var method = typeof(ITagged).GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						method?.Invoke(fieldValue, new object[] { tag });
						RemoveTag(fieldValue, tag);
					}
					// Check if the field is a collection of ITagged
					else if (typeof(IEnumerable<ITagged>).IsAssignableFrom(field.FieldType))
					{
						var collection = fieldValue as IEnumerable<ITagged>;
						foreach (var item in collection)
						{
							if (item != null && typeof(ITagged).IsAssignableFrom(item.GetType()))
							{
								var method = typeof(ITagged).GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
								method?.Invoke(item, new object[] { tag });
								RemoveTag(item, tag);
							}
						}
					}
					/*
										if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
										{
											var collection = fieldValue as IEnumerable;
											foreach (var item in collection)
											{
												RemoveTag(item, tag);
											}
										}
										else
										{
											RemoveTag(fieldValue, tag);
										}*/
				}
			}
		}
	}

	private object GetParentObject(SerializedProperty property)
	{
		string[] path = property.propertyPath.Split('.');

		object obj = property.serializedObject.targetObject;

		foreach (string part in path.Take(path.Length - 1))
		{
			obj = GetFieldValue(obj, part);
		}

		return obj;
	}
	private object GetFieldValue(object source, string fieldName)
	{
		if (source == null)
			return null;

		Type type = source.GetType();
		FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

		if (field == null)
			return null;

		return field.GetValue(source);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{

		//return EditorGUI.GetPropertyHeight(property, label, true);
		return EditorGUIUtility.singleLineHeight * 2 + 2;
	}
}
