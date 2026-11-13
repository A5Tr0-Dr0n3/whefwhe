using TMPro;
using UnityEngine;

public class SelectSubtitlesButton : MonoBehaviour
{
	[SerializeField]
	private ObjectButton mButton;

	[SerializeField]
	private TextMeshPro mText;

	private bool subtitles;

	private void Awake()
	{
		subtitles = PersistentDataManager.GetSubtitleLanguage() == VVR_METASAVEDATA.SubtitlesState.ENGLISH;
		mText.text = getSubtitlesStateText(subtitles);
		mButton.onClickEvent.AddListener(clickAction);
		PersistentDataManager.onLanguageChange += UpdateState;
	}

	private void UpdateState()
	{
		mText.text = getSubtitlesStateText(PersistentDataManager.GetSubtitleLanguage() == VVR_METASAVEDATA.SubtitlesState.ENGLISH);
	}

	private void clickAction()
	{
		PersistentDataManager.ToggleSubtitles();
		UpdateState();
	}

	private string getSubtitlesStateText(bool subs)
	{
		return (!subs) ? "Subtitles: Off" : "Subtitles: On";
	}
}
