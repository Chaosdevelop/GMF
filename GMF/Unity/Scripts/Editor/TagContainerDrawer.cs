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
		string[] tagNames = tags.Select(t => (t as Tag).ToDropdownString()).ToArray();
		uint[] tagIds = tags.Select(t => t.Id).ToArray();

		int removeIndex = -1;
		var removerect = new Rect(position);
		removerect.y += EditorGUIUtility.singleLineHeight;
		removeIndex = EditorGUI.Popup(removerect, "Remove General Tag", removeIndex, tagNames);

		if (removeIndex >= 0)
		{
			var tagremove = TagManager.GetTagById(tagIds[removeIndex]);
			var parent = GetParentObject(property);
			RemoveTag(parent, tagremove);
		}

		int addIndex = -1;
		var addrect = new Rect(position);
		addIndex = EditorGUI.Popup(addrect, "Add General Tag", addIndex, tagNames);

		if (addIndex >= 0)
		{
			var tagadd = TagManager.GetTagById(tagIds[addIndex]);
			var parent = GetParentObject(property);
			AddTag(parent, tagadd);
		}




		//if (EditorGUI.EndChangeCheck())
		{


			/*			// �������� ������ ��������
						var targetObject = property.serializedObject.targetObject;

						// ���������� Reflection ��� ��������� �������� ��������

						var fieldInfo = targetObject.GetType().GetProperty("TaggedContainer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

						if (fieldInfo != null)
						{
							var fieldValue = fieldInfo.GetValue(targetObject);

							// ���������, ��������� �� �������� �������� ��������� ITaggedContainer
							if (fieldValue is TaggedContainerController taggedContainer)
							{
								// �������� ����� Update �� �������� ��������
								*//*					taggedContainer.Update();
													var parent = GetParentObject(property);
													AddTags<ITaggedValue>(parent, taggedContainer.GeneralTags);*//*

							}
						}*/
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

	void AddTag(object parent, ITag tag)
	{
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
				}
			}
		}
	}
	void RemoveTag(object parent, ITag tag)
	{
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