using System;
using System.Collections.Generic;

namespace GMF.Tags
{
    public interface ITag : IEquatable<ITag>
    {
        uint Id { get; }
        string Name { get; set; }
        void AddSubTag(ITag subTag);
        IEnumerable<ITag> GetSubTags();
        bool ContainsTag(ITag tag);
        IEnumerable<ITag> GetAllTags();
    }

    public interface ITagID : IEquatable<ITagID>
    {
    }

    public interface ITagsIdCollection
    {
        public Int32 Count { get; }
        public IReadOnlyCollection<ITag> GetAsTags();
    }
}