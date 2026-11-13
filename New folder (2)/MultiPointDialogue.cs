using System.Collections.Generic;
using UnityEngine;

public class MultiPointDialogue : MonoBehaviour
{
	private class DialogueSourceTracker
	{
		public VVRDialogueSource source;

		private float _pitch = 1f;

		private float _volume = 1f;

		public bool resetting;

		public float pitch
		{
			get
			{
				return _pitch;
			}
			set
			{
				source.mAudioSource.pitch = (_pitch = value);
			}
		}

		public float volume
		{
			get
			{
				return _volume;
			}
			set
			{
				source.mAudioSource.volume = (_volume = value);
			}
		}

		public DialogueSourceTracker(VVRDialogueSource dialogueSource)
		{
			source = dialogueSource;
		}
	}

	private class dspData
	{
		public Vector3 position;

		public float sqrMagnitude;

		public dspData(Vector3 myPosition)
		{
			position = myPosition;
			sqrMagnitude = 0f;
		}
	}

	public VVRDialogueSource mainDialogueSource;

	public List<VVRDialogueSource> addtlDialogueSource;

	public float pitchRangeMin = 0.98f;

	public float pitchRangeMax = 1.02f;

	public float timeOffsetMax = 0.05f;

	[SerializeField]
	private float _repositionSpeed = 3f;

	public List<Transform> dialogueSourcePositions;

	[SerializeField]
	private AudioSourceLoudness _loudness;

	[SerializeField]
	private float _shaderVolumeMult = 3f;

	[SerializeField]
	private float _shaderVolumeMultMain = 15f;

	private List<dspData> availableDsp;

	private List<DialogueSourceTracker> trackers;

	private Vector4[] _sourcePos;

	private float _asl;

	public bool isPlaying => trackers[0].source.isPlaying;

	public float time => trackers[0].source.mAudioSource.time;

	private void Start()
	{
		trackers = new List<DialogueSourceTracker>();
		for (int i = 0; i < addtlDialogueSource.Count + 1; i++)
		{
			if (i == 0)
			{
				trackers.Add(new DialogueSourceTracker(mainDialogueSource));
				continue;
			}
			trackers.Add(new DialogueSourceTracker(addtlDialogueSource[i - 1]));
			trackers[i].pitch = Random.Range(pitchRangeMin, pitchRangeMax);
		}
		availableDsp = new List<dspData>();
		for (int j = 0; j < dialogueSourcePositions.Count; j++)
		{
			availableDsp.Add(new dspData(dialogueSourcePositions[j].position));
		}
		_sourcePos = new Vector4[trackers.Count];
		for (int k = 0; k < _sourcePos.Length; k++)
		{
			_sourcePos[k].x = trackers[k].source.transform.position.x;
			_sourcePos[k].y = trackers[k].source.transform.position.y;
			_sourcePos[k].z = trackers[k].source.transform.position.z;
			if (k == 0)
			{
				_sourcePos[k].w = ((_shaderVolumeMultMain != 0f) ? _shaderVolumeMultMain : 0.01f);
			}
			else
			{
				_sourcePos[k].w = ((_shaderVolumeMult != 0f) ? _shaderVolumeMult : 0.01f);
			}
		}
	}

	private void Update()
	{
		if (!isPlaying)
		{
			return;
		}
		for (int i = 1; i < trackers.Count; i++)
		{
			if (!trackers[i].source.isPlaying)
			{
				continue;
			}
			if (trackers[i].resetting)
			{
				trackers[i].volume = Mathf.Lerp(trackers[i].volume, 1f, 2f * Time.deltaTime);
				if (trackers[i].volume >= 0.95f)
				{
					trackers[i].volume = 1f;
					trackers[i].resetting = false;
				}
			}
			else
			{
				trackers[i].volume = Mathf.Lerp(trackers[i].volume, 0f, Mathf.Abs(trackers[i].pitch - 1f) * 50f * Time.deltaTime);
				if (trackers[i].volume < 0.09f)
				{
					if (availableDsp.Count > 0)
					{
						UpdateDspDistances(availableDsp);
						int index = 0;
						if (availableDsp.Count >= 3)
						{
							index = Random.Range(0, 3);
						}
						availableDsp.Add(new dspData(trackers[i].source.transform.position));
						trackers[i].source.transform.position = availableDsp[index].position;
						availableDsp.RemoveAt(index);
					}
					trackers[i].pitch = Random.Range(pitchRangeMin, pitchRangeMax);
					trackers[i].source.mAudioSource.time = trackers[0].source.mAudioSource.time;
					trackers[i].resetting = true;
				}
			}
			_sourcePos[i].x = Mathf.Lerp(_sourcePos[i].x, trackers[i].source.transform.position.x, _repositionSpeed * Time.deltaTime);
			_sourcePos[i].y = Mathf.Lerp(_sourcePos[i].y, trackers[i].source.transform.position.y, _repositionSpeed * Time.deltaTime);
			_sourcePos[i].z = Mathf.Lerp(_sourcePos[i].z, trackers[i].source.transform.position.z, _repositionSpeed * Time.deltaTime);
			Shader.SetGlobalVectorArray("_SourcePos", _sourcePos);
		}
		_sourcePos[0].x = trackers[0].source.transform.position.x;
		_sourcePos[0].y = trackers[0].source.transform.position.y;
		_sourcePos[0].z = trackers[0].source.transform.position.z;
		_asl = Mathf.Clamp01(_loudness.GetFilteredLoudness() * 15f + 0.3f);
		Shader.SetGlobalFloat("_GlobalASL", _asl);
	}

	public void PlayClip(AudioClip rawAudioClip)
	{
		for (int i = 0; i < trackers.Count; i++)
		{
			trackers[i].source.PlayClip(rawAudioClip);
			if (i > 0)
			{
				trackers[i].source.mAudioSource.time = Random.Range(0f, timeOffsetMax);
			}
		}
	}

	public void PlayClip(NPCDialogueClip dialogueClip)
	{
		for (int i = 0; i < trackers.Count; i++)
		{
			trackers[i].source.PlayClip(dialogueClip);
			if (i > 0)
			{
				trackers[i].source.mAudioSource.time = Random.Range(0f, timeOffsetMax);
			}
		}
	}

	public void ForcePlayClipInterrupt(NPCDialogueClip dialogueClip)
	{
		for (int i = 0; i < trackers.Count; i++)
		{
			trackers[i].source.ForcePlayClipInterrupt(dialogueClip);
			if (i > 0)
			{
				trackers[i].source.mAudioSource.time = Random.Range(0f, timeOffsetMax);
			}
		}
	}

	public void ForcePlayClipOverwrite(NPCDialogueClip dialogueClip)
	{
		for (int i = 0; i < trackers.Count; i++)
		{
			trackers[i].source.ForcePlayClipOverwrite(dialogueClip);
			if (i > 0)
			{
				trackers[i].source.mAudioSource.time = Random.Range(0f, timeOffsetMax);
			}
		}
	}

	public void PurgePlaybackClipFromQueue(DialoguePlaybackInstance clipToPurge)
	{
		for (int i = 0; i < trackers.Count; i++)
		{
			trackers[i].source.PurgePlaybackClipFromQueue(clipToPurge);
		}
	}

	public void PurgeAllDialogue()
	{
		for (int i = 0; i < trackers.Count; i++)
		{
			trackers[i].source.PurgeAllDialogue();
		}
	}

	private void UpdateDspDistances(List<dspData> listOfDsp)
	{
		for (int i = 0; i < listOfDsp.Count; i++)
		{
			listOfDsp[i].sqrMagnitude = Vector3.SqrMagnitude(listOfDsp[i].position - EagerMaster.instance.player.transform.position);
		}
		listOfDsp.Sort((dspData x, dspData y) => x.sqrMagnitude.CompareTo(y.sqrMagnitude));
	}
}
