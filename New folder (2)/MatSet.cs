using System;
using UnityEngine;

[Serializable]
public class MatSet
{
	public Material latinMat;

	public Material koreanMat;

	public Material jaMat;

	public Material twMat;

	public Material chMat;

	public Material GetCurrentMaterial()
	{
		return PersistentDataManager.GetSubtitleLanguage() switch
		{
			VVR_METASAVEDATA.SubtitlesState.ENGLISH => latinMat, 
			VVR_METASAVEDATA.SubtitlesState.KOREAN => koreanMat, 
			VVR_METASAVEDATA.SubtitlesState.JAPANESE => jaMat, 
			VVR_METASAVEDATA.SubtitlesState.CHINESE_T => twMat, 
			VVR_METASAVEDATA.SubtitlesState.CHINESE_S => chMat, 
			VVR_METASAVEDATA.SubtitlesState.GERMAN => latinMat, 
			VVR_METASAVEDATA.SubtitlesState.FRENCH => latinMat, 
			VVR_METASAVEDATA.SubtitlesState.ITALIAN => latinMat, 
			VVR_METASAVEDATA.SubtitlesState.SPANISH => latinMat, 
			_ => latinMat, 
		};
	}
}
