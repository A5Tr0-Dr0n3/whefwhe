using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TButt;

internal static class TBEnumFlags
{
	public static T ToMask<T>(this IEnumerable<T> values) where T : struct, IConvertible
	{
		if (!typeof(T).IsEnum)
		{
			throw new ArgumentException("T must be an enumerated type.");
		}
		int num = 0;
		foreach (T value in Enum.GetValues(typeof(T)))
		{
			if (values.Contains(value))
			{
				num |= Convert.ToInt32(value);
			}
		}
		return (T)Enum.Parse(typeof(T), num.ToString());
	}

	public static IEnumerable<T> ToValues<T>(this T flags) where T : struct, IConvertible
	{
		if (!typeof(T).IsEnum)
		{
			throw new ArgumentException("T must be an enumerated type.");
		}
		int inputInt = (int)(object)flags;
		IEnumerator enumerator = Enum.GetValues(typeof(T)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				T value = (T)enumerator.Current;
				int valueInt = (int)(object)value;
				if ((valueInt & inputInt) != 0)
				{
					yield return value;
				}
			}
		}
		finally
		{
			IDisposable disposable2;
			IDisposable disposable = (disposable2 = enumerator as IDisposable);
			if (disposable2 != null)
			{
				disposable.Dispose();
			}
		}
	}
}
