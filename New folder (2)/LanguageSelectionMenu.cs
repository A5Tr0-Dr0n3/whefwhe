using TButt;
using UnityEngine;

public class LanguageSelectionMenu : MonoBehaviour
{
	public bool hideMainButton = true;

	public LanguageSelectionButton[] languageButtons;

	public LanguageSelectionButton currentLanguage;

	public GameObject languageMenuText;

	public GameObject currLangIcon;

	public GameObject subtitleSwitch;

	public HubControls controlsMenu;

	public GraphicsSelectionMenu graphicsMenu;

	private bool justOpened;

	private void Start()
	{
		currentLanguage = GetButtonFromLang(PersistentDataManager.GetSubtitleLanguage());
		for (int i = 0; i < languageButtons.Length; i++)
		{
			languageButtons[i].menu = this;
			if (languageButtons[i] != currentLanguage)
			{
				languageButtons[i].Highlight = false;
			}
			else
			{
				languageButtons[i].Highlight = true;
			}
		}
		CloseMenu();
	}

	private void SetActive()
	{
		currentLanguage = GetButtonFromLang(PersistentDataManager.GetSubtitleLanguage());
		SetCurrentLanguage(currentLanguage.language);
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
				else
				{
					CloseMenu();
				}
				break;
			}
		}
	}

	public void OpenMenu()
	{
		justOpened = true;
		if (controlsMenu != null)
		{
			controlsMenu.Toggle(forceClose: true);
		}
		if (graphicsMenu != null)
		{
			graphicsMenu.ForceToggle(forceClose: true);
		}
		if (hideMainButton)
		{
			languageMenuText.gameObject.SetActive(value: false);
			subtitleSwitch.SetActive(value: false);
		}
		for (int i = 0; i < languageButtons.Length; i++)
		{
			languageButtons[i].gameObject.SetActive(value: true);
		}
		if (graphicsMenu != null)
		{
			graphicsMenu.toggleButton.gameObject.SetActive(value: false);
		}
		currLangIcon.SetActive(value: true);
		PositionCurrLangIcon();
	}

	public void CloseMenu()
	{
		languageMenuText.gameObject.SetActive(value: true);
		if (currentLanguage.language == VVR_METASAVEDATA.SubtitlesState.NONE || currentLanguage.language == VVR_METASAVEDATA.SubtitlesState.ENGLISH)
		{
			subtitleSwitch.SetActive(value: true);
		}
		else
		{
			subtitleSwitch.SetActive(value: false);
		}
		for (int i = 0; i < languageButtons.Length; i++)
		{
			languageButtons[i].gameObject.SetActive(value: false);
		}
		if (graphicsMenu != null && graphicsMenu.menuOpen)
		{
			languageMenuText.gameObject.SetActive(value: false);
			subtitleSwitch.SetActive(value: false);
		}
		if (graphicsMenu != null && !graphicsMenu.menuOpen)
		{
			graphicsMenu.toggleButton.gameObject.SetActive(value: true);
		}
		currLangIcon.SetActive(value: false);
	}

	public void SetCurrentLanguage(VVR_METASAVEDATA.SubtitlesState newLang)
	{
		currentLanguage.Highlight = false;
		currentLanguage = GetButtonFromLang(newLang);
		currentLanguage.Highlight = true;
		PersistentDataManager.OverwriteSubtitlesState(newLang);
		CloseMenu();
	}

	private LanguageSelectionButton GetButtonFromLang(VVR_METASAVEDATA.SubtitlesState lang)
	{
		if (lang == VVR_METASAVEDATA.SubtitlesState.ENGLISH)
		{
			lang = VVR_METASAVEDATA.SubtitlesState.NONE;
		}
		for (int i = 0; i < languageButtons.Length; i++)
		{
			if (languageButtons[i].language == lang)
			{
				return languageButtons[i];
			}
		}
		SafeDebug.Log("failed to get button from language");
		return null;
	}

	private void PositionCurrLangIcon()
	{
		currLangIcon.transform.localPosition = currentLanguage.transform.localPosition - transform.right * 0.068f;
	}
}
