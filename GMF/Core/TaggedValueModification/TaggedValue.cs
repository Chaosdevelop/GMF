using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMF.Tags
{

    public interface ITaggedValue
    {
        TagsIdCollection Tags { get; }
        void ApplyModifiers(List<ITaggedModifier> modifiers);
        void Register();
        void Unregister();

    }

    [System.Serializable]
    public abstract class TaggedValue<T> : ITaggedValue where T : unmanaged
    {
        [field: SerializeField]
        public T DefaultValue { get; private set; }

        [field: SerializeField]
        public TagsIdCollection Tags { get; private set; }

        public T CurrentValue { get; protected set; }

        public abstract void ApplyModifiers(List<ITaggedModifier> modifiers);
        public void Register()
        {
            TaggedValueModificationManager.AddValue(this);
        }


        public void Unregister()
        {
            TaggedValueModificationManager.RemoveValue(this);
        }

        public static implicit operator T(TaggedValue<T> taggedValue)
        {
            return taggedValue?.CurrentValue ?? default;
        }

    }

    [System.Serializable]
    public class TaggedIntValue : TaggedValue<int>
    {
        public override void ApplyModifiers(List<ITaggedModifier> modifiers)
        {
            //Debug.Log(string.Join(" ", modifiers.Select(arg => arg.ToString())));
            float floatValue = DefaultValue;
            foreach (var modifier in modifiers.OrderBy(m => m.Order))
            {

                switch (modifier.Category)
                {
                    case TagStackableCategory.BaseOverride:
                        floatValue = modifier.Modifier;
                        break;
                    case TagStackableCategory.BaseAdd:
                        floatValue += modifier.Modifier;
                        break;
                    case TagStackableCategory.Increase:
                        floatValue += DefaultValue * modifier.Modifier / 100;
                        break;
                    case TagStackableCategory.Multiply:
                        floatValue *= modifier.Modifier;
                        break;
                    case TagStackableCategory.ExtraAdd:
                        floatValue += modifier.Modifier;
                        break;
                    case TagStackableCategory.ExtraOverride:
                        floatValue = modifier.Modifier;
                        break;

                }

            }
            CurrentValue = (int) floatValue;

        }


    }

    [System.Serializable]
    public class TaggedFloatValue : TaggedValue<float>
    {
        public override void ApplyModifiers(List<ITaggedModifier> modifiers)
        {
            float floatValue = DefaultValue;
            foreach (var modifier in modifiers.OrderBy(m => m.Order))
            {

                switch (modifier.Category)
                {
                    case TagStackableCategory.BaseOverride:
                        floatValue = modifier.Modifier;
                        break;
                    case TagStackableCategory.BaseAdd:
                        floatValue += modifier.Modifier;
                        break;
                    case TagStackableCategory.Increase:
                        floatValue += DefaultValue * modifier.Modifier / 100;
                        break;
                    case TagStackableCategory.Multiply:
                        floatValue *= modifier.Modifier;
                        break;
                    case TagStackableCategory.ExtraAdd:
                        floatValue += modifier.Modifier;
                        break;
                    case TagStackableCategory.ExtraOverride:
                        floatValue = modifier.Modifier;
                        break;

                }

            }
            CurrentValue = floatValue;
        }
    }
}