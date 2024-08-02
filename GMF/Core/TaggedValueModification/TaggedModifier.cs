using System;
using UnityEngine;

namespace GMF.Tags
{
	public enum TagStackableCategory
	{
		BaseOverride = 0,
		BaseAdd = 10,
		Increase = 20,
		Multiply = 30,
		ExtraAdd = 40,
		ExtraOverride = 50
	}

	[Serializable]
	public class TaggedModifier : ITaggedModifier
	{
		[SerializeField]
		TagsIdCollection tags;

		[SerializeField]
		float modifier;

		[SerializeField]
		int order;

		[SerializeField]
		TagStackableCategory category = TagStackableCategory.BaseAdd;

		public TaggedModifier(TagsIdCollection tags, float modifier, int order, TagStackableCategory category)
		{
			this.tags = tags ?? throw new ArgumentNullException(nameof(tags));
			this.modifier = modifier;
			this.order = order;
			this.category = category;
		}

		public int Order => order;
		public TagStackableCategory Category => category;
		public TagsIdCollection Tags => tags;
		public float Modifier => modifier;

		public TaggedModifier Copy()
		{
			return MemberwiseClone() as TaggedModifier;
		}

		public override string ToString()
		{
			return $"Tags: {tags} Modifier: {category} {modifier}";
		}
	}
}