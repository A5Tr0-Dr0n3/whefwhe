using System;
using UnityEngine;

public class EnumArrayAttribute : PropertyAttribute
{
	public Type Type;

	public EnumArrayAttribute(Type type)
	{
		Type = type;
	}
}
