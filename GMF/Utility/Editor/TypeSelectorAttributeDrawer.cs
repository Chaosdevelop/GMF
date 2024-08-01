using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GMF.Utility;
using UnityEditor;
using UnityEngine;

namespace GMF.Editor
{
	/// <summary>
	/// Custom property drawer for TypeSelectorAttribute, adds a dropdown to select suitable types.
	/// </summary>
	[CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
	public class TypeSelectorAttributeDrawer : PropertyDrawer
	{
		List<Type> instantiableTypes = null;

		/// <summary>
		/// Renders the property in the Unity editor with a type selector dropdown.
		/// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			TypeSelectorAttribute typeSelectorAttribute = attribute as TypeSelectorAttribute;
			if (typeSelectorAttribute == null)
			{
				throw new ArgumentNullException(nameof(typeSelectorAttribute), "TypeSelectorAttribute is null.");
			}

			string[] baseTypeAndAssemblyName = property.managedReferenceFieldTypename.Split(' ');
			if (baseTypeAndAssemblyName.Length < 2)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			string baseTypeString = $"{baseTypeAndAssemblyName[1]}, {baseTypeAndAssemblyName[0]}";
			Type baseType = Type.GetType(baseTypeString);
			if (baseType == null)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			if (instantiableTypes == null)
			{
				instantiableTypes = TypeUtility.GetAllTypesDerivedFrom(baseType, true, false, true);
			}

			GUIContent[] options = new GUIContent[instantiableTypes.Count + 1];
			options[0] = new GUIContent("Null");
			List<int> deprecatedIndexes = new List<int>();
			int selectedIndex = 0;

			for (int i = 0; i < instantiableTypes.Count; i++)
			{
				Type type = instantiableTypes[i];
				string name = type.FullName;
				string typeAndAssemblyName = $"{type.Assembly.GetName().Name} {name}";
				var descriptionAttributes = type.GetCustomAttributes(typeof(SelectTypeDescriptionAttribute), true);
				var deprecatedAttributes = type.GetCustomAttributes(typeof(DeprecatedTypeAttribute), true);
				string typeDescription = string.Empty;

				if (descriptionAttributes.Length > 0)
				{
					var attr = descriptionAttributes[0] as SelectTypeDescriptionAttribute;
					typeDescription = attr?.TypeDescription;
				}

				if (deprecatedAttributes.Length > 0)
				{
					name = "(DEPRECATED) " + name;
					deprecatedIndexes.Add(i + 1);
				}

				options[i + 1] = new GUIContent(name, typeDescription);
				if (property.managedReferenceFullTypename == typeAndAssemblyName)
				{
					selectedIndex = i + 1;
				}
			}

			var selectionRect = new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.6f, EditorGUIUtility.singleLineHeight);
			int newSelectedIndex = EditorGUI.Popup(selectionRect, selectedIndex, options);

			if (selectedIndex != newSelectedIndex && !deprecatedIndexes.Contains(newSelectedIndex))
			{
				Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "selected type change");

				if (newSelectedIndex == 0)
				{
					property.managedReferenceValue = null;
				}
				else
				{
					Type selectedType = instantiableTypes[newSelectedIndex - 1];
					property.managedReferenceValue = FormatterServices.GetUninitializedObject(selectedType);
				}

				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.PropertyField(position, property, label, true);
		}

		/// <summary>
		/// Gets the height of the property for rendering in the inspector.
		/// </summary>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}
	}
}
