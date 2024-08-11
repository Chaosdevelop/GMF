using System.Collections.Generic;
using System.Linq;


namespace GMF.Tags
{
	public interface ITaggedContainerRegistrator
	{
		void Register();
		void Unregister();
	}


	public static class TaggedValueModificationManager
	{

		static HashSet<ITaggedModifier> modifiers = new HashSet<ITaggedModifier>();

		static Dictionary<ITaggedValue, HashSet<ITaggedModifier>> valuesMods = new Dictionary<ITaggedValue, HashSet<ITaggedModifier>>();


		public static void AddModifier(ITaggedModifier modifier)
		{
			var success = modifiers.Add(modifier);
			if (!success) return;

			foreach (var kvp in valuesMods)
			{
				var taggedValue = kvp.Key;
				var correct = TagManager.IsSubsetOf(taggedValue.Tags, modifier.Tags);
				if (correct)
				{
					kvp.Value.Add(modifier);
					taggedValue.ApplyModifiers(kvp.Value);
				}


			}

		}

		public static void AddModifiers(IEnumerable<ITaggedModifier> newmodifiers)
		{
			foreach (var mod in newmodifiers)
			{
				AddModifier(mod);
			}
		}

		public static void RemoveModifier(ITaggedModifier modifier)
		{
			modifiers.Remove(modifier);
			foreach (var kvp in valuesMods)
			{
				var taggedValue = kvp.Key;
				var correct = TagManager.IsSubsetOf(taggedValue.Tags, modifier.Tags);
				if (correct)
				{
					kvp.Value.Remove(modifier);
					taggedValue.ApplyModifiers(kvp.Value);
				}
			}

		}

		public static void RemoveModifiers(IEnumerable<ITaggedModifier> newmodifiers)
		{
			foreach (var mod in newmodifiers)
			{
				RemoveModifier(mod);
			}

		}



		public static void AddValue(ITaggedValue taggedValue)
		{
			if (!valuesMods.ContainsKey(taggedValue))
			{
				var valuemods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags)).ToHashSet();
				valuesMods.Add(taggedValue, valuemods);
				taggedValue.ApplyModifiers(valuemods);
			}

		}

		public static void RemoveValue(ITaggedValue taggedValue)
		{
			valuesMods.Remove(taggedValue);
		}

	}
}