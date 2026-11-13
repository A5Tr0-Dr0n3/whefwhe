using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleVR.VideoDemo;

public class VideoControlsManager : MonoBehaviour
{
	private GameObject pauseSprite;

	private GameObject playSprite;

	private Slider videoScrubber;

	private Slider volumeSlider;

	private GameObject volumeWidget;

	private GameObject settingsPanel;

	private GameObject bufferedBackground;

	private Vector3 basePosition;

	private Text videoPosition;

	private Text videoDuration;

	public GvrVideoPlayerTexture Player { get; set; }

	private void Awake()
	{
		Text[] componentsInChildren = GetComponentsInChildren<Text>();
		foreach (Text text in componentsInChildren)
		{
			if (text.gameObject.name == "curpos_text")
			{
				videoPosition = text;
			}
			else if (text.gameObject.name == "duration_text")
			{
				videoDuration = text;
			}
		}
		RawImage[] componentsInChildren2 = GetComponentsInChildren<RawImage>(includeInactive: true);
		foreach (RawImage rawImage in componentsInChildren2)
		{
			if (rawImage.gameObject.name == "playImage")
			{
				playSprite = rawImage.gameObject;
			}
			else if (rawImage.gameObject.name == "pauseImage")
			{
				pauseSprite = rawImage.gameObject;
			}
		}
		Slider[] componentsInChildren3 = GetComponentsInChildren<Slider>(includeInactive: true);
		foreach (Slider slider in componentsInChildren3)
		{
			if (slider.gameObject.name == "video_slider")
			{
				videoScrubber = slider;
				videoScrubber.maxValue = 100f;
				videoScrubber.minValue = 0f;
				Image[] componentsInChildren4 = videoScrubber.GetComponentsInChildren<Image>();
				foreach (Image image in componentsInChildren4)
				{
					if (image.gameObject.name == "BufferedBackground")
					{
						bufferedBackground = image.gameObject;
					}
				}
			}
			else if (slider.gameObject.name == "volume_slider")
			{
				volumeSlider = slider;
			}
		}
		RectTransform[] componentsInChildren5 = GetComponentsInChildren<RectTransform>(includeInactive: true);
		foreach (RectTransform rectTransform in componentsInChildren5)
		{
			if (rectTransform.gameObject.name == "volume_widget")
			{
				volumeWidget = rectTransform.gameObject;
			}
			else if (rectTransform.gameObject.name == "settings_panel")
			{
				settingsPanel = rectTransform.gameObject;
			}
		}
	}

	private void Start()
	{
		ScrubberEvents[] componentsInChildren = GetComponentsInChildren<ScrubberEvents>(includeInactive: true);
		foreach (ScrubberEvents scrubberEvents in componentsInChildren)
		{
			scrubberEvents.ControlManager = this;
		}
		if (Player != null)
		{
			Player.Init();
		}
	}

	private void Update()
	{
		if (!Player.VideoReady || Player.IsPaused)
		{
			pauseSprite.SetActive(value: false);
			playSprite.SetActive(value: true);
		}
		else if (Player.VideoReady && !Player.IsPaused)
		{
			pauseSprite.SetActive(value: true);
			playSprite.SetActive(value: false);
		}
		if (Player.VideoReady)
		{
			if (basePosition == Vector3.zero)
			{
				basePosition = videoScrubber.handleRect.localPosition;
			}
			videoScrubber.maxValue = Player.VideoDuration;
			videoScrubber.value = Player.CurrentPosition;
			float value = (float)Player.BufferedPercentage / 100f;
			float num = Mathf.Clamp(value, 0f, 1f);
			bufferedBackground.transform.localScale = new Vector3(num, 1f, 1f);
			bufferedBackground.transform.localPosition = new Vector3(basePosition.x - basePosition.x * num, 0f, 0f);
			videoPosition.text = FormatTime(Player.CurrentPosition);
			videoDuration.text = FormatTime(Player.VideoDuration);
			if (volumeSlider != null)
			{
				volumeSlider.minValue = 0f;
				volumeSlider.maxValue = Player.MaxVolume;
				volumeSlider.value = Player.CurrentVolume;
			}
		}
		else
		{
			videoScrubber.value = 0f;
		}
	}

	public void OnVolumeUp()
	{
		if (Player.CurrentVolume < Player.MaxVolume)
		{
			Player.CurrentVolume++;
		}
	}

	public void OnVolumeDown()
	{
		if (Player.CurrentVolume > 0)
		{
			Player.CurrentVolume--;
		}
	}

	public void OnToggleVolume()
	{
		bool flag = !volumeWidget.activeSelf;
		volumeWidget.SetActive(flag);
		settingsPanel.SetActive(settingsPanel.activeSelf && !flag);
	}

	public void OnToggleSettings()
	{
		bool flag = !settingsPanel.activeSelf;
		settingsPanel.SetActive(flag);
		volumeWidget.SetActive(volumeWidget.activeSelf && !flag);
	}

	public void OnPlayPause()
	{
		bool isPaused = Player.IsPaused;
		if (isPaused)
		{
			Player.Play();
		}
		else
		{
			Player.Pause();
		}
		pauseSprite.SetActive(isPaused);
		playSprite.SetActive(!isPaused);
		CloseSubPanels();
	}

	public void OnVolumePositionChanged(float val)
	{
		if (Player.VideoReady)
		{
			Debug.Log("Setting current volume to " + val);
			Player.CurrentVolume = (int)val;
		}
	}

	public void CloseSubPanels()
	{
		volumeWidget.SetActive(value: false);
		settingsPanel.SetActive(value: false);
	}

	public void Fade(bool show)
	{
		if (show)
		{
			StartCoroutine(DoAppear());
		}
		else
		{
			StartCoroutine(DoFade());
		}
	}

	private IEnumerator DoAppear()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		while ((double)cg.alpha < 1.0)
		{
			cg.alpha += Time.deltaTime * 2f;
			yield return null;
		}
		cg.interactable = true;
	}

	private IEnumerator DoFade()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		while (cg.alpha > 0f)
		{
			cg.alpha -= Time.deltaTime;
			yield return null;
		}
		cg.interactable = false;
		CloseSubPanels();
	}

	private string FormatTime(long ms)
	{
		int num = (int)(ms / 1000);
		int num2 = num / 60;
		num %= 60;
		int num3 = num2 / 60;
		num2 %= 60;
		if (num3 > 0)
		{
			return $"{num3:00}:{num2:00}:{num:00}";
		}
		return $"{num2:00}:{num:00}";
	}
}
