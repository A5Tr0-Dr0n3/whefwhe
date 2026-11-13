using System;
using System.Collections;
using System.Collections.Generic;
using TButt;
using UnityEngine;

[Serializable]
public class AudioEvent : MonoBehaviour
{
	public GvrAudioSource spatializedSource;

	[EnumArray(typeof(TBInput.Controller))]
	public List<GvrAudioSource> controllerSources;

	public TestAudio[] alwaysPlay;

	[Range(0f, 1f)]
	public float randVolume = 1f;

	public TestAudio[] randPlay;

	private GvrAudioSource parsedSpatialSource;

	private void Start()
	{
		if (spatializedSource == null)
		{
			spatializedSource = EagerMaster.instance.defaultGVRSource;
		}
	}

	public float Play(TBInput.Controller controller = TBInput.Controller.None, bool rumble = false)
	{
		float num = 0f;
		if (alwaysPlay.Length > 0)
		{
			playTracks(alwaysPlay, controller, rumble);
			num = alwaysPlay[0].clip.length;
		}
		if (randPlay.Length > 0)
		{
			TestAudio testAudio = randPlay[Mathf.FloorToInt(UnityEngine.Random.Range(0f, randPlay.Length))];
			if (testAudio.delaySwitch)
			{
				GlobalCoroutineContainer.AddThread(playTrack(testAudio, testAudio.delay, testAudio.volume * randVolume, controller, rumble), this);
			}
			else
			{
				GlobalCoroutineContainer.AddThread(playTrack(testAudio, 0f, testAudio.volume * randVolume, controller, rumble), this);
			}
			num = Mathf.Max(num, testAudio.clip.length);
		}
		return num;
	}

	private void playTracks(TestAudio[] tracks, TBInput.Controller controller, bool rumble)
	{
		for (int i = 0; i < tracks.Length; i++)
		{
			if (tracks[i].delaySwitch)
			{
				GlobalCoroutineContainer.AddThread(playTrack(tracks[i], tracks[i].delay, tracks[i].volume, controller, rumble), this);
			}
			else
			{
				GlobalCoroutineContainer.AddThread(playTrack(tracks[i], 0f, tracks[i].volume, controller, rumble), this);
			}
		}
	}

	private IEnumerator playTrack(TestAudio track, float delay, float volume, TBInput.Controller controller, bool rumble)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		parsedSpatialSource = null;
		if (controller == TBInput.Controller.None)
		{
			parsedSpatialSource = spatializedSource;
		}
		else if ((int)controller < controllerSources.Count)
		{
			parsedSpatialSource = controllerSources[(int)controller];
		}
		else
		{
			parsedSpatialSource = spatializedSource;
		}
		if (track.spatial > 0f && track.volume > 0f && (bool)parsedSpatialSource && parsedSpatialSource.isActiveAndEnabled)
		{
			parsedSpatialSource.PlayOneShot(track.clip, spatialVolumeScale(volume, track.spatial));
		}
		if (track.spatial < 1f && track.volume > 0f && EagerMaster.instance.flatSource.isActiveAndEnabled)
		{
			EagerMaster.instance.flatSource.PlayOneShot(track.clip, FlatVolumeScale(volume, track.spatial));
		}
		if (!rumble)
		{
			yield break;
		}
		if (controller == TBInput.Controller.None)
		{
			for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
			{
				TBInput.StartRumbleClip(ToolManager.ActiveToolGroups[i].Controller, track.clip);
			}
		}
		else
		{
			TBInput.StartRumbleClip(controller, track.clip);
		}
	}

	private float FlatVolumeScale(float volume, float spatial)
	{
		return volume * (1f - spatial);
	}

	private float spatialVolumeScale(float volume, float spatial)
	{
		return volume * spatial;
	}
}
