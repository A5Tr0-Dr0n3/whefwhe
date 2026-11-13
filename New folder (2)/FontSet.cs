using System;
using TMPro;

[Serializable]
public class FontSet
{
	public TMP_FontAsset latinFont;

	public TMP_FontAsset koreanFont;

	public TMP_FontAsset jaFont;

	public TMP_FontAsset twFont;

	public TMP_FontAsset chFont;

	public TMP_FontAsset GetCurrentFont()
	{
		return PersistentDataManager.GetSubtitleLanguage() switch
		{
			VVR_METASAVEDATA.SubtitlesState.ENGLISH => latinFont, 
			VVR_METASAVEDATA.SubtitlesState.KOREAN => koreanFont, 
			VVR_METASAVEDATA.SubtitlesState.JAPANESE => jaFont, 
			VVR_METASAVEDATA.SubtitlesState.CHINESE_T => twFont, 
			VVR_METASAVEDATA.SubtitlesState.CHINESE_S => chFont, 
			VVR_METASAVEDATA.SubtitlesState.GERMAN => latinFont, 
			VVR_METASAVEDATA.SubtitlesState.FRENCH => latinFont, 
			VVR_METASAVEDATA.SubtitlesState.ITALIAN => latinFont, 
			VVR_METASAVEDATA.SubtitlesState.SPANISH => latinFont, 
			_ => latinFont, 
		};
	}
}
