using System;

[Serializable]
public class VVR_METASAVEDATA
{
	public enum SubtitlesState
	{
		NONE,
		ENGLISH,
		KOREAN,
		JAPANESE,
		CHINESE_T,
		CHINESE_S,
		GERMAN,
		FRENCH,
		ITALIAN,
		SPANISH,
		NUMELEMENTS
	}

	private const string neverBackedUpString = "[Never backed up a save]";

	public SubtitlesState subtitlesState = SubtitlesState.NUMELEMENTS;

	public bool canActivateDebug;

	public DebugPanelType debugPanelType;

	public bool useHacktivitudeMenuFlag;

	public string lastBackedUpVersionNumber = "[Never backed up a save]";

	public StandaloneGraphicsMode graphicsMode = StandaloneGraphicsMode.Medium;

	public SerializableDictionary<string, int> arbitraryIntDictionary;

	public SerializableDictionary<string, string> arbitraryStringDictionary;
}
