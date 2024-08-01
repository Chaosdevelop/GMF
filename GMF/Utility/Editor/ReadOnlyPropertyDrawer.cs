using GMF.Utility;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom property drawer for ReadOnlyAttribute in the Unity editor.
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
	/// <summary>
	/// Gets the height of the property.
	/// </summary>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	/// <summary>
	/// Renders the property as read-only in the Unity editor.
	/// </summary>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;
	}
}
