using System;
using TMPro;
using UnityEngine;

public class DroidClockBehavior : MonoBehaviour
{
	private string time;

	private static int weekFirstLine = 174;

	private static int monthFirstLine = 224;

	private DateTime currentTime;

	[SerializeField]
	private TextMeshPro clock;

	[SerializeField]
	private TextMeshPro date;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		PersistentDataManager.onLanguageChange += UpdateDateText;
		date.text = LocalizedDateLong(DateTime.Now);
	}

	private void OnDisable()
	{
		PersistentDataManager.onLanguageChange -= UpdateDateText;
	}

	private void Update()
	{
		currentTime = DateTime.Now;
		time = currentTime.ToString("hh:mm tt");
		time = time.Substring(0, time.Length - 3);
		if (time[0] == '0')
		{
			time = time.Substring(1);
		}
		if (clock.text != time)
		{
			UpdateDateText();
		}
		clock.text = time;
	}

	private void UpdateDateText()
	{
		date.text = LocalizedDateLong(currentTime);
	}

	public static string LocalizedDateLong(DateTime dateTime)
	{
		int month = dateTime.Month;
		int day = dateTime.Day;
		int dayOfWeek = (int)dateTime.DayOfWeek;
		string empty = string.Empty;
		return PersistentDataManager.GetSubtitleLanguage() switch
		{
			VVR_METASAVEDATA.SubtitlesState.NONE => DayOfTheWeek(dayOfWeek) + ", " + Month(month) + " " + day, 
			VVR_METASAVEDATA.SubtitlesState.ENGLISH => DayOfTheWeek(dayOfWeek) + ", " + Month(month) + " " + day, 
			VVR_METASAVEDATA.SubtitlesState.FRENCH => DayOfTheWeek(dayOfWeek) + " " + day + " " + Month(month), 
			VVR_METASAVEDATA.SubtitlesState.GERMAN => DayOfTheWeek(dayOfWeek) + ", " + day + ". " + Month(month), 
			VVR_METASAVEDATA.SubtitlesState.ITALIAN => DayOfTheWeek(dayOfWeek) + " " + day + " " + Month(month), 
			VVR_METASAVEDATA.SubtitlesState.SPANISH => DayOfTheWeek(dayOfWeek) + ", " + day + " DE " + Month(month), 
			VVR_METASAVEDATA.SubtitlesState.KOREAN => month + "월" + day + "일 " + DayOfTheWeek(dayOfWeek), 
			_ => month + "月 " + day + "日 " + DayOfTheWeek(dayOfWeek), 
		};
	}

	private static string DayOfTheWeek(int dotwNum)
	{
		string key = "TEXT_" + (weekFirstLine + dotwNum);
		return SubtitleLoader.instance.GetDictionary()[key].dialogueLines[0].text;
	}

	private static string Month(int monthNum)
	{
		string key = "TEXT_" + (monthFirstLine + monthNum);
		return SubtitleLoader.instance.GetDictionary()[key].dialogueLines[0].text;
	}
}
