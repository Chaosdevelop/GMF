using System;
using UnityEngine;

namespace GMF.Tags
{
    [Serializable]
    public struct TagId : ITagID
    {
        public static bool operator ==(TagId left, TagId right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(TagId left, TagId right)
        {
            return !(left == right);
        }
        
        [SerializeField]
        [HideInInspector]
        uint id;

        public uint Id => id;
        
        public TagId(uint id)
        {
            this.id = id;
        }
        
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
        
        public override Boolean Equals(System.Object obj)
        {
            return obj is TagId tag && Equals(tag);
        }
        
        public bool Equals(TagId other)
        {
            return Id == other.Id;
        }

        public Boolean Equals(ITagID other)
        {
            return other is TagId tag && Equals(tag);
        }
    }
}