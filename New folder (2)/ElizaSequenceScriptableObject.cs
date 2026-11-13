using UnityEngine;

[CreateAssetMenu(fileName = "New Archive Eliza Sequence Content", menuName = "TenderClaws/Create Archive Eliza Sequence Content", order = 1)]
public class ElizaSequenceScriptableObject : ScriptableObject
{
	public ElizaDialogue dialogue;

	public ElizaEyeTimings eyeTimings;

	[Header("Eliza Dialogue Timing:")]
	public float elizaGreetingWaitTime = 2f;

	public float walkToSunTime = 3f;

	public float elizaHoverCycleTime = 1.4f;

	public float elizaHoverDistance = 0.4f;

	[Header("Eliza On Levels:")]
	public float bootSunsetPercentage = 0.9f;

	public float bootCavePercentage = 0.9f;

	public float bootCanyonPercentage = 0.9f;

	public float bootSpacePercentage = 0.9f;

	public float bootVolcanoPercentage = 0.9f;

	public float turnOnTime = 2f;

	public float turnOffTime = 2f;

	public float turnPadding = 2f;

	[Header("Commercial Music Controls:")]
	public float brokenMusicPitch = 0.2f;

	public float pitchChangeRate = 0.1f;

	public float brokenMusicVolume = 0.2f;

	public float volumeChangeRate = 0.1f;

	[Header("Environment On Timings:")]
	public AnimationCurve pitchUpCurve;

	public AnimationCurve pitchDownCurve;

	public float environmentFillInRate = 1f;

	public float canyonSmallBalloonOnTime = 0.4f;

	public float canyonBigBalloonOnTime = 1f;

	public float vesuviusSmokeOnTime = 1f;

	public float vesuviusLetterTime = 1.4f;

	public float torchOnTime = 0.2f;

	public float eruptionForceMin;

	public float eruptionForceMax;

	public float eruptionRainTiming = 0.3f;

	[Header("Shell Sequence Timings:")]
	public float timeBetweenShellsOn = 0.4f;

	public int[] numShellsToAdvanceMonologue;

	public float encourageShellTime = 4f;

	[Header("Glitch variables")]
	public float introSceneFlashInterval = 0.15f;

	public float introInitialGlitchTime = 1f;

	public float sceneFilledInGlitchDuration = 0.7f;
}
