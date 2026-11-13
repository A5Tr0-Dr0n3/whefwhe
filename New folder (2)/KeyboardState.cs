using UnityEngine;

public class KeyboardState
{
	internal string editorText = string.Empty;

	internal GvrKeyboardInputMode mode;

	internal bool isValid;

	internal bool isReady;

	internal Matrix4x4 worldMatrix;

	public void CopyFrom(KeyboardState other)
	{
		editorText = other.editorText;
		mode = other.mode;
		isValid = other.isValid;
		isReady = other.isReady;
		worldMatrix = other.worldMatrix;
	}
}
