using UnityEngine;

namespace GoogleVR.VideoDemo;

[RequireComponent(typeof(GvrVideoPlayerTexture))]
public class AutoPlayVideo : MonoBehaviour
{
	private bool done;

	private float t;

	private GvrVideoPlayerTexture player;

	public float delay = 2f;

	public bool loop;

	private void Start()
	{
		t = 0f;
		done = false;
		player = GetComponent<GvrVideoPlayerTexture>();
		if (player != null)
		{
			player.Init();
		}
	}

	private void Update()
	{
		if (player == null)
		{
			return;
		}
		if (player.PlayerState == GvrVideoPlayerTexture.VideoPlayerState.Ended && done && loop)
		{
			player.Pause();
			player.CurrentPosition = 0L;
			done = false;
			t = 0f;
		}
		else if (!done)
		{
			t += Time.deltaTime;
			if (t >= delay && player != null)
			{
				player.Play();
				done = true;
			}
		}
	}
}
