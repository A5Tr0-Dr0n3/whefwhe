using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleVR.KeyboardDemo;

public class KeyboardDelegateExample : GvrKeyboardDelegateBase
{
	public Text KeyboardText;

	public Canvas UpdateCanvas;

	private const string DD_KEYBOARD_NOT_INSTALLED_MSG = "Please update the Daydream Keyboard app from the Play Store.";

	public override event EventHandler KeyboardHidden;

	public override event EventHandler KeyboardShown;

	private void Awake()
	{
		if (UpdateCanvas != null)
		{
			UpdateCanvas.gameObject.SetActive(value: false);
		}
	}

	public override void OnKeyboardShow()
	{
		Debug.Log("Calling Keyboard Show Delegate!");
		KeyboardShown?.Invoke(this, null);
	}

	public override void OnKeyboardHide()
	{
		Debug.Log("Calling Keyboard Hide Delegate!");
		KeyboardHidden?.Invoke(this, null);
	}

	public override void OnKeyboardUpdate(string text)
	{
		if (KeyboardText != null)
		{
			KeyboardText.text = text;
		}
		else
		{
			Debug.Log("Keyboard text is null....");
		}
	}

	public override void OnKeyboardEnterPressed(string text)
	{
		Debug.Log("Calling Keyboard Enter Pressed Delegate: " + text);
	}

	public override void OnKeyboardError(GvrKeyboardError errCode)
	{
		Debug.Log("Calling Keyboard Error Delegate: ");
		switch (errCode)
		{
		case GvrKeyboardError.UNKNOWN:
			Debug.Log("Unknown Error");
			break;
		case GvrKeyboardError.SERVICE_NOT_CONNECTED:
			Debug.Log("Service not connected");
			break;
		case GvrKeyboardError.NO_LOCALES_FOUND:
			Debug.Log("No locales found");
			break;
		case GvrKeyboardError.SDK_LOAD_FAILED:
			Debug.LogWarning("Please update the Daydream Keyboard app from the Play Store.");
			if (KeyboardText != null)
			{
				KeyboardText.text = "Please update the Daydream Keyboard app from the Play Store.";
			}
			if (UpdateCanvas != null)
			{
				UpdateCanvas.gameObject.SetActive(value: true);
			}
			break;
		}
	}

	public void LaunchPlayStore()
	{
		if (UpdateCanvas != null)
		{
			UpdateCanvas.gameObject.SetActive(value: false);
			Debug.LogError("GVR Keyboard available only on Android.");
		}
	}
}
