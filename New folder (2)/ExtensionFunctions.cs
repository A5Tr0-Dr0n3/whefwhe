using System.IO;
using UnityEngine;

public static class ExtensionFunctions
{
	public static Vector3 Flatten(this Vector3 vec)
	{
		return new Vector3(vec.x, 0f, vec.z);
	}

	public static bool ComparePath(this string a, string b)
	{
		return Path.GetFullPath(a) == Path.GetFullPath(b);
	}
}
