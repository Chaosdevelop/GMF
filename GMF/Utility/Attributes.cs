using System;
using UnityEngine;

namespace GMF.Utility
{
	/// <summary>
	/// Attribute to group enums.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = false)]
	public class EnumGroupAttribute : Attribute
	{
		public string[] GroupNames { get; }

		public EnumGroupAttribute(params string[] groupNames)
		{
			GroupNames = groupNames;
		}
	}

	/// <summary>
	/// Attribute to exclude certain enum groups.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = false)]
	public class ExceptEnumGroupsAttribute : Attribute
	{
		public string[] GroupNames { get; }

		public ExceptEnumGroupsAttribute(params string[] groupNames)
		{
			GroupNames = groupNames;
		}
	}

	/// <summary>
	/// Attribute to include only certain enum groups.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = false)]
	public class OnlyEnumGroupsAttribute : Attribute
	{
		public string[] GroupNames { get; }

		public OnlyEnumGroupsAttribute(params string[] groupNames)
		{
			GroupNames = groupNames;
		}
	}

	/// <summary>
	/// Extension methods for enum groups.
	/// </summary>
	public static class EnumGroupsExtension
	{
		/// <summary>
		/// Gets a specified attribute of an enum value.
		/// </summary>
		/// <typeparam name="T">Type of the attribute to get.</typeparam>
		/// <param name="enumVal">The enum value.</param>
		/// <returns>The attribute of type T, if found; otherwise, null.</returns>
		public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
		{
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
			return (attributes.Length > 0) ? (T)attributes[0] : null;
		}
	}

	/// <summary>
	/// Attribute to mark a property as read-only in the inspector.
	/// </summary>
	public class ReadOnlyAttribute : PropertyAttribute
	{
	}

	/// <summary>
	/// Attribute to allow reference type selection in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class TypeSelectorAttribute : PropertyAttribute
	{
	}

	/// <summary>
	/// Attribute to add a description to a type selection.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class SelectTypeDescriptionAttribute : Attribute
	{
		public string TypeDescription { get; }

		public SelectTypeDescriptionAttribute(string typeDescription)
		{
			TypeDescription = typeDescription;
		}
	}

	/// <summary>
	/// Attribute to mark a class as deprecated.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DeprecatedTypeAttribute : Attribute
	{
	}
}
