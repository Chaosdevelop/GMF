using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMF.Tags
{
    [Serializable]
    public class Tag : ITag
    {
        [field: SerializeField]
        [field: HideInInspector]
        public uint Id { get; private set; }
        
        [field: SerializeField]
        public string Name { get; set; }
        
        //[SerializeField]
        //Tag[] subTags;
        [field: SerializeField]
        public TagsIdCollection SubTagIds { get; private set; } = new TagsIdCollection();

        public Tag(uint id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Tag name cannot be null or empty.", nameof(name));
            }
            
            Id = id;
            Name = name;
        }

        public void AddSubTag(ITag subTag)
        {
            if (subTag == null)
            {
                throw new ArgumentNullException(nameof(subTag));
            }
            
            SubTagIds.Add(new TagId(subTag.Id));
        }

        public bool ContainsTag(ITag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }
            
            return Id == tag.Id || SubTagIds.Contains(tag);
        }

        public IEnumerable<ITag> GetSubTags()
        {
            return SubTagIds.GetAsTags();
        }

        public IEnumerable<ITag> GetAllTags()
        {
            var allTags = new List<ITag> { this };
            allTags.AddRange(SubTagIds.GetAsTags());

            return allTags;
        }

        public void RemoveSubTag(ITag subTag)
        {
            SubTagIds.Remove(new TagId(subTag.Id));
        }
        
        public bool Equals(ITag other)
        {
            return other is not null && Id == other.Id;
        }
        
        public override string ToString()
        {
            return $"{Name} (ID: {Id})";
        }

        public string ToDropdownString()
        {
            if (SubTagIds.IsEmpty)
            {
                return Name;
            }
            
            var subTagNames = string.Join(", ", SubTagIds.GetAsTags().Select(t => t.Name));
            return $"{Name} ({subTagNames})";
            
            /*			if (subTags != null && subTags.Length > 0)
                        {
                            var subTagNames = string.Join(", ", subTags.Select(t => t.Name));
                            return $"{Name} ({subTagNames})";
                        }
                        else
                        {
                            return Name;
                        }*/
        }

    }


}
