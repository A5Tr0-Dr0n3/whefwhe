using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using TMPro;
using UnityEngine;

public class ElizaEyeStates : MonoBehaviour
{
	public enum ElizaEyeState
	{
		EyesTurningOff,
		EyesTurningOn,
		EyeBlink,
		EyesOn,
		EyesOff
	}

	private ObjectBase elizaObject;

	public TextMeshPro rendererLeft;

	public TextMeshPro rendererRight;

	private string currentEyeString = "0";

	private string currentEyeStringAlt = "0";

	private string currentErrorString = "~";

	private int currentErrorToUse;

	private float changeErrorCount;

	public List<string> errorCharacters;

	private List<string> randomizedCharacters;

	private float blinkTime = 0.12f;

	[Header("Effect Properties")]
	public float flickerTime;

	public float flickerTimeLong;

	public float timeBetweenFlickers;

	public float timeBetweenFlickersLong;

	[Header("On Properties")]
	public float onWaitTime = 1f;

	public int onFlickers = 3;

	public int onLongFlickers = 2;

	[Header("Off Properties")]
	public float offWaitTime = 0.4f;

	public int offFlickers = 2;

	public int offLongFlickers = 2;

	private ElizaRotationFollow followReference;

	private ElizaEyeState _currentState;

	private int _currentFlicker;

	private SafeCoroutine eyesCoroutine;

	[Header("Solo Eliza Controller")]
	public bool startBlinking;

	public float blinkTriggerTimeMin;

	public float blinkTriggerTimeMax;

	private float blinkTriggerCountdown;

	public ElizaEyeState currentState => _currentState;

	private void Start()
	{
		elizaObject = transform.parent.GetComponent<ObjectBase>();
		randomizedCharacters = new List<string>();
		if (startBlinking)
		{
			_currentState = ElizaEyeState.EyesOn;
		}
	}

	private void Update()
	{
		if (startBlinking)
		{
			soloElizaBlinkController();
			if (_currentState != ElizaEyeState.EyeBlink)
			{
				rendererLeft.text = currentEyeString;
				rendererRight.text = currentEyeStringAlt;
			}
		}
		else if (_currentState == ElizaEyeState.EyeBlink || _currentState == ElizaEyeState.EyesOn)
		{
			if (elizaObject != null && elizaObject.isGrabbed && followReference.sceneReference.elizaIsOn)
			{
				rendererLeft.text = ">";
				rendererRight.text = "<";
			}
			else if (_currentState != ElizaEyeState.EyeBlink)
			{
				rendererLeft.text = currentEyeString;
				rendererRight.text = currentEyeStringAlt;
			}
		}
		else
		{
			if (_currentState != ElizaEyeState.EyesTurningOff)
			{
				return;
			}
			changeErrorCount -= Time.deltaTime;
			if (changeErrorCount <= 0f)
			{
				currentErrorToUse++;
				if (currentErrorToUse >= randomizedCharacters.Count)
				{
					randomizeErrorCharacters();
				}
				currentErrorString = randomizedCharacters[currentErrorToUse];
			}
			TextMeshPro textMeshPro = rendererLeft;
			string text = currentErrorString;
			rendererRight.text = text;
			textMeshPro.text = text;
		}
	}

	private void randomizeErrorCharacters()
	{
		randomizedCharacters = errorCharacters;
		currentErrorToUse = 0;
		changeErrorCount = 0f;
		System.Random random = new System.Random();
		int count = randomizedCharacters.Count;
		for (int i = 0; i < count; i++)
		{
			int index = i + (int)(random.NextDouble() * (double)(count - i));
			randomizedCharacters[index] = randomizedCharacters[i];
		}
	}

	private void soloElizaBlinkController()
	{
		if (currentState == ElizaEyeState.EyesOn)
		{
			blinkTriggerCountdown -= Time.deltaTime;
			if (blinkTriggerCountdown <= 0f)
			{
				blinkTriggerCountdown = UnityEngine.Random.Range(blinkTriggerTimeMin, blinkTriggerTimeMax);
				ElizaBlink("_");
			}
		}
	}

	public void TurnEyesOffInstant()
	{
		TextMeshPro textMeshPro = rendererLeft;
		string empty = string.Empty;
		rendererRight.text = empty;
		textMeshPro.text = empty;
		_currentState = ElizaEyeState.EyesOff;
	}

	public void StartEyesOn()
	{
		if (eyesCoroutine == null)
		{
			eyesCoroutine = GlobalCoroutineContainer.AddThread(EyesOn(), this);
		}
	}

	public void StartEyesOff()
	{
		randomizeErrorCharacters();
		eyesCoroutine = GlobalCoroutineContainer.AddThread(EyesOff(), this);
	}

	public void SetElizaState(ElizaEyeState newState)
	{
		_currentState = newState;
	}

	public void ElizaBlink(string blinkString, float nBlinkTime = 0.1f)
	{
		if (eyesCoroutine == null && _currentState == ElizaEyeState.EyesOn)
		{
			eyesCoroutine = GlobalCoroutineContainer.AddThread(EyesBlink(blinkString, nBlinkTime), this);
		}
	}

	public void SetFollowReference(ElizaRotationFollow newFollow)
	{
		followReference = newFollow;
	}

	public void SetEyes(string newEye, string otherEye = "")
	{
		currentEyeString = newEye;
		if (otherEye != string.Empty)
		{
			rendererLeft.text = newEye;
			rendererRight.text = otherEye;
			currentEyeStringAlt = otherEye;
		}
		else
		{
			TextMeshPro textMeshPro = rendererLeft;
			string text = currentEyeString;
			rendererRight.text = text;
			textMeshPro.text = text;
			currentEyeStringAlt = newEye;
		}
	}

	public void SetEyesResting()
	{
		SetEyes("0", string.Empty);
	}

	public void SetEyesMoney()
	{
		SetEyes("$", string.Empty);
	}

	public void SetEyesProcessing()
	{
		SetEyes(".", string.Empty);
	}

	public void SetEyesHappy()
	{
		SetEyes("^", string.Empty);
	}

	public void SetEyesMischievious()
	{
		SetEyes(";)", string.Empty);
	}

	public void SetEyesTrademark()
	{
		SetEyes("©", string.Empty);
	}

	public void SetEyesConfused()
	{
		SetEyes("?", string.Empty);
	}

	public void SetEyesExcited()
	{
		SetEyes("!", string.Empty);
	}

	public void SetEyesWide()
	{
		SetEyes("0 ", "  0");
	}

	public void SetEyesNarrow()
	{
		SetEyes(" .", ". ");
	}

	public void SetEyesWink()
	{
		SetEyes("0", "_");
	}

	public void SetEyesRelaxed()
	{
		SetEyes("V", string.Empty);
	}

	public void SetEyesLookLeft()
	{
		SetEyes("<", string.Empty);
	}

	private IEnumerator EyesOn()
	{
		_currentState = ElizaEyeState.EyesTurningOn;
		yield return new WaitForSeconds(onWaitTime);
		for (int i = 0; i < onLongFlickers; i++)
		{
			yield return new WaitForSeconds(flickerTimeLong);
			yield return new WaitForSeconds(timeBetweenFlickersLong);
		}
		string text;
		for (int j = 0; j < onFlickers; j++)
		{
			TextMeshPro textMeshPro = rendererLeft;
			text = currentEyeString;
			rendererRight.text = text;
			textMeshPro.text = text;
			yield return new WaitForSeconds(flickerTime);
			TextMeshPro textMeshPro2 = rendererLeft;
			text = string.Empty;
			rendererRight.text = text;
			textMeshPro2.text = text;
			yield return new WaitForSeconds(timeBetweenFlickers);
		}
		TextMeshPro textMeshPro3 = rendererLeft;
		text = currentEyeString;
		rendererRight.text = text;
		textMeshPro3.text = text;
		_currentState = ElizaEyeState.EyesOn;
		eyesCoroutine = null;
	}

	private IEnumerator EyesOff()
	{
		_currentState = ElizaEyeState.EyesTurningOff;
		yield return new WaitForSeconds(offWaitTime);
		string empty;
		for (int i = 0; i < offLongFlickers; i++)
		{
			TextMeshPro textMeshPro = rendererLeft;
			empty = string.Empty;
			rendererRight.text = empty;
			textMeshPro.text = empty;
			yield return new WaitForSeconds(timeBetweenFlickersLong);
			TextMeshPro textMeshPro2 = rendererLeft;
			empty = currentErrorString;
			rendererRight.text = empty;
			textMeshPro2.text = empty;
			yield return new WaitForSeconds(flickerTimeLong);
		}
		for (int j = 0; j < offFlickers; j++)
		{
			TextMeshPro textMeshPro3 = rendererLeft;
			empty = string.Empty;
			rendererRight.text = empty;
			textMeshPro3.text = empty;
			yield return new WaitForSeconds(timeBetweenFlickers);
			TextMeshPro textMeshPro4 = rendererLeft;
			empty = currentErrorString;
			rendererRight.text = empty;
			textMeshPro4.text = empty;
			yield return new WaitForSeconds(flickerTime);
		}
		_currentState = ElizaEyeState.EyesOff;
		TextMeshPro textMeshPro5 = rendererLeft;
		empty = string.Empty;
		rendererRight.text = empty;
		textMeshPro5.text = empty;
		eyesCoroutine = null;
	}

	private IEnumerator EyesBlink(string nBlinkString, float nBlinkTime = 0.1f)
	{
		string text;
		if (currentEyeString == "0")
		{
			blinkTime = nBlinkTime;
			TextMeshPro textMeshPro = rendererLeft;
			text = nBlinkString;
			rendererRight.text = text;
			textMeshPro.text = text;
			_currentState = ElizaEyeState.EyeBlink;
			yield return new WaitForSeconds(blinkTime);
		}
		eyesCoroutine = null;
		TextMeshPro textMeshPro2 = rendererLeft;
		text = currentEyeString;
		rendererRight.text = text;
		textMeshPro2.text = text;
		_currentState = ElizaEyeState.EyesOn;
	}
}
