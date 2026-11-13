using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LastFrameTimePrinter : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro timeValue;

	private void Awake()
	{
		StartCoroutine(lastFrameCatcher());
	}

	private IEnumerator lastFrameCatcher()
	{
		yield return new WaitForEndOfFrame();
		timeValue.text = (DateTime.Now - GlobalSystemClockTimestampContainer.endOfLogoSystemTime).TotalSeconds.ToString("F5");
	}
}
