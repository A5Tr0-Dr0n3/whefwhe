using System.Collections.Generic;
using UnityEngine;

public class BackstageJobDisplay : MonoBehaviour
{
	public enum DisplayState
	{
		InUse,
		Error,
		Offline
	}

	private static string[] humanNames = new string[14]
	{
		"TEXT_212", "TEXT_213", "TEXT_214", "TEXT_215", "TEXT_216", "TEXT_217", "TEXT_218", "TEXT_219", "TEXT_220", "TEXT_221",
		"TEXT_222", "TEXT_223", "TEXT_321", "TEXT_322"
	};

	private static List<int> AIDisplayList;

	private static List<int> humanDisplayList;

	private bool useTimer;

	[SerializeField]
	private GameObject AIDisplayParent;

	[SerializeField]
	private GameObject lightError;

	[SerializeField]
	private GameObject lightOnline;

	[SerializeField]
	private GameObject lightOffline;

	[SerializeField]
	private TextSwapper AIName;

	[SerializeField]
	private TextSwapper humanName;

	[SerializeField]
	private TextSwapper rating;

	[SerializeField]
	private TextSwapper timer;

	private static string[] fakeAINames = new string[9] { "TEXT_199", "TEXT_200", "TEXT_201", "TEXT_202", "TEXT_203", "TEXT_204", "TEXT_205", "TEXT_206", "TEXT_207" };

	[SerializeField]
	private GameObject[] fakeAIModels;

	private static string[] realAINames = new string[4] { "TEXT_208", "TEXT_209", "TEXT_210", "TEXT_211" };

	[SerializeField]
	private GameObject[] AIModels;

	private GameObject displayedAI;

	private float ticker;

	private float minutes;

	private float seconds;

	private float timeElapsed;

	public static void ResetDisplayList()
	{
		AIDisplayList = new List<int>();
		humanDisplayList = new List<int>();
	}

	public void SetupDisplay(DisplayState displayState)
	{
		switch (displayState)
		{
		case DisplayState.InUse:
		{
			int num2 = Random.Range(0, fakeAINames.Length);
			while (AIDisplayList.Contains(num2))
			{
				num2 = Random.Range(0, fakeAINames.Length);
			}
			AIDisplayList.Add(num2);
			AIName.SetNewKey(fakeAINames[num2]);
			fakeAIModels[num2].SetActive(value: true);
			displayedAI = fakeAIModels[num2];
			int num3 = Random.Range(0, humanNames.Length);
			while (humanDisplayList.Contains(num3))
			{
				num3 = Random.Range(0, humanNames.Length);
			}
			humanDisplayList.Add(num3);
			humanName.SetNewKey(humanNames[num3]);
			rating.UseAltString(Random.Range(4, 6).ToString());
			lightOnline.SetActive(value: true);
			useTimer = true;
			timeElapsed = Random.Range(30f, 300f);
			break;
		}
		case DisplayState.Offline:
			lightOffline.SetActive(value: true);
			AIName.UseAltString("-");
			humanName.UseAltString("-");
			rating.UseAltString("-");
			timer.UseAltString("-");
			break;
		default:
		{
			VVR_HUB.JobStates jobState = PersistentDataManager.GetJobState();
			int num = 0;
			switch (jobState)
			{
			case VVR_HUB.JobStates.Garden:
				num = ((PersistentDataManager.GetSavedHubState() != VVR_HUB.HubStates.Intro) ? 2 : 0);
				break;
			case VVR_HUB.JobStates.Sunset:
				num = 1;
				break;
			case VVR_HUB.JobStates.Tumbleweed1:
				num = 3;
				break;
			case VVR_HUB.JobStates.ButterSnack:
				num = 0;
				break;
			case VVR_HUB.JobStates.GardenNight:
				num = 2;
				break;
			case VVR_HUB.JobStates.Tumbleweed2:
				num = 3;
				break;
			case VVR_HUB.JobStates.LOOP:
				num = 0;
				break;
			}
			AIModels[num].SetActive(value: true);
			displayedAI = AIModels[num];
			AIName.SetNewKey(realAINames[num]);
			humanName.SetNewKey("TEXT_040");
			humanName.UseAltString(alt: false);
			rating.SetNewKey("TEXT_224");
			rating.UseAltString(alt: false);
			timer.SetNewKey("TEXT_224");
			timer.UseAltString(alt: false);
			lightError.SetActive(value: true);
			break;
		}
		}
		if (displayedAI != null)
		{
			displayedAI.transform.parent = AIDisplayParent.transform;
		}
	}

	private void Update()
	{
		if (!useTimer)
		{
			return;
		}
		ticker += Time.deltaTime;
		timeElapsed += Time.deltaTime;
		if (!(ticker < 1f))
		{
			minutes = Mathf.FloorToInt(timeElapsed / 60f);
			seconds = Mathf.FloorToInt(timeElapsed % 60f);
			if (seconds < 10f)
			{
				timer.UseAltString(minutes + ":0" + seconds);
			}
			else
			{
				timer.UseAltString(minutes + ":" + seconds);
			}
			ticker = 0f;
		}
	}
}
