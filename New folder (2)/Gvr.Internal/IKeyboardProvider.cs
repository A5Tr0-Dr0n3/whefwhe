using UnityEngine;

namespace Gvr.Internal;

internal interface IKeyboardProvider
{
	string EditorText { get; set; }

	void OnPause();

	void OnResume();

	void ReadState(KeyboardState outState);

	bool Create(GvrKeyboard.KeyboardCallback keyboardEvent);

	void UpdateData();

	void Render(int eye, Matrix4x4 modelview, Matrix4x4 projection, Rect viewport);

	void Hide();

	void Show(Matrix4x4 controllerMatrix, bool useRecommended, float distance, Matrix4x4 model);

	void SetInputMode(GvrKeyboardInputMode mode);
}
