namespace GMF.Tags
{
    public interface ITaggedModifier
    {
        TagStackableCategory Category { get; }
        System.Single Modifier { get; }
        System.Int32 Order { get; }
        TagsIdCollection Tags { get; }
    }
}