using System.Diagnostics;
using UnityEngine;

public static class TBLogging
{
	[Conditional("TB_ENABLE_LOGS")]
	public static void LogMessage(string message, string end = "\n <size=9><color=orange>via TButt</color></size>")
	{
		UnityEngine.Debug.Log(message + end);
	}

	[Conditional("TB_ENABLE_LOGS")]
	public static void LogWarning(string message, string end = "\n <size=9><color=orange>via TButt</color></size>")
	{
		UnityEngine.Debug.LogWarning(message + end);
	}

	[Conditional("TB_ENABLE_LOGS")]
	public static void LogError(string message, string end = "\n <size=9><color=orange>via TButt</color></size>")
	{
		UnityEngine.Debug.LogError("<color=red>" + message + "</color>" + end);
	}
}
