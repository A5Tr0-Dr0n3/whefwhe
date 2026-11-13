using System.Collections.Generic;
using UnityEngine;

public class KitchenToastManager : MonoBehaviour
{
	private static KitchenToastManager instance;

	private List<ObjectToast> butterToast;

	private List<ObjectToast> kitchenToast;

	private static KitchenToastManager GetInstance()
	{
		if (instance != null)
		{
			return instance;
		}
		SafeDebug.LogWarning("KitchenToastManager manager requested, but no instance exists!");
		return null;
	}

	private void ensureOneInstance()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(gameObject);
		}
	}

	private void Awake()
	{
		ensureOneInstance();
	}

	private void Start()
	{
		butterToast = new List<ObjectToast>();
		kitchenToast = new List<ObjectToast>();
	}

	public static void AddKitchenToast(ObjectToast newKitchenToast)
	{
		instance.kitchenToast.Add(newKitchenToast);
	}

	public static void AddButterToast(ObjectToast newButterToast)
	{
		if (!(GetInstance() != null))
		{
			return;
		}
		for (int i = 0; i < instance.kitchenToast.Count; i++)
		{
			if (instance.kitchenToast[i] == newButterToast)
			{
				instance.kitchenToast.RemoveAt(i);
				i--;
			}
		}
		instance.butterToast.Add(newButterToast);
	}

	public static void RemoveToast(ObjectToast removedToast)
	{
		if (!(GetInstance() != null))
		{
			return;
		}
		for (int i = 0; i < instance.kitchenToast.Count; i++)
		{
			if (instance.kitchenToast[i] == removedToast)
			{
				instance.kitchenToast.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < instance.butterToast.Count; j++)
		{
			if (instance.butterToast[j] == removedToast)
			{
				instance.butterToast.RemoveAt(j);
				j--;
			}
		}
	}

	public static List<ObjectToast> GetButterToast()
	{
		if (GetInstance() != null)
		{
			return instance.butterToast;
		}
		return null;
	}

	public static List<ObjectToast> GetKitchenToast()
	{
		if (GetInstance() != null)
		{
			return instance.kitchenToast;
		}
		return null;
	}

	private void checkForMissingToast()
	{
		for (int i = 0; i < instance.kitchenToast.Count; i++)
		{
			if (instance.kitchenToast[i] == null)
			{
				instance.kitchenToast.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < instance.butterToast.Count; j++)
		{
			if (instance.butterToast[j] == null)
			{
				instance.butterToast.RemoveAt(j);
				j--;
			}
		}
	}

	private void Update()
	{
		checkForMissingToast();
	}
}
