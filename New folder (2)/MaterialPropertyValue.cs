using System;
using UnityEngine;

[Serializable]
public class MaterialPropertyValue
{
	public enum PropertyType
	{
		Vector,
		Float,
		Texture,
		Keyword,
		Color
	}

	public PropertyType type;

	public string propID;

	public string keyword;

	public Vector4 propVector;

	public float propFloat;

	public Texture2D propTexture;

	public bool attributeBool;

	public Color color;
}
