using GoogleVR.Demos;
using UnityEngine;

namespace GoogleVR.GVRDemo;

public class GVRDemoManager : MonoBehaviour
{
	public GameObject m_launchVrHomeButton;

	public DemoInputManager m_demoInputManager;

	private void Start()
	{
		if (!(m_launchVrHomeButton == null))
		{
			m_launchVrHomeButton.SetActive(value: false);
		}
	}

	public void LaunchVrHome()
	{
	}
}
