using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseCore.Collections;
using UnityEngine;


namespace GMF.Tags
{
    [Serializable]
    public class TagsIdCollection : ITagsIdCollection
    {
        [SerializeField]
        TagId[] tagIds;
        HashSet<ITag> tags;
        
        public int Count
        {
            get
            {
                return tagIds.Length;
            }
        }

        public bool IsEmpty => Count <= 0;

        public TagsIdCollection()
        {
            tagIds = new TagId[0];
        }

        public TagsIdCollection(IEnumerable<TagId> tagIds)
        {
            this.tagIds = tagIds.ToArray();
        }
        
        public bool Contains(TagId tagId)
        {
            return tagIds.Contains(tagId);
        }
        
        public bool Contains(ITag tag)
        {
            return GetAsTags().Contains(tag);
        }
        
        public void Add(TagId tagId)
        {
            if (TagManager.GetTagById(tagId.Id) is not { } tag)
            {
                return;
            }
            
            tagIds.Add(tagId, out tagIds);
            tags ??= new HashSet<ITag>();
            tags.Add(tag);
        }
        
        public void Remove(TagId tagId)
        {
            tagIds.Remove(tagId, out tagIds);
            
            if (TagManager.GetTagById(tagId.Id) is not { } tag)
            {
                return;
            }
            
            tags?.Remove(tag);
        }
        
        public IReadOnlyCollection<ITag> GetAsTags()
        {
            if (tags != null)
            {
                return tags;
            }

            tags = new HashSet<ITag>();
            
            for (int i = 0; i < tagIds.Length; i++)
            {
                if (TagManager.GetTagById(tagIds[i].Id) is not { } tag)
                {
                    continue;
                }
                
                tags.Add(tag);
            }
            
            return tags;
        }
        
        public override string ToString()
        {
            return string.Join(", ", tagIds);
        }
    }
}