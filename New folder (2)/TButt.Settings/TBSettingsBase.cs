using UnityEngine;
using UnityEngine.XR;

namespace TButt.Settings;

public class TBSettingsBase : MonoBehaviour
{
	public virtual void Initialize()
	{
		SetRenderscale(GetConfiguredRenderscale());
		SetRefreshRate();
	}

	public virtual void SetRenderscale(float newScale)
	{
		TBCore.GetActivePlatform();
		if (XRSettings.eyeTextureResolutionScale != newScale)
		{
			XRSettings.eyeTextureResolutionScale = newScale;
		}
	}

	public virtual void SetRefreshRate()
	{
	}

	public virtual float GetRefreshRate()
	{
		return XRDevice.refreshRate;
	}

	public virtual TBSettings.TBQualityLevel GetConfiguredQualityLevel()
	{
		return TBSettings.GetDisplaySettings().minQualityLevel;
	}

	public virtual float GetConfiguredRenderscale()
	{
		return TBSettings.GetDisplaySettings().minRenderScale;
	}
}
