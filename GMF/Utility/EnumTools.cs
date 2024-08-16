using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GMF.Utility
{


	public enum EnumFilterUsage
	{
		None = 0,
		MustContainsAll = 1,
		MustHaveAny = 2,
		ExceptAllSelected = 3,
		ExceptAnySelected = 4,

	}

	public static class EnumTools
	{

		/*		static readonly Dictionary<Type, bool> EnumsContainsFlags;
				static EnumTools()
				{

					var all = Reflection.GetAllClasses();
					EnumsContainsFlags = new Dictionary<Type, bool>();
					foreach (var kvp in all)
					{
						var type = kvp.Value;
						if (type.IsEnum)
						{
							EnumsContainsFlags.Add(type, type.GetCustomAttribute(typeof(FlagsAttribute)) != null);
						}
					}
				}
		*/
		public static TOutgoing Convert<TOutgoing>(this object obj) where TOutgoing : class
		{
			Type incomingType = obj.GetType();

			MethodInfo conversionOperator = null;
			foreach (var method in incomingType.GetMethods(BindingFlags.Static | BindingFlags.Public))
			{
				if (
					method.Name == "op_Explicit" && //explicit converter
					method.ReturnType == typeof(TOutgoing) && //returns your outgoing ("Point") type
					method.GetParameters().Length == 1 && //only has 1 input parameter
					method.GetParameters()[0].ParameterType == incomingType //parameter type matches your incoming ("float2D") type
				)
				{
					conversionOperator = method;
					break;
				}
			}

			if (conversionOperator != null)
				return (TOutgoing)conversionOperator.Invoke(null, new object[] { obj });

			return obj as TOutgoing;

		}

		public static bool EnumFilteredCorrect<T>(T flags, T filter, EnumFilterUsage option)
		{

			switch (option)
			{
				case EnumFilterUsage.MustContainsAll:
					return EnumContainsAll(flags, filter);
				case EnumFilterUsage.MustHaveAny:
					return EnumContainsAny(flags, filter);
				case EnumFilterUsage.ExceptAnySelected:
					return !EnumContainsAny(flags, filter);
				case EnumFilterUsage.ExceptAllSelected:
					return !EnumContainsAll(flags, filter);
				default:
					return true;
			}
		}

		public static bool EnumContainsAll<T>(T flags, T flag)
		{
			var arr = Enum.GetValues(typeof(T));
			bool contains = true;
			foreach (var a in arr)
			{
				if (Checked(flag, (T)a))
				{
					if (!Checked(flags, (T)a))
					{
						contains = false;
					}
				}

			}
			return contains;
		}

		public static bool EnumContainsAny<T>(T flags, T flag)
		{
			var arr = Enum.GetValues(typeof(T));
			bool contains = false;
			foreach (var a in arr)
			{
				if (Checked(flag, (T)a))
				{
					if (Checked(flags, (T)a))
					{
						contains = true;
						break;
					}
				}

			}
			return contains;
		}

		/*		public static bool EnumContainsAny(System.Enum flags, System.Enum flag, System.Type type)
				{
					var arr = Enum.GetValues(type);
					bool withFlags = EnumsContainsFlags[type];
					bool contains = false;

					foreach (var a in arr)
					{
						var f = (System.Enum)a;
						if (withFlags)
						{
							if (flag.HasFlag(f) && flags.HasFlag(f))
							{
								contains = true;
								break;
							}
						}
						else
						{
							if (System.Convert.ToInt32(flags) == System.Convert.ToInt32(f) && System.Convert.ToInt32(flag) == System.Convert.ToInt32(f))
							{
								contains = true;
								break;
							}
						}


					}
					return contains;
				}*/


		public static bool Checked<T>(T flags, T flag)
		{
			return (System.Convert.ToInt32(flags) & System.Convert.ToInt32(flag)) != 0;
		}

		public static bool Checked<T>(this System.Enum flags, T flag)
		{
			return (System.Convert.ToInt32(flags) & System.Convert.ToInt32(flag)) != 0;
		}
		//
		public static T Check<T>(T flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (System.Convert.ToInt32(flags) | System.Convert.ToInt32(flag)));
		}

		public static T Check<T>(this System.Enum flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (System.Convert.ToInt32(flags) | System.Convert.ToInt32(flag)));
		}

		public static T UnCheck<T>(T flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (System.Convert.ToInt32(flags) & ~System.Convert.ToInt32(flag)));
		}

		public static T UnCheck<T>(this System.Enum flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (System.Convert.ToInt32(flags) & ~System.Convert.ToInt32(flag)));
		}

		public static T Toggle<T>(T flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (System.Convert.ToInt32(flags) ^ System.Convert.ToInt32(flag)));
		}

		public static T Toggle<T>(this System.Enum flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (System.Convert.ToInt32(flags) ^ System.Convert.ToInt32(flag)));
		}

		public static IEnumerable<T> GetAllValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static IEnumerable<T> GetValues<T>(this System.Enum enum1)
		{
			return Enum.GetValues(typeof(T)).Cast<T>().Where(arg => EnumTools.Checked(enum1, arg));
		}

		public static T2 ConvertEnum<T2>(this System.Enum enum1)
		{
			T2 converted = (T2)System.Enum.ToObject(typeof(T2), enum1);
			return converted;
		}

		public static T Random<T>() where T : struct
		{
			System.Array list = System.Enum.GetValues(typeof(T));
			return (T)list.GetValue(Randoms.Range(0, list.Length));
		}

		public static bool MaskedEnumEqual(object value, object masked)
		{
			int val1 = 1 << (int)value - 1;
			int val2 = (int)masked;
			return ((val2 & val1) == val1);
		}

		public static T RandomMask<T>() where T : struct
		{
			System.Array list = System.Enum.GetValues(typeof(T));
			T value = new T();

			for (int i = 0; i < list.Length; i++)
			{
				int rnd = Randoms.Range(0, 2);
				if (rnd == 1)
				{
					value = EnumTools.Check<T>(value, (T)list.GetValue(i));
				}
			}

			return value;
		}

		public static T RandomFromMask<T>(T masked) where T : struct
		{
			System.Array list = System.Enum.GetValues(typeof(T));
			List<T> variants = new List<T>();
			for (int i = 0; i < list.Length; i++)
			{
				T val = (T)list.GetValue(i);
				if (EnumTools.Checked(masked, val))
				{
					variants.Add(val);
				}
			}

			T value = new T();
			if (variants.Count > 0)
			{
				value = variants[Randoms.Range(0, variants.Count)];
			}
			return value;
		}

		public static T ConvertByName<T, T2>(T2 toconvert) where T : System.Enum where T2 : System.Enum
		{
			var name = System.Enum.GetName(typeof(T2), toconvert);
			if (string.IsNullOrEmpty(name))
			{
				Debug.LogError($"No name for {typeof(T2)}.{toconvert} in type {typeof(T)}");
				return default(T);
			}
			var result = System.Enum.Parse(typeof(T), name);
			return (T)result;
		}
		public static T ConvertByValue<T, T2>(T2 toconvert) where T : System.Enum where T2 : System.Enum
		{
			//int result = System.Convert.ToInt32(toconvert);
			return (T)System.Enum.ToObject(typeof(T), toconvert);
			// (T)result;
		}

		public static string GetFullName(this Enum myEnum)
		{
			return string.Format("{0}.{1}", myEnum.GetType().FullName, myEnum.ToString());
		}
	}
}
