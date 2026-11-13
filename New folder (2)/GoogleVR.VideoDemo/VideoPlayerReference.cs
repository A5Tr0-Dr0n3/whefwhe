using UnityEngine;

namespace GoogleVR.VideoDemo;

public class VideoPlayerReference : MonoBehaviour
{
	public GvrVideoPlayerTexture player;

	private void Awake()
	{
		GetComponentInChildren<VideoControlsManager>(includeInactive: true).Player = player;
	}
}
