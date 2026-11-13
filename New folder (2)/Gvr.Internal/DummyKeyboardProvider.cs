using UnityEngine;

namespace Gvr.Internal;

internal class DummyKeyboardProvider : IKeyboardProvider
{
	private KeyboardState dummyState = new KeyboardState();

	public string EditorText { get; set; }

	internal DummyKeyboardProvider()
	{
	}

	public void ReadState(KeyboardState outState)
	{
		outState.CopyFrom(dummyState);
	}

	public void OnPause()
	{
	}

	public void OnResume()
	{
	}

	public void UpdateData()
	{
	}

	public void Render(int eye, Matrix4x4 modelview, Matrix4x4 projection, Rect viewport)
	{
	}

	public void Hide()
	{
	}

	public void Show(Matrix4x4 controllerMatrix, bool useRecommended, float distance, Matrix4x4 model)
	{
	}

	public bool Create(GvrKeyboard.KeyboardCallback keyboardEvent)
	{
		return true;
	}

	public void SetInputMode(GvrKeyboardInputMode mode)
	{
	}
}
