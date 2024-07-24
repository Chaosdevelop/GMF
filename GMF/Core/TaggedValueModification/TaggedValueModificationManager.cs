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

        static List<ITaggedValue> values = new List<ITaggedValue>();

        static List<ITaggedModifier> modifiers = new List<ITaggedModifier>();

        public static void AddModifier(ITaggedModifier modifier)
        {
            modifiers.Add(modifier);
            foreach (var taggedValue in values)
            {
                var mods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags));
                if (mods.Any())
                {
                    taggedValue.ApplyModifiers(mods.ToList());
                }

            }
        }

        public static void RemoveModifier(ITaggedModifier modifier)
        {
            modifiers.Remove(modifier);
            foreach (var taggedValue in values)
            {
                var mods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags));
                if (mods.Any())
                {
                    taggedValue.ApplyModifiers(mods.ToList());
                }

            }
        }

        public static void AddValue(ITaggedValue taggedValue)
        {
            values.Add(taggedValue);
            var mods = modifiers.Where(arg => TagManager.IsSubsetOf(taggedValue.Tags, arg.Tags));

            taggedValue.ApplyModifiers(mods.ToList());

        }

        public static void RemoveValue(ITaggedValue taggedValue)
        {
            values.Remove(taggedValue);
        }

    }
}