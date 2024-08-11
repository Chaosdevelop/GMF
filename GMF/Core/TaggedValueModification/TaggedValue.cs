using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMF.Tags
{
	public interface ITagged
	{
		TagsIdCollection Tags { get; }
		public void Add(ITag tag)
		{
			if (!Tags.Contains(tag))
			{
				Tags.Add(tag);
			}
		}

		public void Remove(ITag tag)
		{
			Tags.Remove(tag);
		}
	}
	public interface ITaggedValue : ITagged
	{

		void ApplyModifiers(HashSet<ITaggedModifier> modifiers);
		void Register();
		void Unregister();


	}

	[System.Serializable]
	public abstract class TaggedValue<T> : ITaggedValue where T : unmanaged
	{
		[field: SerializeField]
		public T DefaultValue { get; private set; }

		[field: SerializeField]
		public TagsIdCollection Tags { get; private set; } = new TagsIdCollection();

		public T CurrentValue {
			get {

				if (!registered)
				{
					Register();
				}
				return currentValue;
			}
			protected set {
				var prev = currentValue;
				currentValue = value;
				if (!prev.Equals(value))
				{
					changedEvent?.Invoke(currentValue);
				}
			}
		}

		[System.NonSerialized]
		Action<T> changedEvent;

		T currentValue;
		[System.NonSerialized]
		protected bool registered = false;

		~TaggedValue()
		{
			Unregister();
		}

		public abstract void ApplyModifiers(HashSet<ITaggedModifier> modifiers);
		public void Register()
		{
			registered = true;
			currentValue = DefaultValue;
			TaggedValueModificationManager.AddValue(this);
		}


		public void Unregister()
		{
			TaggedValueModificationManager.RemoveValue(this);
			registered = false;
		}
		public void Subscribe(Action<T> subscribe)
		{
			changedEvent += subscribe;
		}


		public static implicit operator T(TaggedValue<T> taggedValue)
		{
			return taggedValue?.CurrentValue ?? default;
		}

	}

	[System.Serializable]
	public class TaggedIntValue : TaggedValue<int>
	{
		public override void ApplyModifiers(HashSet<ITaggedModifier> modifiers)
		{
			//Debug.Log(string.Join(" ", modifiers.Select(arg => arg.ToString())));
			float floatValue = DefaultValue;
			foreach (var modifier in modifiers.OrderBy(m => m.Order))
			{
				//Need to fix order
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
			CurrentValue = (int)floatValue;

		}


	}

	[System.Serializable]
	public class TaggedFloatValue : TaggedValue<float>
	{
		public override void ApplyModifiers(HashSet<ITaggedModifier> modifiers)
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