using System;
using UnityEngine;

public abstract class GvrKeyboardDelegateBase : MonoBehaviour
{
	public abstract event EventHandler KeyboardHidden;

	public abstract event EventHandler KeyboardShown;

	public abstract void OnKeyboardShow();

	public abstract void OnKeyboardHide();

	public abstract void OnKeyboardUpdate(string edit_text);

	public abstract void OnKeyboardEnterPressed(string edit_text);

	public abstract void OnKeyboardError(GvrKeyboardError errorCode);
}
