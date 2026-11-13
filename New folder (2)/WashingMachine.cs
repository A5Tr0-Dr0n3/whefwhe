using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class WashingMachine : MonoBehaviour
{
	public SafeCoroutine Vibrate(float time)
	{
		return GlobalCoroutineContainer.AddThread(VibrateCoroutine(time), this);
	}

	private IEnumerator VibrateCoroutine(float time)
	{
		float timer = 0f;
		float rotTimer = 0f;
		Quaternion startRot = transform.rotation;
		Quaternion targetRot = Quaternion.identity;
		while (timer < time)
		{
			timer += Time.deltaTime;
			rotTimer += Time.deltaTime * 10f;
			if (rotTimer >= 1f)
			{
				targetRot = Quaternion.AngleAxis(Random.Range(-30, 30), transform.forward);
				rotTimer = 0f;
			}
			transform.rotation = Quaternion.Slerp(startRot, targetRot, rotTimer);
			yield return null;
		}
	}
}
