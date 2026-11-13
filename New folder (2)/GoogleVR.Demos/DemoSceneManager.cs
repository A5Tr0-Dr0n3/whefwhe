using UnityEngine;

namespace GoogleVR.Demos;

public class DemoSceneManager : MonoBehaviour
{
	private void Start()
	{
		Input.backButtonLeavesApp = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
