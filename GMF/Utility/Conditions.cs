using System.Collections.Generic;

namespace GMF.Utility
{
	public interface IMatchable
	{
		bool Match(object pointer);
	}
	public interface IMatchable<T>
	{
		bool Match(T pointer);
	}
	public struct EmptyContext
	{
		public static EmptyContext Empty;
	}

	public interface IConditional<Tcontext>
	{
		bool Satisfied(Tcontext context);
	}

	static public partial class Tools
	{
		static public bool CompareConditions<T>(this IEnumerable<IConditional<T>> collection, T context, ConditionsCompare method)
		{
			bool condition = true;
			if (method == ConditionsCompare.AllTrue)
			{
				condition = true;
				foreach (var item in collection)
				{
					bool value = item.Satisfied(context);
					if (!value)
					{
						return false;
					}
				}
			}
			else if (method == ConditionsCompare.AllFalse)
			{
				condition = true;
				foreach (var item in collection)
				{
					bool value = item.Satisfied(context);
					if (value)
					{
						return false;
					}
				}
			}
			else if (method == ConditionsCompare.OneTrue)
			{
				condition = false;
				foreach (var item in collection)
				{
					bool value = item.Satisfied(context);
					if (value)
					{
						return true;
					}
				}
			}
			else if (method == ConditionsCompare.OneFalse)
			{
				condition = false;
				foreach (var item in collection)
				{
					bool value = item.Satisfied(context);
					if (!value)
					{
						return true;
					}
				}
			}

			return condition;
		}
	}
}