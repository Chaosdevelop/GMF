using System.Collections.Generic;
using System.Linq;
using GMF.Tags;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "UnityTagsRepository", menuName = "ScriptableObjects/UnityTagsRepository")]
public class UnityTagsRepository : InstallableScriptableObject, ITagRepository
{
    private static UnityTagsRepository instance;

    public override ServiceDescriptor GetServiceDescriptor()
    {
        return new ServiceDescriptor(typeof(ITagRepository), this);
    }

    void OnValidate()
    {
        instance = this;
        TagManager.Initialize(instance);
    }

    void OnEnable()
    {
        instance = this;
        TagManager.Initialize(instance);
    }



    [SerializeField]
    List<Tag> tags;

    [HideInInspector]
    [SerializeField]
    uint nextId = 1;

    public ITag GetTagById(uint id)
    {
        return tags.FirstOrDefault(arg => arg.Id == id);
    }

    public ITag GetTagByName(string name)
    {
        return tags.FirstOrDefault(arg => arg.Name == name);
    }

    public IEnumerable<ITag> GetAllTags()
    {
        return tags;
    }

    public ITag CreateTag(string name)
    {
        var existingITag = GetTagByName(name);
        if (existingITag != null)
            return existingITag;

        var ITag = new Tag(nextId++, name);
        tags.Add(ITag);
        return ITag;
    }

    public void RemoveTag(ITag tag)
    {
        RemoveTagFromGroups(tag);
        tags.Remove((Tag) tag);

    }

    public void RemoveTagFromGroups(ITag tagToRemove)
    {
        foreach (var parentTag in tags)
        {
            RemoveTagFromGroup(parentTag, tagToRemove);

        }
    }

    public void RemoveTagFromGroup(ITag parentTag, ITag tagToRemove)
    {
        if (parentTag.GetSubTags() != null && parentTag.GetSubTags().Count() > 0)
        {
            ((Tag) parentTag).RemoveSubTag(tagToRemove);
        }
    }


}
