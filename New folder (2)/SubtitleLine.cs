using System;

[Serializable]
public struct SubtitleLine(float time, string sub)
{
	public float timeKey = time;

	public string text = sub;
}
