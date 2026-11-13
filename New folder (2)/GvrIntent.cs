public static class GvrIntent
{
	private const string METHOD_GET_INTENT = "getIntent";

	private const string METHOD_HASH_CODE = "hashCode";

	private const string METHOD_INTENT_GET_DATA_STRING = "getDataString";

	private const string METHOD_INTENT_GET_BOOLEAN_EXTRA = "getBooleanExtra";

	private const string EXTRA_VR_LAUNCH = "android.intent.extra.VR_LAUNCH";

	public static string GetData()
	{
		return null;
	}

	public static bool IsLaunchedFromVr()
	{
		return false;
	}

	public static int GetIntentHashCode()
	{
		return 0;
	}
}
