using System;
using UnityEngine;

[Serializable]
public class MacyLine
{
	public LineRenderer line;

	public Transform groundAnchor;

	public Transform floatAnchor;

	private float startLen;

	private float currLen;

	private float midPoint = 0.5f;

	private void Start()
	{
		midPoint = UnityEngine.Random.Range(0.3f, 0.7f);
	}

	public void SetStartLength(float length)
	{
		if (startLen == 0f)
		{
			startLen = length;
		}
	}

	public float GetStartLength()
	{
		return startLen;
	}

	public void SetCurrLength(float length)
	{
		currLen = length;
	}

	public float GetCurrLength()
	{
		return currLen;
	}

	public float GetMidPoint()
	{
		return midPoint;
	}
}
