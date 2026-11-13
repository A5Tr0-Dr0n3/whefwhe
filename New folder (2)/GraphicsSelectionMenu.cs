using TButt;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;

public class GraphicsSelectionMenu : MonoBehaviour
{
	[Header("Low Settings")]
	public TextMeshPro lowLabel;

	public GraphicsSelectButton lowButton;

	public GraphicsMode lowSettings;

	[Header("Medium Settings")]
	public TextMeshPro mediumLabel;

	public GraphicsSelectButton mediumButton;

	public GraphicsMode medSettings;

	[Header("High Settings")]
	public TextMeshPro highLabel;

	public GraphicsSelectButton highButton;

	public GraphicsMode highSettings;

	[Header("Scene References")]
	public ObjectButton toggleButton;

	public TextMeshPro toggleLabel;

	public Transform menuRoot;

	public LanguageSelectionMenu languageMenu;

	public Color opaque = Vector4.one;

	public Color semiOpaque = new Vector4(1f, 1f, 1f, 0.6f);

	internal bool menuOpen;

	internal StandaloneGraphicsMode currentGraphicsMode = StandaloneGraphicsMode.Medium;

	internal bool highlight;

	private bool justOpened;

	public bool Highlight
	{
		get
		{
			return highlight;
		}
		set
		{
			if (value)
			{
				toggleLabel.color = opaque;
			}
			else
			{
				toggleLabel.color = semiOpaque;
			}
			highlight = value;
		}
	}

	private void Start()
	{
		SetGraphicsMode(PersistentDataManager.GetGraphicsMode());
		toggleButton.onClickEvent.AddListener(Toggle);
		ForceToggle(forceClose: true);
	}

	public void IncrementGraphicsMode()
	{
		if (currentGraphicsMode != StandaloneGraphicsMode.High)
		{
			currentGraphicsMode++;
		}
		else
		{
			currentGraphicsMode = StandaloneGraphicsMode.Low;
		}
	}

	public void SetGraphicsMode(StandaloneGraphicsMode s, GraphicsSelectButton b)
	{
		lowButton.isCurrentSetting = false;
		mediumButton.isCurrentSetting = false;
		highButton.isCurrentSetting = false;
		lowButton.Highlight = false;
		mediumButton.Highlight = false;
		highButton.Highlight = false;
		b.isCurrentSetting = true;
		b.Highlight = false;
		currentGraphicsMode = s;
		switch (currentGraphicsMode)
		{
		case StandaloneGraphicsMode.Low:
			XRSettings.eyeTextureResolutionScale = lowSettings.renderScale;
			QualitySettings.antiAliasing = lowSettings.msaa;
			EagerMaster.instance.mainCamera.GetComponent<ValveCamera>().m_valveShadowTextureHeight /= lowSettings.shadowDivider;
			EagerMaster.instance.SetShadowDivider(lowSettings.shadowDivider);
			if (lowSettings.disablePostProcessing)
			{
				PostProcessLayer component3 = EagerMaster.instance.mainCamera.GetComponent<PostProcessLayer>();
				if ((bool)component3)
				{
					component3.enabled = false;
				}
			}
			else
			{
				PostProcessLayer component4 = EagerMaster.instance.mainCamera.GetComponent<PostProcessLayer>();
				if ((bool)component4)
				{
					component4.enabled = true;
				}
			}
			break;
		case StandaloneGraphicsMode.Medium:
			XRSettings.eyeTextureResolutionScale = medSettings.renderScale;
			QualitySettings.antiAliasing = medSettings.msaa;
			EagerMaster.instance.mainCamera.GetComponent<ValveCamera>().m_valveShadowTextureHeight /= medSettings.shadowDivider;
			EagerMaster.instance.SetShadowDivider(medSettings.shadowDivider);
			if (medSettings.disablePostProcessing)
			{
				PostProcessLayer component5 = EagerMaster.instance.mainCamera.GetComponent<PostProcessLayer>();
				if ((bool)component5)
				{
					component5.enabled = false;
				}
			}
			else
			{
				PostProcessLayer component6 = EagerMaster.instance.mainCamera.GetComponent<PostProcessLayer>();
				if ((bool)component6)
				{
					component6.enabled = true;
				}
			}
			break;
		case StandaloneGraphicsMode.High:
			XRSettings.eyeTextureResolutionScale = highSettings.renderScale;
			EagerMaster.instance.mainCamera.GetComponent<ValveCamera>().m_valveShadowTextureHeight /= highSettings.shadowDivider;
			EagerMaster.instance.SetShadowDivider(highSettings.shadowDivider);
			QualitySettings.antiAliasing = highSettings.msaa;
			if (highSettings.disablePostProcessing)
			{
				PostProcessLayer component = EagerMaster.instance.mainCamera.GetComponent<PostProcessLayer>();
				if ((bool)component)
				{
					component.enabled = false;
				}
			}
			else
			{
				PostProcessLayer component2 = EagerMaster.instance.mainCamera.GetComponent<PostProcessLayer>();
				if ((bool)component2)
				{
					component2.enabled = true;
				}
			}
			break;
		}
		PersistentDataManager.OverwriteGraphicsMode(s);
	}

	public void SetGraphicsMode(StandaloneGraphicsMode s)
	{
		switch (s)
		{
		case StandaloneGraphicsMode.Low:
			SetGraphicsMode(s, lowButton);
			break;
		case StandaloneGraphicsMode.Medium:
			SetGraphicsMode(s, mediumButton);
			break;
		case StandaloneGraphicsMode.High:
			SetGraphicsMode(s, highButton);
			break;
		}
	}

	public void ForceToggle(bool forceClose = false)
	{
		if (forceClose)
		{
			menuRoot.gameObject.SetActive(value: false);
			toggleButton.gameObject.SetActive(value: true);
			menuOpen = false;
			if (PersistentDataManager.GetSubtitleLanguage() == VVR_METASAVEDATA.SubtitlesState.ENGLISH)
			{
				languageMenu.subtitleSwitch.SetActive(value: true);
			}
			languageMenu.languageMenuText.gameObject.SetActive(value: true);
		}
	}

	public void Toggle()
	{
		if (menuOpen)
		{
			menuRoot.gameObject.SetActive(value: false);
			toggleButton.gameObject.SetActive(value: true);
			menuOpen = false;
			languageMenu.languageMenuText.gameObject.SetActive(value: true);
			if (PersistentDataManager.GetSubtitleLanguage() == VVR_METASAVEDATA.SubtitlesState.ENGLISH)
			{
				languageMenu.subtitleSwitch.SetActive(value: true);
			}
		}
		else
		{
			justOpened = true;
			menuRoot.gameObject.SetActive(value: true);
			toggleButton.gameObject.SetActive(value: false);
			menuOpen = true;
			languageMenu.CloseMenu();
			languageMenu.languageMenuText.gameObject.SetActive(value: false);
			languageMenu.subtitleSwitch.SetActive(value: false);
		}
	}

	private void Update()
	{
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (TBInput.GetButtonUp(TBInput.Button.Action1, ToolManager.ActiveToolGroups[i].Controller))
			{
				if (justOpened)
				{
					justOpened = false;
				}
				else if (menuOpen)
				{
					ForceToggle(forceClose: true);
				}
				break;
			}
		}
	}
}
