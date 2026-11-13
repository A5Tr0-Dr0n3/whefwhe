using System;
using UnityEngine;

namespace Gvr.Internal;

public class EmulatorKeyboardProvider : IKeyboardProvider
{
	private GameObject stub;

	private bool showing;

	private GvrKeyboard.KeyboardCallback keyboardCallback;

	private string editorText = string.Empty;

	private GvrKeyboardInputMode mode;

	private Matrix4x4 worldMatrix;

	private bool isValid;

	public string EditorText
	{
		get
		{
			return editorText;
		}
		set
		{
			editorText = value;
		}
	}

	public EmulatorKeyboardProvider()
	{
		Debug.Log("Creating stub keyboard");
		showing = false;
		isValid = true;
	}

	public void SetInputMode(GvrKeyboardInputMode mode)
	{
		this.mode = mode;
	}

	public void OnPause()
	{
	}

	public void OnResume()
	{
	}

	public void ReadState(KeyboardState outState)
	{
		outState.mode = mode;
		outState.editorText = editorText;
		outState.worldMatrix = worldMatrix;
		outState.isValid = isValid;
		outState.isReady = true;
	}

	public bool Create(GvrKeyboard.KeyboardCallback keyboardEvent)
	{
		keyboardCallback = keyboardEvent;
		if (!isValid)
		{
			keyboardCallback(IntPtr.Zero, GvrKeyboardEvent.GVR_KEYBOARD_ERROR_SERVICE_NOT_CONNECTED);
		}
		return true;
	}

	public void Show(Matrix4x4 controllerMatrix, bool useRecommended, float distance, Matrix4x4 model)
	{
		if (!showing && isValid)
		{
			showing = true;
			worldMatrix = controllerMatrix;
			keyboardCallback(IntPtr.Zero, GvrKeyboardEvent.GVR_KEYBOARD_SHOWN);
		}
	}

	public void UpdateData()
	{
		if (!showing)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			keyboardCallback(IntPtr.Zero, GvrKeyboardEvent.GVR_KEYBOARD_TEXT_COMMITTED);
		}
		else if (Input.GetKeyDown(KeyCode.Backspace))
		{
			if (editorText.Length > 0)
			{
				editorText = editorText.Substring(0, editorText.Length - 1);
				SendUpdateNotification();
			}
		}
		else
		{
			if (Input.inputString.Length <= 0)
			{
				return;
			}
			switch (mode)
			{
			case GvrKeyboardInputMode.DEFAULT:
				editorText += Input.inputString;
				break;
			case GvrKeyboardInputMode.NUMERIC:
			{
				string inputString = Input.inputString;
				foreach (char c in inputString)
				{
					if (c >= '0' && c <= '9')
					{
						editorText += c;
					}
				}
				break;
			}
			}
			SendUpdateNotification();
		}
	}

	public void Render(int eye, Matrix4x4 modelview, Matrix4x4 projection, Rect viewport)
	{
	}

	public void Hide()
	{
		if (showing)
		{
			showing = false;
			keyboardCallback(IntPtr.Zero, GvrKeyboardEvent.GVR_KEYBOARD_HIDDEN);
		}
	}

	private void SendUpdateNotification()
	{
		keyboardCallback(IntPtr.Zero, GvrKeyboardEvent.GVR_KEYBOARD_TEXT_UPDATED);
	}
}
