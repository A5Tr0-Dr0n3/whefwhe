using System;
using UnityEngine;

[Serializable]
public class DialogueSegment
{
	public float maxTimeToRestart = 1f;

	public float minTimeForceCompletion = 2f;

	public Interruptability mInterruptability = Interruptability.unlimitedInterruptions;

	public float startTime;

	[HideInInspector]
	private float resetBound;

	[HideInInspector]
	private float continueBound;

	public float endTime;

	public float getResetBound()
	{
		resetBound = startTime + maxTimeToRestart;
		return resetBound;
	}

	public float getContinueBound()
	{
		continueBound = endTime - minTimeForceCompletion;
		return continueBound;
	}
}
