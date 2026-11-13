using System.Collections.Generic;

public static class TBArrayExtensions
{
	public static T[] AddToArray<T>(T[] array, T item)
	{
		T[] array2 = new T[array.Length + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i];
		}
		array2[array2.Length - 1] = item;
		return array2;
	}

	public static T[] RemoveFromArray<T>(T[] array, int index)
	{
		List<T> list = new List<T>(array);
		list.RemoveAt(index);
		return list.ToArray();
	}
}
