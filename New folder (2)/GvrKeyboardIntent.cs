using UnityEngine;

public class GvrKeyboardIntent
{
	private class KeyboardCallback : AndroidJavaProxy
	{
		internal KeyboardCallback()
			: base("com.google.gvr.keyboardsupport.KeyboardFragment$KeyboardCallback")
		{
		}

		private void onPlayStoreResult()
		{
			Application.Quit();
		}
	}

	private const string FRAGMENT_CLASSNAME = "com.google.gvr.keyboardsupport.KeyboardFragment";

	private const string CALLBACK_CLASSNAME = "com.google.gvr.keyboardsupport.KeyboardFragment$KeyboardCallback";

	private static GvrKeyboardIntent theInstance;

	public static GvrKeyboardIntent Instance
	{
		get
		{
			if (theInstance == null)
			{
				theInstance = new GvrKeyboardIntent();
				if (!theInstance.InitializeFragment())
				{
					Debug.LogError("Cannot initialize fragment!");
					theInstance = null;
				}
			}
			return theInstance;
		}
	}

	protected bool InitializeFragment()
	{
		Debug.LogWarning("GvrKeyboardIntent requires the Android runtime environment");
		return false;
	}

	public void LaunchPlayStore()
	{
		Debug.LogError("GvrKeyboardIntent requires the Android runtime environment");
	}
}
