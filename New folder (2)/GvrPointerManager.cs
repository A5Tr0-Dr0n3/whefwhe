using System;
using UnityEngine;

[Obsolete("Replaced by GvrPointerInputModule.")]
[AddComponentMenu("")]
public class GvrPointerManager : MonoBehaviour
{
	public static GvrBasePointer Pointer
	{
		get
		{
			return GvrPointerInputModule.Pointer;
		}
		set
		{
			GvrPointerInputModule.Pointer = value;
		}
	}

	public static void OnPointerCreated(GvrBasePointer createdPointer)
	{
		GvrPointerInputModule.OnPointerCreated(createdPointer);
	}
}
