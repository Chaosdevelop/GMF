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

		//static HashSet<ITaggedValue> values = new HashSet<ITaggedValue>();

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

			/*			foreach (var taggedValue in values)
						{
							var mods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags));
							if (mods.Any())
							{
								taggedValue.ApplyModifiers(mods.ToList());
							}

						}*/
		}

		public static void AddModifiers(IEnumerable<ITaggedModifier> newmodifiers)
		{
			foreach (var mod in newmodifiers)
			{
				AddModifier(mod);
			}

			/*			foreach (var mod in newmodifiers)
						{
							modifiers.Add(mod);
						}
						foreach (var taggedValue in values)
						{
							var mods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags));
							if (mods.Any())
							{
								taggedValue.ApplyModifiers(mods.ToList());
							}

						}*/
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


			/*			foreach (var taggedValue in values)
						{
							*//*				var mods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags));
											if (mods.Any())
											{
												taggedValue.ApplyModifiers(mods.ToList());
											}*//*
							var correct = TagManager.IsSubsetOf(taggedValue.Tags, modifier.Tags);
							if (correct)
							{
								taggedValue.ApplyModifiers(mods.ToList());
							}
						}*/
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



			/*			values.Add(taggedValue);
						var mods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags));

						taggedValue.ApplyModifiers(mods.ToList());*/

		}

		public static void RemoveValue(ITaggedValue taggedValue)
		{
			valuesMods.Remove(taggedValue);
			/*			values.Remove(taggedValue);*/
		}

	}
}