using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class GvrInfo : PropertyAttribute
{
	public string text;

	public int numLines;

	public GvrInfo(string text, int numLines)
	{
		this.text = text;
		this.numLines = numLines;
	}
}
