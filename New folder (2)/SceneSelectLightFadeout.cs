using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class SceneSelectLightFadeout : MonoBehaviour
{
	[SerializeField]
	private VVR_Point fadeoutLight;

	[SerializeField]
	private float lightLerpTime = 3f;

	private float lightStartIntensity;

	private SafeCoroutine lightFadeThread;

	private void Start()
	{
		lightStartIntensity = fadeoutLight.intensity;
	}

	public SafeCoroutine StartLightFadeSequence()
	{
		lightFadeThread = GlobalCoroutineContainer.AddThread(lightFadeSequence(), this);
		return lightFadeThread;
	}

	public void CancelLightFadeSequence()
	{
		if (lightFadeThread.IsRunning)
		{
			lightFadeThread.Stop();
		}
		fadeoutLight.intensity = lightStartIntensity;
	}

	private IEnumerator lightFadeSequence()
	{
		float timer = 0f;
		while (timer < lightLerpTime)
		{
			timer += Time.deltaTime;
			float t = timer / lightLerpTime;
			fadeoutLight.intensity = Mathf.Lerp(lightStartIntensity, 0f, t);
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
	}
}
