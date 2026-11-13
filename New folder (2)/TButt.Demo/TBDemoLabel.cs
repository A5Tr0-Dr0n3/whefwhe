using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace TButt.Demo;

public class TBDemoLabel : MonoBehaviour
{
	public enum DemoLabel
	{
		ActiveSDK,
		ActiveHMD,
		Renderscale
	}

	public DemoLabel label;

	private Text text;

	private void Start()
	{
		text = GetComponent<Text>();
	}

	private void Update()
	{
		text.text = GetLabel(label);
	}

	private string GetLabel(DemoLabel label)
	{
		return label switch
		{
			DemoLabel.ActiveSDK => TBCore.GetActivePlatform().ToString(), 
			DemoLabel.ActiveHMD => XRDevice.model, 
			DemoLabel.Renderscale => XRSettings.eyeTextureResolutionScale.ToString(), 
			_ => "Null", 
		};
	}
}
