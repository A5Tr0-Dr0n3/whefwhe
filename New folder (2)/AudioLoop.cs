using System;
using System.Collections.Generic;
using TButt;
using UnityEngine;

[Serializable]
public class AudioLoop : MonoBehaviour
{
	public GvrAudioSource gvrSource;

	public AudioSource flatSource;

	public bool playOnAwake;

	public TestAudio loop;

	private float _initialSpatialVolume;

	private float _initialFlatVolume;

	private int _disableSamplePoint;

	private bool _disabledWhilePlaying;

	[EnumArray(typeof(TBInput.Controller))]
	public List<GvrAudioSource> controllerSources;

	private void OnEnable()
	{
		if (_disabledWhilePlaying)
		{
			if (gvrSource != null)
			{
				gvrSource.timeSamples = _disableSamplePoint;
			}
			if (flatSource != null)
			{
				flatSource.timeSamples = _disableSamplePoint;
			}
			Play();
			_disabledWhilePlaying = false;
		}
	}

	private void OnDisable()
	{
		if ((flatSource != null && flatSource.isPlaying) || (gvrSource != null && gvrSource.isPlaying))
		{
			_disabledWhilePlaying = true;
			if (flatSource != null)
			{
				_disableSamplePoint = flatSource.timeSamples;
			}
		}
	}

	private void Awake()
	{
		if (gvrSource == null)
		{
			gvrSource = GetComponent<GvrAudioSource>();
		}
		if (flatSource == null)
		{
			flatSource = GetComponent<AudioSource>();
		}
		if (playOnAwake)
		{
			Play();
		}
	}

	private GvrAudioSource parsedSpatialSource(TBInput.Controller controller)
	{
		if (controller == TBInput.Controller.None)
		{
			return gvrSource;
		}
		if ((int)controller < controllerSources.Count)
		{
			return controllerSources[(int)controller];
		}
		return gvrSource;
	}

	public void Play(TBInput.Controller controller = TBInput.Controller.None)
	{
		if (parsedSpatialSource(controller) != null)
		{
			parsedSpatialSource(controller).loop = true;
			parsedSpatialSource(controller).clip = loop.clip;
			parsedSpatialSource(controller).volume = loop.volume;
			parsedSpatialSource(controller).Play();
		}
		if (flatSource != null && loop.spatial < 1f)
		{
			flatSource.loop = true;
			flatSource.clip = loop.clip;
			flatSource.volume = FlatVolumeScale(loop.volume, loop.spatial);
			flatSource.Play();
		}
	}

	public void Stop(TBInput.Controller controller = TBInput.Controller.None)
	{
		if (parsedSpatialSource(controller) != null)
		{
			parsedSpatialSource(controller).Stop();
		}
		if (flatSource != null)
		{
			flatSource.Stop();
		}
	}

	public void Pause(TBInput.Controller controller = TBInput.Controller.None)
	{
		if (parsedSpatialSource(controller) != null)
		{
			parsedSpatialSource(controller).Pause();
		}
		if (flatSource != null)
		{
			flatSource.Pause();
		}
	}

	public void UnPause(TBInput.Controller controller = TBInput.Controller.None)
	{
		if (parsedSpatialSource(controller) != null)
		{
			parsedSpatialSource(controller).UnPause();
		}
		if (flatSource != null)
		{
			flatSource.UnPause();
		}
	}

	public void SetSourceVolume(float input, TBInput.Controller controller = TBInput.Controller.None)
	{
		if (input <= 0.0001f)
		{
			if (parsedSpatialSource(controller) != null)
			{
				parsedSpatialSource(controller).volume = input;
				parsedSpatialSource(controller).Pause();
			}
			if (flatSource != null)
			{
				flatSource.volume = input;
				flatSource.Pause();
			}
		}
		else
		{
			if (parsedSpatialSource(controller) != null)
			{
				parsedSpatialSource(controller).volume = loop.volume * input;
				parsedSpatialSource(controller).UnPause();
			}
			if (flatSource != null)
			{
				flatSource.volume = loop.volume * input;
				flatSource.UnPause();
			}
		}
	}

	public void SetSourcePitch(float input, TBInput.Controller controller = TBInput.Controller.None)
	{
		if (input <= 0.001f)
		{
			if (parsedSpatialSource(controller) != null)
			{
				parsedSpatialSource(controller).Pause();
			}
			if (flatSource != null)
			{
				flatSource.Pause();
			}
			return;
		}
		if (parsedSpatialSource(controller) != null)
		{
			parsedSpatialSource(controller).pitch = input;
			parsedSpatialSource(controller).UnPause();
		}
		if (flatSource != null)
		{
			flatSource.pitch = input;
			flatSource.UnPause();
		}
	}

	private float FlatVolumeScale(float volume, float spatial)
	{
		return volume * (1f - spatial);
	}

	private float SpatialVolumeScale(float volume, float spatial)
	{
		return volume * spatial;
	}
}
