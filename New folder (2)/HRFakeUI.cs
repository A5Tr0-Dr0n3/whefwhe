using System;
using UnityEngine;

public class HRFakeUI : MonoBehaviour
{
	[SerializeField]
	private TextMesh sessionTimerText;

	[SerializeField]
	private TextMesh worldTimeText;

	private int year;

	private float ticker;

	private float nextJobTimer;

	private int minutes;

	private int seconds;

	private void Start()
	{
		year = DateTime.Now.Year + 12;
		worldTimeText.text = DateTime.Now.ToString("MM\\/dd\\/" + year);
	}

	private void Update()
	{
		UpdateText();
	}

	private void UpdateText()
	{
		ticker += Time.deltaTime;
		if (!(ticker < 1f))
		{
			minutes = Mathf.FloorToInt(Time.realtimeSinceStartup / 60f);
			seconds = Mathf.FloorToInt(Time.realtimeSinceStartup % 60f);
			if (seconds < 10)
			{
				sessionTimerText.text = minutes + ":0" + seconds;
			}
			else
			{
				sessionTimerText.text = minutes + ":" + seconds;
			}
		}
	}
}
