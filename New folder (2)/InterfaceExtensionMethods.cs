using System;
using System.Linq;
using UnityEngine;

public static class InterfaceExtensionMethods
{
	public static T[] GetInterfaces<T>(this GameObject gObj)
	{
		if (!typeof(T).IsInterface)
		{
			throw new SystemException("Specified type is not an interface!");
		}
		MonoBehaviour[] components = gObj.GetComponents<MonoBehaviour>();
		return (from a in components
			where a.GetType().GetInterfaces().Any((Type k) => k == typeof(T))
			select (T)(object)a).ToArray();
	}

	public static T GetInterface<T>(this GameObject gObj) where T : class
	{
		if (!typeof(T).IsInterface)
		{
			throw new SystemException("Specified type is not an interface!");
		}
		return gObj.GetComponent(typeof(T)) as T;
	}

	public static T[] GetInterfaces<T>(this Component gObj)
	{
		if (!typeof(T).IsInterface)
		{
			throw new SystemException("Specified type is not an interface!");
		}
		MonoBehaviour[] components = gObj.GetComponents<MonoBehaviour>();
		return (from a in components
			where a.GetType().GetInterfaces().Any((Type k) => k == typeof(T))
			select (T)(object)a).ToArray();
	}

	public static T GetInterface<T>(this Component gObj) where T : class
	{
		if (!typeof(T).IsInterface)
		{
			throw new SystemException("Specified type is not an interface!");
		}
		return gObj.GetComponent(typeof(T)) as T;
	}

	public static T GetInterfaceInChildren<T>(this GameObject gObj)
	{
		if (!typeof(T).IsInterface)
		{
			throw new SystemException("Specified type is not an interface!");
		}
		return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
	}

	public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
	{
		if (!typeof(T).IsInterface)
		{
			throw new SystemException("Specified type is not an interface!");
		}
		MonoBehaviour[] componentsInChildren = gObj.GetComponentsInChildren<MonoBehaviour>();
		return (from a in componentsInChildren
			where a.GetType().GetInterfaces().Any((Type k) => k == typeof(T))
			select (T)(object)a).ToArray();
	}
}
