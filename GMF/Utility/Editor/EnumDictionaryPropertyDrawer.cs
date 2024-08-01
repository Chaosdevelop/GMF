using System.Linq;
using GMF.Collections;
using GMF.Utility;
using UnityEditor;
using UnityEngine;

namespace GMF.Editor
{
	/// <summary>
	/// Custom property drawer for EnumDictionary in the Unity editor.
	/// </summary>
	[CustomPropertyDrawer(typeof(EnumDictionary<,>), true)]
	public class EnumDictionaryPropertyDrawer : PropertyDrawer
	{
		const float KeyMinWidth = 150;
		const float KeyRelativeWidth = 0.4f;
		float height;

		/// <summary>
		/// Renders the custom property in the Unity editor.
		/// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var enumType = fieldInfo.FieldType.GetGenericArguments().FirstOrDefault();
			if (enumType == null)
			{
				height = EditorGUI.GetPropertyHeight(property);
				return;
			}

			var listProp = property.FindPropertyRelative("list");
			var size = listProp.arraySize;

			var rect = new Rect(position)
			{
				height = EditorGUIUtility.singleLineHeight
			};
			EditorGUI.LabelField(rect, label);
			EditorGUI.indentLevel++;
			rect.y += EditorGUIUtility.singleLineHeight;

			for (int i = 0; i < size; i++)
			{
				var pairProp = listProp.GetArrayElementAtIndex(i);
				var keyProp = pairProp.FindPropertyRelative("key");
				var valueProp = pairProp.FindPropertyRelative("value");

				var keyRect = new Rect(rect)
				{
					width = Mathf.Max(rect.width * KeyRelativeWidth, KeyMinWidth)
				};
				var valueRect = new Rect(rect)
				{
					width = rect.width - keyRect.width,
					x = keyRect.x + keyRect.width
				};

				var enumVal = (System.Enum)System.Enum.ToObject(enumType, keyProp.enumValueFlag);
				var enumGroup = enumVal.GetAttributeOfType<EnumGroupAttribute>();
				var enumGroupOnly = fieldInfo.GetCustomAttributes(typeof(OnlyEnumGroupsAttribute), true).FirstOrDefault() as OnlyEnumGroupsAttribute;
				bool addEnum = enumGroupOnly == null || (enumGroup != null && enumGroup.GroupNames.Intersect(enumGroupOnly.GroupNames).Any());

				if (enumGroup != null)
				{
					var enumGroupExcept = fieldInfo.GetCustomAttributes(typeof(ExceptEnumGroupsAttribute), true).FirstOrDefault() as ExceptEnumGroupsAttribute;
					if (enumGroupExcept != null && addEnum)
					{
						addEnum = !enumGroup.GroupNames.Intersect(enumGroupExcept.GroupNames).Any();
					}
				}

				if (addEnum)
				{
					EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none, true);
					if (valueProp != null)
					{
						EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none, true);
						rect.y += EditorGUI.GetPropertyHeight(valueProp);
					}
					else
					{
						EditorGUI.LabelField(valueRect, new GUIContent("Non serializable value"));
						rect.y += EditorGUI.GetPropertyHeight(keyProp);
					}
				}
			}

			height = rect.y - position.y;
			EditorGUI.indentLevel--;
		}

		/// <summary>
		/// Gets the property height for rendering in the inspector.
		/// </summary>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return height;
		}
	}
}
