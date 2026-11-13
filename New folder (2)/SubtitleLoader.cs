using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;

public class SubtitleLoader : MonoBehaviour
{
	[Header("Dictionaries")]
	[Inspectionary("Key", "Value")]
	[HideInInspector]
	public LanguageDictionary serializableDictionaries;

	[Header("Settings")]
	public string filePath = "Assets/VVR_ASSETS/Subtitles/Resources/vvr-all-lines.xml";

	public Dictionary<VVR_METASAVEDATA.SubtitlesState, Dictionary<string, SubtitleContainer>> standardDictionaries;

	private XDocument currentDoc;

	private IEnumerable<XElement> items;

	private char[] delimChars = new char[1] { '-' };

	public static SubtitleLoader instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			SafeDebug.LogError("You have more than one subtitle loader. I'm deleting myself");
			Object.Destroy(gameObject);
		}
		ConstructDictionaries();
		DeleteSubtitles();
	}

	private void Update()
	{
	}

	public void LoadSubtitles(string[] filePaths)
	{
		for (int i = 0; i < filePaths.Length; i++)
		{
		}
	}

	public void LoadSubtitles(string filePath, VVR_METASAVEDATA.SubtitlesState state)
	{
		for (int i = 0; i < 10; i++)
		{
			if (!serializableDictionaries.ContainsKey((VVR_METASAVEDATA.SubtitlesState)i))
			{
				serializableDictionaries.Add((VVR_METASAVEDATA.SubtitlesState)i, new SubtitleDictionary());
			}
		}
		TextAsset textAsset = (TextAsset)Resources.Load("vvr-all-lines");
		currentDoc = XDocument.Parse(textAsset.text);
		items = currentDoc.Descendants("row");
		SubtitleContainer value = default;
		SubtitleContainer value2 = default;
		foreach (XElement item4 in items)
		{
			string input = item4.Element("stringID").ToString();
			input = Regex.Replace(input, "<.*?>", string.Empty);
			string[] array = input.Split(delimChars);
			if (array.Length > 1)
			{
				string text = array[0];
				if (!serializableDictionaries[state].ContainsKey(text))
				{
					string input2 = string.Empty;
					switch (state)
					{
					case VVR_METASAVEDATA.SubtitlesState.ENGLISH:
						input2 = item4.Element("lang").Element("en-US").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.KOREAN:
						input2 = item4.Element("lang").Element("ko").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.JAPANESE:
						input2 = item4.Element("lang").Element("ja").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.CHINESE_T:
						input2 = item4.Element("lang").Element("zh-TW").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.CHINESE_S:
						input2 = item4.Element("lang").Element("zh-CH").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.GERMAN:
						input2 = item4.Element("lang").Element("de").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.FRENCH:
						input2 = item4.Element("lang").Element("fr").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.ITALIAN:
						input2 = item4.Element("lang").Element("it").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.SPANISH:
						input2 = item4.Element("lang").Element("es").ToString();
						break;
					}
					input2 = Regex.Replace(input2, "<.*?>", string.Empty);
					input2 = Regex.Replace(input2, "&amp;", "&");
					string input3 = item4.Element("time").ToString();
					input3 = Regex.Replace(input3, "<.*?>", string.Empty);
					if (!float.TryParse(input3, out var result))
					{
						SafeDebug.LogError("Likely there was a typo in the xml on timekey: " + text + " your time string was: " + input3);
						result = 0f;
					}
					value.key = text;
					value.dialogueLines = new List<SubtitleLine>();
					SubtitleLine item = new SubtitleLine(result, input2);
					value.dialogueLines.Add(item);
					serializableDictionaries[state].Add(text, value);
				}
				else
				{
					SubtitleContainer subtitleContainer = serializableDictionaries[state][text];
					string input4 = string.Empty;
					switch (state)
					{
					case VVR_METASAVEDATA.SubtitlesState.ENGLISH:
						input4 = item4.Element("lang").Element("en-US").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.KOREAN:
						input4 = item4.Element("lang").Element("ko").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.JAPANESE:
						input4 = item4.Element("lang").Element("ja").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.CHINESE_T:
						input4 = item4.Element("lang").Element("zh-TW").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.CHINESE_S:
						input4 = item4.Element("lang").Element("zh-CH").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.GERMAN:
						input4 = item4.Element("lang").Element("de").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.FRENCH:
						input4 = item4.Element("lang").Element("fr").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.ITALIAN:
						input4 = item4.Element("lang").Element("it").ToString();
						break;
					case VVR_METASAVEDATA.SubtitlesState.SPANISH:
						input4 = item4.Element("lang").Element("es").ToString();
						break;
					}
					input4 = Regex.Replace(input4, "<.*?>", string.Empty);
					input4 = Regex.Replace(input4, "&amp;", "&");
					string input5 = item4.Element("time").ToString();
					input5 = Regex.Replace(input5, "<.*?>", string.Empty);
					if (!float.TryParse(input5, out var result2))
					{
						SafeDebug.LogError("Likely there was a typo in the xml on timekey: " + text + " your time string was: " + input5);
						result2 = 0f;
					}
					SubtitleLine item2 = new SubtitleLine(result2, input4);
					subtitleContainer.dialogueLines.Add(item2);
				}
			}
			else if (!serializableDictionaries[state].ContainsKey(input))
			{
				string input6 = string.Empty;
				switch (state)
				{
				case VVR_METASAVEDATA.SubtitlesState.ENGLISH:
					input6 = item4.Element("lang").Element("en-US").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.KOREAN:
					input6 = item4.Element("lang").Element("ko").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.JAPANESE:
					input6 = item4.Element("lang").Element("ja").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.CHINESE_T:
					input6 = item4.Element("lang").Element("zh-TW").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.CHINESE_S:
					input6 = item4.Element("lang").Element("zh-CH").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.GERMAN:
					input6 = item4.Element("lang").Element("de").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.FRENCH:
					input6 = item4.Element("lang").Element("fr").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.ITALIAN:
					input6 = item4.Element("lang").Element("it").ToString();
					break;
				case VVR_METASAVEDATA.SubtitlesState.SPANISH:
					input6 = item4.Element("lang").Element("es").ToString();
					break;
				}
				input6 = Regex.Replace(input6, "<.*?>", string.Empty);
				input6 = Regex.Replace(input6, "&amp;", "&");
				string input7 = item4.Element("time").ToString();
				input7 = Regex.Replace(input7, "<.*?>", string.Empty);
				if (!float.TryParse(input7, out var result3))
				{
					SafeDebug.LogError("Likely there was a typo in the xml on timekey: " + input + " your time string was: " + input7);
					result3 = 0f;
				}
				value2.key = input;
				value2.dialogueLines = new List<SubtitleLine>();
				SubtitleLine item3 = new SubtitleLine(result3, input6);
				value2.dialogueLines.Add(item3);
				serializableDictionaries[state].Add(input, value2);
			}
		}
		Debug.Log("Subtitle dictionaries rebuilt");
	}

	public void DeleteSubtitles()
	{
		for (int i = 0; i < 10; i++)
		{
			if (serializableDictionaries.ContainsKey((VVR_METASAVEDATA.SubtitlesState)i))
			{
				serializableDictionaries[(VVR_METASAVEDATA.SubtitlesState)i].Clear();
			}
		}
		serializableDictionaries.Clear();
		serializableDictionaries = null;
		Debug.Log("Subtitles Deleted");
	}

	public SubtitleContainer GetSubtitleContainer(string k)
	{
		return standardDictionaries[PersistentDataManager.GetSubtitleLanguage()][k];
	}

	public Dictionary<string, SubtitleContainer> GetDictionary()
	{
		if (PersistentDataManager.GetSubtitleLanguage() != VVR_METASAVEDATA.SubtitlesState.NONE)
		{
			return standardDictionaries[PersistentDataManager.GetSubtitleLanguage()];
		}
		return standardDictionaries[VVR_METASAVEDATA.SubtitlesState.ENGLISH];
	}

	private void ConstructDictionaries()
	{
		standardDictionaries = new Dictionary<VVR_METASAVEDATA.SubtitlesState, Dictionary<string, SubtitleContainer>>();
		for (int i = 1; i < 10; i++)
		{
			Dictionary<string, SubtitleContainer> dictionary = new Dictionary<string, SubtitleContainer>();
			foreach (KeyValuePair<string, SubtitleContainer> item in serializableDictionaries[(VVR_METASAVEDATA.SubtitlesState)i])
			{
				dictionary.Add(item.Key, item.Value);
			}
			standardDictionaries.Add((VVR_METASAVEDATA.SubtitlesState)i, dictionary);
		}
	}
}
