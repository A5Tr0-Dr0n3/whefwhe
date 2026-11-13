using UnityEngine;

public class ObjectSwapper : MonoBehaviour
{
	public GameObject CHINESE_S;

	public GameObject CHINESE_T;

	public GameObject ENGLISH;

	public GameObject FRENCH;

	public GameObject GERMAN;

	public GameObject ITALIAN;

	public GameObject JAPANESE;

	public GameObject KOREAN;

	public GameObject SPANISH;

	[HideInInspector]
	public GameObject activeObject;

	private void OnEnable()
	{
		PersistentDataManager.onLanguageChange += SwapObjects;
		SwapObjects();
	}

	private void OnDisable()
	{
		PersistentDataManager.onLanguageChange -= SwapObjects;
	}

	public void SwapObjects()
	{
		DisableAllObjects();
		switch (PersistentDataManager.GetSubtitleLanguage())
		{
		case VVR_METASAVEDATA.SubtitlesState.CHINESE_S:
			if (CHINESE_S != null)
			{
				activeObject = CHINESE_S;
				CHINESE_S.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.CHINESE_T:
			if (CHINESE_T != null)
			{
				activeObject = CHINESE_T;
				CHINESE_T.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.ENGLISH:
			if (ENGLISH != null)
			{
				activeObject = ENGLISH;
				ENGLISH.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.NONE:
			if (ENGLISH != null)
			{
				activeObject = ENGLISH;
				ENGLISH.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.FRENCH:
			if (FRENCH != null)
			{
				activeObject = FRENCH;
				FRENCH.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.GERMAN:
			if (GERMAN != null)
			{
				activeObject = GERMAN;
				GERMAN.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.ITALIAN:
			if (ITALIAN != null)
			{
				activeObject = ITALIAN;
				ITALIAN.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.JAPANESE:
			if (JAPANESE != null)
			{
				activeObject = JAPANESE;
				JAPANESE.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.KOREAN:
			if (KOREAN != null)
			{
				activeObject = KOREAN;
				KOREAN.SetActive(value: true);
			}
			break;
		case VVR_METASAVEDATA.SubtitlesState.SPANISH:
			if (SPANISH != null)
			{
				activeObject = SPANISH;
				SPANISH.SetActive(value: true);
			}
			break;
		default:
			activeObject = null;
			break;
		}
	}

	private void DisableAllObjects()
	{
		if (CHINESE_S != null)
		{
			CHINESE_S.SetActive(value: false);
		}
		if (CHINESE_T != null)
		{
			CHINESE_T.SetActive(value: false);
		}
		if (ENGLISH != null)
		{
			ENGLISH.SetActive(value: false);
		}
		if (FRENCH != null)
		{
			FRENCH.SetActive(value: false);
		}
		if (GERMAN != null)
		{
			GERMAN.SetActive(value: false);
		}
		if (ITALIAN != null)
		{
			ITALIAN.SetActive(value: false);
		}
		if (JAPANESE != null)
		{
			JAPANESE.SetActive(value: false);
		}
		if (KOREAN != null)
		{
			KOREAN.SetActive(value: false);
		}
		if (SPANISH != null)
		{
			SPANISH.SetActive(value: false);
		}
	}
}
