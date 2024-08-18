using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GMF.Utility
{
	public enum ConditionsCompare
	{
		AllTrue,
		AllFalse,
		OneTrue,
		OneFalse
	}

	public enum BooleanOperators
	{
		And = 0,
		Or = 1
	}

	public enum NumberMathOperators
	{
		Add = 0,
		Sub = 1,
		Mul = 2,
		Div = 3,
		Mod = 4,
		Set = 5
	}

	public enum NumbersCompareOperators
	{
		Equal,
		NotEqual,
		Great,
		GreatOrEqual,
		Less,
		LessOrEqual
	}

	public enum DateTimeParts
	{
		Second,
		Minute,
		Hour,
		Day,
		Week,
		Month,
		Year,
		DayOfYear,
		DayOfWeek
	}


	[Serializable]
	public struct Bunch<T1, T2>
	{
		public T1 item1;
		public T2 item2;

		public Bunch(T1 item1, T2 item2)
		{
			this.item1 = item1;
			this.item2 = item2;
		}
	}
	[Serializable]
	public struct Bunch<T1, T2, T3>
	{
		public T1 item1;
		public T2 item2;
		public T3 item3;

		public Bunch(T1 item1, T2 item2, T3 item3)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
		}
	}

	[Serializable]
	public struct MinMax<T>
	{
		[SerializeField]
		private T min;
		[SerializeField]
		private T max;

		public T Min { get => min; set => min = value; }
		public T Max { get => max; set => max = value; }

		public MinMax(T min, T max)
		{
			this.min = min;
			this.max = max;
		}
	}

	static public partial class Tools
	{
		public static DateTime TimeStart = new DateTime(2016, 1, 1);
		public static bool IsBuildingBundles = false;
		public static Vector3[] RawDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };

		static public float TotalLenght(this IList<Vector3> points)
		{
			Vector3 vector = Vector3.zero;
			for (int i = 1; i < points.Count; i++)
			{
				vector += points[i] - points[i - 1];
			}
			return vector.magnitude;
		}

		static public float GetTime()
		{
			return (float)(DateTime.UtcNow - TimeStart).TotalSeconds;
		}

		static public GameObject Instantiate(this GameObject go)
		{
			return GameObject.Instantiate(go);
		}

		/// <summary>
		/// Set transform values to default.
		/// </summary>
		/// <param name="transform">Transform.</param>
		public static void Reset(this Transform transform)
		{
			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
		}

		/// <summary>
		/// Sets the parent with reset option.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="worldStays">If set to <c>true</c> world stays.</param>
		/// <param name="reset">If set to <c>true</c> reset.</param>
		public static void SetParent(this Transform transform, Transform parent, bool worldStays = false, bool reset = true)
		{
			transform.SetParent(parent, worldStays);
			if (reset) { transform.Reset(); }
		}

		public static void SetParentAndReset(this Transform transform, Transform parent)
		{
			transform.SetParent(parent, false);
			transform.Reset();
		}

		/// <summary>
		/// Instantiate the specified orignal with it name.
		/// </summary>
		/// <param name="orignal">Orignal.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Instantiate<T>(this T original) where T : UnityEngine.Object
		{
			var instance = UnityEngine.Object.Instantiate<T>(original);
			instance.name = original.name;
			return instance;
		}

		public static T Instantiate<T>(this T original, Transform parent) where T : MonoBehaviour
		{
			var instance = UnityEngine.Object.Instantiate<T>(original);
			instance.name = original.name;
			instance.transform.SetParent(parent, false, true);
			return instance;
		}

		public static void DestroyChildren(this Transform transform, bool immediate = true)
		{
			List<GameObject> children = new List<GameObject>();
			while (transform.childCount > 0)
			{
				var child = transform.GetChild(0);
				child.parent = null;
				children.Add(child.gameObject);
			}
			for (int i = 0; i < children.Count; i++)
			{
				if (immediate)
				{
					GameObject.DestroyImmediate(children[i]);
				}
				else
				{
					GameObject.Destroy(children[i]);
				}

			}
		}

		static public GameObject FindGO(this GameObject go, string child)
		{
			return go.transform.Find(child).gameObject;
		}

		static public void Swap<T>(ref T lhs, ref T rhs)
		{
			var temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		static public bool ToBool(this float num)
		{
			return num >= 1;
		}
		static public float ToFloat(this bool val)
		{
			return val ? 1 : 0;
		}


		static public Vector3 ToVector3(this Vector2 vector)
		{
			return new Vector3(vector.x, vector.y);
		}

		static public Vector2 ToVector2(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		static public int XInt(this Vector2 val)
		{
			return (int)val.x;
		}

		static public int YInt(this Vector2 val)
		{
			return (int)val.y;
		}

		static public int MathfRepeat(int i, int lenght)
		{
			return (int)Mathf.Repeat(i, lenght);
		}

		static public RectTransform TransformToRectTransform(this Transform t)
		{
			return t.gameObject.AddComponent<RectTransform>();
		}

		public static bool MaskedEnumEqual(object value, object masked)
		{
			int val1 = 1 << (int)value - 1;
			int val2 = (int)masked;
			return ((val2 & val1) == val1);
		}

		public static bool SafeEquals<T>(this object obj, T other)
		{
			return obj != null && ((T)obj).Equals(other);
		}

		public static bool EnumChecked<T>(T flags, T flag)
		{
			return (Convert.ToUInt32(flags) & Convert.ToUInt32(flag)) != 0;

		}

		public static T EnumCheck<T>(T flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (Convert.ToUInt32(flags) | Convert.ToUInt32(flag)));

		}

		public static T EnumUnCheck<T>(T flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (Convert.ToUInt32(flags) & ~Convert.ToUInt32(flag)));

		}

		public static T EnumToggle<T>(T flags, T flag)
		{
			return (T)System.Enum.ToObject(flags.GetType(), (Convert.ToUInt32(flags) ^ Convert.ToUInt32(flag)));

		}

		public static IEnumerable<T> GetValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}


		static public List<int> JsonListToIntList(ArrayList list)
		{
			List<int> listout = new List<int>();
			for (int i = 0; i < list.Count; i++)
			{
				listout.Add((int)(double)list[i]);
			}
			return listout;
		}
		static public List<float> JsonListToFloatList(ArrayList list)
		{
			List<float> listout = new List<float>();
			for (int i = 0; i < list.Count; i++)
			{
				listout.Add((float)(double)list[i]);
			}
			return listout;
		}

		static public List<string> JsonListToStringList(ArrayList list)
		{
			List<string> listout = new List<string>();
			for (int i = 0; i < list.Count; i++)
			{
				listout.Add((string)list[i]);
			}
			return listout;
		}

		static public List<T> JsonListToTypeList<T>(this ArrayList list)
		{
			List<T> listout = new List<T>();
			for (int i = 0; i < list.Count; i++)
			{
				listout.Add((T)list[i]);
			}
			return listout;
		}

		static public bool CompareAll(ConditionsCompare method, List<bool> list)
		{
			bool condition = true;
			if (method == ConditionsCompare.AllTrue)
			{
				condition = true;
				for (int i = 0; i < list.Count; i++)
				{
					bool value = list[i];
					if (!value)
					{
						return false;
					}
				}
			}
			else if (method == ConditionsCompare.AllFalse)
			{
				condition = true;
				for (int i = 0; i < list.Count; i++)
				{
					bool value = list[i];
					if (value)
					{
						return false;
					}
				}
			}
			else if (method == ConditionsCompare.OneTrue)
			{
				condition = false;
				for (int i = 0; i < list.Count; i++)
				{
					bool value = list[i];
					if (value)
					{
						return true;
					}
				}
			}
			else if (method == ConditionsCompare.OneFalse)
			{
				condition = false;
				for (int i = 0; i < list.Count; i++)
				{
					bool value = list[i];
					if (!value)
					{
						return true;
					}
				}
			}

			return condition;
		}

		public static bool CompareNumbers(NumbersCompareOperators op, float a, float b)
		{
			switch (op)
			{
				case NumbersCompareOperators.Equal:
					return a == b;
				case NumbersCompareOperators.NotEqual:
					return a != b;
				case NumbersCompareOperators.Great:
					return a > b;
				case NumbersCompareOperators.GreatOrEqual:
					return a >= b;
				case NumbersCompareOperators.Less:
					return a < b;
				case NumbersCompareOperators.LessOrEqual:
					return a <= b;
				default:
					goto case NumbersCompareOperators.Equal;
			}
		}

		public static float OperateNumbers(NumberMathOperators op, float a, float b)
		{
			switch (op)
			{
				case NumberMathOperators.Add:
					return a + b;
				case NumberMathOperators.Sub:
					return a - b;
				case NumberMathOperators.Mul:
					return a * b;
				case NumberMathOperators.Div:
					return a / b;
				case NumberMathOperators.Mod:
					return a % b;
				case NumberMathOperators.Set:
					return b;
				default:
					goto case NumberMathOperators.Add;
			}
		}


		public static void SafeLog(string str)
		{
			string datapath = Path.Combine(Application.persistentDataPath, "log.txt");
			string value = "";
			if (File.Exists(datapath))
			{
				value = File.ReadAllText(datapath);
			}
			value += str;
			value += "\n";
			File.WriteAllText(datapath, value);
		}

		public static float Sigmoid(float x, float alpha = 1)
		{
			return 1 / (1 + Mathf.Exp(-x * alpha));
		}

		public static float SigmoidDerivative(float x, float alpha = 1)
		{
			return Sigmoid(x, alpha) * (1 - Sigmoid(x, alpha));
		}

		public static float BipolarSigmoid(float x, float alpha = 1)
		{
			return 2 / (1 + Mathf.Exp(-x * alpha)) - 1;
		}


		public static bool IsNullOrDefault<T>(this T value)
		{
			if (value is System.ValueType)
			{
				return object.Equals(value, default(T));
			}
			else
			{
				return value == null;
			}
		}

		public static IEnumerator WaitFor(this YieldInstruction instruction)
		{
			yield return instruction;
		}

		public static IEnumerator WaitIEnumerator<T>(this T en, bool breakEnumerator = false) where T : IEnumerator
		{

			if (en != null)
			{
				while (en.MoveNext() && !breakEnumerator)
				{
					yield return en.Current;
				}
			}

		}
	}
}