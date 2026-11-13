using System;
using UnityEngine;

[Serializable]
public class ElizaEyeTimings
{
	[Header("Burrows of Mystery Timings")]
	public float aEyesNarrowStartTime;

	public float aEyesNarrowEndTime;

	[Header("Cosmic Journey Timings")]
	public float bOutOfThisWorldWinkTime;

	[Header("Plot Chunk One Timings")]
	public float cRelaxTimeStartTime;

	public float cExcitementStartTime;

	[Header("Plot Chunk Two Timings")]
	public float dMishapLookStartTime;

	public float dMishapLookEndTime;

	[Header("Plot Chunk Three Timings")]
	public float hDollaStartTime;

	[Header("Plot Chunk Four Timings")]
	public float eProcessingStartTime;

	public float eProcessingEndTime;

	[Header("On Monologue Timings")]
	public float fDrabOfficesTime;

	public float fProcessingStartTime;

	public float fThoughtfulStartTime;

	[Header("End Monologue Timings")]
	public float gThinkStartTime;

	public float gThinkEndTime;
}
