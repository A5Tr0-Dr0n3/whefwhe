using System;
using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class ElizaMouth : MonoBehaviour
{
	[Header("Color Transition")]
	public Color mouthOnColor;

	public float onTransitionTime;

	public Color mouthOffColor;

	public float offTransitionTime;

	private Renderer _renderer;

	private float transitionTime;

	private float t;

	public AudioSourceLoudness Loudness;

	public float LoudnessMultiplier;

	public AnimationCurve LoudnessCurve;

	private SafeCoroutine mouthCoroutine;

	private void Start()
	{
		_renderer = GetComponent<Renderer>();
	}

	private void Update()
	{
		_renderer.sharedMaterial.SetFloat("_Volume", LoudnessCurve.Evaluate(Mathf.Clamp01(Loudness.lerpLoudness * LoudnessMultiplier)));
	}

	public void TurnMouthOn(bool instant = false)
	{
		if (_renderer == null)
		{
			_renderer = GetComponent<Renderer>();
		}
		if (mouthCoroutine == null)
		{
			if (instant)
			{
				_renderer.sharedMaterial.SetColor("_Color", mouthOnColor);
			}
			else
			{
				mouthCoroutine = GlobalCoroutineContainer.AddThread(mouthOn(), this);
			}
		}
	}

	public void TurnMouthOff(bool instant = false)
	{
		if (_renderer == null)
		{
			_renderer = GetComponent<Renderer>();
		}
		if (mouthCoroutine == null)
		{
			if (instant)
			{
				_renderer.sharedMaterial.SetColor("_Color", mouthOffColor);
			}
			else
			{
				mouthCoroutine = GlobalCoroutineContainer.AddThread(mouthOff(), this);
			}
		}
	}

	private IEnumerator mouthOff()
	{
		transitionTime = 0f;
		while (transitionTime < offTransitionTime)
		{
			transitionTime += Time.deltaTime;
			if (transitionTime > offTransitionTime)
			{
				transitionTime = offTransitionTime;
			}
			t = transitionTime / offTransitionTime;
			t = Mathf.Sin(t * (float)Math.PI * 0.5f);
			if (_renderer == null)
			{
				_renderer = GetComponent<Renderer>();
			}
			_renderer.sharedMaterial.SetColor("_Color", Color.Lerp(mouthOnColor, mouthOffColor, t));
			yield return null;
		}
		mouthCoroutine = null;
	}

	private IEnumerator mouthOn()
	{
		transitionTime = 0f;
		if (_renderer == null)
		{
			_renderer = GetComponent<Renderer>();
		}
		while (transitionTime < onTransitionTime)
		{
			transitionTime += Time.deltaTime;
			if (transitionTime > onTransitionTime)
			{
				transitionTime = onTransitionTime;
			}
			t = transitionTime / onTransitionTime;
			t = Mathf.Sin(t * (float)Math.PI * 0.5f);
			_renderer.sharedMaterial.SetColor("_Color", Color.Lerp(mouthOffColor, mouthOnColor, t));
			yield return null;
		}
		mouthCoroutine = null;
	}
}
