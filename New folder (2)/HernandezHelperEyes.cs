using System.Collections;
using UnityEngine;

public class HernandezHelperEyes : MonoBehaviour
{
	[SerializeField]
	private AudioSourceLoudness mLoudness;

	private ElizaEyeStates mEyes;

	private char defaultEye = '0';

	private char talkingEye = '@';

	private char blinkEye = '_';

	private string overrideEyeL = string.Empty;

	private string overrideEyeR = string.Empty;

	public bool useOverrideEye;

	private bool blinkEnabled = true;

	[SerializeField]
	private float blinkIntervalMin = 0.8f;

	[SerializeField]
	private float blinkIntervalMax = 3f;

	[SerializeField]
	private float doubleBlinkChance = 0.1f;

	private float blinkTimer;

	private float blinkLength = 0.2f;

	private void Start()
	{
		mEyes = GetComponent<ElizaEyeStates>();
		mEyes.StartEyesOn();
		GlobalCoroutineContainer.AddThread(blinkSequence(), this);
	}

	private IEnumerator blinkSequence()
	{
		while (true)
		{
			mEyes.SetEyes(string.Empty + getOpenEyeChar(left: true), getOpenEyeChar(left: false));
			if (blinkEnabled)
			{
				blinkTimer -= Time.deltaTime;
				if (blinkTimer <= 0f)
				{
					mEyes.SetEyes(string.Empty + blinkEye, string.Empty);
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(blinkLength, this);
					blinkTimer = Random.Range(blinkIntervalMin, blinkIntervalMax);
					if (Random.Range(0f, 1f) < doubleBlinkChance)
					{
						blinkTimer /= 10f;
					}
				}
			}
			yield return null;
		}
	}

	private string getOpenEyeChar(bool left)
	{
		if (useOverrideEye)
		{
			return (!left) ? overrideEyeR : overrideEyeL;
		}
		return string.Empty + ((!(mLoudness.lerpLoudness > 0f)) ? defaultEye : talkingEye);
	}

	public void EnableBlink(bool enable)
	{
		blinkEnabled = enable;
	}

	public void ForceBlink()
	{
		blinkTimer = 0f;
	}

	public void SetOverrideEye(string overrideLIn, string overrideRIn)
	{
		useOverrideEye = true;
		overrideEyeL = overrideLIn;
		overrideEyeR = overrideRIn;
	}

	public void ReleaseOverrideEye()
	{
		useOverrideEye = false;
	}
}
