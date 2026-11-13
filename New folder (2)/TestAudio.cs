using System;
using UnityEngine;

[Serializable]
public class TestAudio
{
	public string label;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 1f)]
	public float spatial = 1f;

	public bool delaySwitch;

	public float delay;
}
