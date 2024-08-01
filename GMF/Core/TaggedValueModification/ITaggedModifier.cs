namespace GMF.Tags
{
	public interface ITaggedModifier : ITagged
	{
		TagStackableCategory Category { get; }
		System.Single Modifier { get; }
		System.Int32 Order { get; }

	}
}