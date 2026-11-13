using UnityEngine;

public abstract class Backstage : SceneBehaviorManager
{
	public enum BackstageStates
	{
		Intro,
		JobIntrusion
	}

	public static Backstage instance;

	public BoidController mainSwarmController;

	public GameObject[] jobGlobes;

	[HideInInspector]
	public bool isLoadingFromJob;

	[HideInInspector]
	public int jobLoadIndex;

	public AudioClip transitionSound;
}
