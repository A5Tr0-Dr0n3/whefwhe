using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleVR.VideoDemo;

public class SwitchVideos : MonoBehaviour
{
	public GameObject localVideoSample;

	public GameObject dashVideoSample;

	public GameObject panoVideoSample;

	private GameObject[] videoSamples;

	public Text missingLibText;

	public void Awake()
	{
		videoSamples = new GameObject[3];
		videoSamples[0] = localVideoSample;
		videoSamples[1] = dashVideoSample;
		videoSamples[2] = panoVideoSample;
		string text = "Video Support libraries not found or could not be loaded!\nPlease add the <b>GVRVideoPlayer.unitypackage</b>\n to this project";
		if (!(missingLibText != null))
		{
			return;
		}
		try
		{
			IntPtr intPtr = GvrVideoPlayerTexture.CreateVideoPlayer();
			if (intPtr != IntPtr.Zero)
			{
				GvrVideoPlayerTexture.DestroyVideoPlayer(intPtr);
				missingLibText.enabled = false;
			}
			else
			{
				missingLibText.text = text;
				missingLibText.enabled = true;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			missingLibText.text = text;
			missingLibText.enabled = true;
		}
	}

	public void ShowMainMenu()
	{
		ShowSample(-1);
	}

	public void OnFlatLocal()
	{
		ShowSample(0);
	}

	public void OnDash()
	{
		ShowSample(1);
	}

	public void On360Video()
	{
		ShowSample(2);
	}

	private void ShowSample(int index)
	{
		if (missingLibText != null && missingLibText.enabled)
		{
			index = -1;
		}
		for (int i = 0; i < videoSamples.Length; i++)
		{
			if (!(videoSamples[i] != null))
			{
				continue;
			}
			if (i != index)
			{
				if (videoSamples[i].activeSelf)
				{
					videoSamples[i].GetComponentInChildren<GvrVideoPlayerTexture>().CleanupVideo();
				}
			}
			else
			{
				videoSamples[i].GetComponentInChildren<GvrVideoPlayerTexture>().ReInitializeVideo();
			}
			videoSamples[i].SetActive(i == index);
		}
		GetComponent<Canvas>().enabled = index == -1;
	}
}
