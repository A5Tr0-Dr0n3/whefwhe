using UnityEngine;

namespace GoogleVR.VideoDemo;

public class AppButtonInput : MonoBehaviour
{
	public ButtonEvent OnAppUp;

	public ButtonEvent OnAppDown;

	private void Update()
	{
		if (GvrControllerInput.AppButtonUp)
		{
			OnAppUp.Invoke();
		}
		if (GvrControllerInput.AppButtonDown)
		{
			OnAppDown.Invoke();
		}
	}
}
