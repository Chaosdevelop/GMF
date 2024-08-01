using System;
using System.Linq;
using UnityEngine;

namespace GMF.Tags
{
	[System.Serializable]
	public sealed class TaggedContainerController
	{
		[field: SerializeField]
		public TagsIdCollection GeneralTags { get; private set; }

		public void Update()
		{

			Debug.Log($"Update {string.Join(" ", GeneralTags.GetAsTags().Select(arg => arg.ToString()))}");
		}
	}

	public interface ITaggedContainer
	{
		public TaggedContainerController TaggedContainer { get; }

	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class DisableGeneralTagsAttribute : Attribute
	{

		public DisableGeneralTagsAttribute()
		{

		}
	}
}