using UnityEngine;
using UnityEngine.XR;

namespace TButt.Settings;

public class TBSettingsDaydream : TBSettingsBase
{
	public override float GetConfiguredRenderscale()
	{
		float num = 1f;
		if (SystemInfo.deviceModel == "Pixel" || SystemInfo.deviceModel == "Google Pixel")
		{
			num = 0.75f;
		}
		else if (SystemInfo.deviceModel == "Google Pixel XL")
		{
			num = 0.75f;
		}
		else
		{
			float num2 = (float)XRSettings.eyeTextureHeight / XRSettings.eyeTextureResolutionScale;
			if (!float.IsNaN(num2))
			{
				num = Mathf.Clamp(1430f / num2, 0.7f, 1f);
			}
		}
		SafeDebug.Log("Your device is a " + SystemInfo.deviceModel + " , renderscale: " + num);
		return num;
	}
}
