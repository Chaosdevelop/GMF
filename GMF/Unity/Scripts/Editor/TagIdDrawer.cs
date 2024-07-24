using System.Linq;
using GMF.Tags;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagId), true)]
public class TagIdPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty idProperty = property.FindPropertyRelative("id");
        uint id = idProperty.uintValue;

        Tag tag = TagManager.GetTagById(id) as Tag;
        string[] tagNames = TagManager.GetAllTags().Select(t => (t as Tag).ToDropdownString()).ToArray();
        uint[] tagIds = TagManager.GetAllTags().Select(t => t.Id).ToArray();

        int selectedIndex = Mathf.Max(0, System.Array.IndexOf(tagIds, id));

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, tagNames);
        if (EditorGUI.EndChangeCheck())
        {
            idProperty.uintValue = tagIds[selectedIndex];
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        var proptoheight = property.isExpanded ? property : property.FindPropertyRelative("id");

        return EditorGUI.GetPropertyHeight(proptoheight, label, true);
    }
}