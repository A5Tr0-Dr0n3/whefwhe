using TMPro;
using UnityEngine;

public class TextSwapper : MonoBehaviour
{
	[SerializeField]
	private string key;

	[SerializeField]
	private string altString = string.Empty;

	[SerializeField]
	private TextMeshPro targetText;

	[SerializeField]
	private bool swapText = true;

	[SerializeField]
	private bool swapInUpdate = true;

	[SerializeField]
	private bool swapFont = true;

	private VVR_METASAVEDATA.SubtitlesState lastLanguage;

	public bool usingAltString;

	private TMP_FontAsset startFont;

	private Material startMat;

	private void OnEnable()
	{
		if (PersistentDataManager.GetSubtitleLanguage() != lastLanguage)
		{
			if (swapFont)
			{
				SwapFont();
			}
			SwapText();
		}
	}

	private void OnDisable()
	{
	}

	private void Awake()
	{
		if (targetText == null)
		{
			targetText = GetComponent<TextMeshPro>();
		}
		startFont = targetText.font;
		startMat = targetText.fontSharedMaterial;
		lastLanguage = PersistentDataManager.GetSubtitleLanguage();
	}

	private void Start()
	{
		Init();
		if (swapFont)
		{
			SwapFont();
		}
		SwapText();
	}

	private void Update()
	{
		if (swapInUpdate && PersistentDataManager.GetSubtitleLanguage() != lastLanguage)
		{
			if (swapFont)
			{
				SwapFont();
			}
			SwapText();
			lastLanguage = PersistentDataManager.GetSubtitleLanguage();
		}
	}

	public void SwapText()
	{
		if (!swapText)
		{
			return;
		}
		if (SubtitleLoader.instance == null)
		{
			SafeDebug.LogWarning("Trying to swap text before subtitle loader instance was initialized");
		}
		else if (!usingAltString)
		{
			if (SubtitleLoader.instance.GetDictionary().ContainsKey(key))
			{
				targetText.text = SubtitleLoader.instance.GetDictionary()[key].dialogueLines[0].text;
			}
			else
			{
				SafeDebug.Log("Key not set. [" + key + "] is not contained in dictionary.");
			}
		}
	}

	public void SetNewKey(string newKey)
	{
		if (key == newKey)
		{
			return;
		}
		if (SubtitleLoader.instance.GetDictionary().ContainsKey(newKey))
		{
			key = newKey;
			SwapText();
			if (newKey == "TEXT_262")
			{
				string text = targetText.text;
				text = text.Substring(0, text.Length - 2);
				text += PersistentDataManager.GetKitchenLoopCounter();
				text += ")";
				targetText.text = text;
			}
		}
		else
		{
			SafeDebug.Log("New key not set. [" + newKey + "] is not contained in dictionary.");
			SafeDebug.Log(string.Empty);
		}
	}

	private void Init()
	{
		if (targetText == null)
		{
			targetText = GetComponent<TextMeshPro>();
			if (targetText == null)
			{
				SafeDebug.LogError("There is no aossigned Text Mesh Pro text swapper will likely not work as intended");
			}
		}
	}

	public static string GetTextFromKey(string key, string time)
	{
		return SubtitleLoader.instance.GetDictionary()[key].dialogueLines[0].text;
	}

	public void SwapFont()
	{
		if (targetText == null)
		{
			targetText = GetComponent<TextMeshPro>();
		}
		targetText.font = FontMatContainer.instance.GetFontSet(startFont).GetCurrentFont();
		targetText.fontSharedMaterial = FontMatContainer.instance.GetMatSet(startMat).GetCurrentMaterial();
	}

	public void UseAltString(string newAltString)
	{
		altString = newAltString;
		UseAltString(alt: true);
	}

	public void UseAltString(bool alt)
	{
		if (alt)
		{
			usingAltString = true;
			targetText.text = altString;
		}
		else
		{
			usingAltString = false;
			targetText.text = SubtitleLoader.instance.GetDictionary()[key].dialogueLines[0].text;
		}
	}
}
