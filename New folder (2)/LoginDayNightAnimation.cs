using System;
using UnityEngine;

public class LoginDayNightAnimation : MonoBehaviour
{
	[SerializeField]
	private Animator mAnimator;

	private void Update()
	{
		mAnimator.Play("DayNight", -1, RealWorldTimeUtility.NormalizeTime(DateTime.Now));
	}

	internal void Disable()
	{
		mAnimator.gameObject.SetActive(value: false);
	}
}
