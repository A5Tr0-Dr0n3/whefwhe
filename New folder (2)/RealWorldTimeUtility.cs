using System;
using UnityEngine;

public class RealWorldTimeUtility : MonoBehaviour
{
	public static float NormalizeTime(DateTime date)
	{
		return ((float)date.Hour + (float)date.Minute / 60f + 1f / 60f * ((float)date.Second / 60f)) / 24f;
	}

	public static DateTime NormalizedTimeToDateTime(float t)
	{
		int hour = Mathf.FloorToInt(t * 24f % 24f);
		int minute = (int)(60f * (t * 24f - (float)Mathf.FloorToInt(t * 24f))) % 60;
		return new DateTime(2017, 1, 1, hour, minute, 1);
	}
}
