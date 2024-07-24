using System.Collections.Generic;
using System.Linq;
using GMF.Tags;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityTagsRepository))]
public class UnityTagsRepositoryEditor : Editor
{
    private UnityTagsRepository repository;
    private string newTagName = "";
    private string newSubTagName = "";
    private Dictionary<uint, bool> tagFoldouts = new Dictionary<uint, bool>();

    private void OnEnable()
    {
        repository = (UnityTagsRepository) target;

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        EditorGUILayout.LabelField("Tags Editor", EditorStyles.boldLabel);

        bool collectionModified = false;

        foreach (var tag in repository.GetAllTags())
        {
            if (!tagFoldouts.ContainsKey(tag.Id))
            {
                tagFoldouts[tag.Id] = false;
            }

            EditorGUILayout.BeginVertical("box");
            tagFoldouts[tag.Id] = EditorGUILayout.Foldout(tagFoldouts[tag.Id], $"{tag.Name}", true);

            if (tagFoldouts[tag.Id])
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"ID:{tag.Id}", GUILayout.MaxWidth(80));
                EditorGUILayout.LabelField("Name:", GUILayout.MaxWidth(50));
                tag.Name = EditorGUILayout.TextField(tag.Name);
                EditorGUILayout.EndHorizontal();

                // Редактирование подчиненных тегов
                EditorGUILayout.LabelField("SubTags", EditorStyles.boldLabel);
                var subTags = tag.GetSubTags();

                if (subTags != null && subTags.Count() > 0)
                {
                    foreach (var subTag in subTags)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{subTag.Name}", GUILayout.MaxWidth(200));
                        if (GUILayout.Button("Remove From Group", GUILayout.MaxWidth(150)))
                        {
                            TagManager.RemoveTagFromGroup(tag, subTag);
                            collectionModified = true;

                            break;
                        }
                        if (GUILayout.Button("Remove Everywere", GUILayout.MaxWidth(150)))
                        {
                            TagManager.RemoveTag(subTag);
                            collectionModified = true;
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                }
                else
                {
                    EditorGUILayout.LabelField("No SubTags", EditorStyles.boldLabel);
                }

                // Добавление нового подчиненного тега
                newSubTagName = EditorGUILayout.TextField("New SubTag Name:", newSubTagName);
                if (GUILayout.Button("Add SubTag"))
                {
                    if (!string.IsNullOrEmpty(newSubTagName))
                    {
                        var newSubTag = TagManager.CreateTag(newSubTagName);
                        tag.AddSubTag(newSubTag);
                        newSubTagName = "";
                        collectionModified = true;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "SubTag name cannot be empty!", "OK");
                    }
                }

                if (GUILayout.Button("Remove Tag"))
                {
                    TagManager.RemoveTag(tag);
                    collectionModified = true;
                }
            }

            EditorGUILayout.EndVertical();
            if (collectionModified)
            {
                GUI.changed = true;

                break;
            }
        }

        EditorGUILayout.Space();

        // Добавление нового тега
        EditorGUILayout.LabelField("Add New Tag", EditorStyles.boldLabel);
        newTagName = EditorGUILayout.TextField("Tag Name", newTagName);

        if (GUILayout.Button("Add Tag"))
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                TagManager.CreateTag(newTagName);
                newTagName = "";
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Tag name cannot be empty!", "OK");
            }
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(repository);
        }
    }
}
