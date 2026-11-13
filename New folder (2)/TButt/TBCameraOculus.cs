using TButt.Settings;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TButt;

public class TBCameraOculus : TBCameraBase
{
	private PostProcessLayer mLayer;

	public override void Initialize()
	{
		OculusMobilePlatform gearVRVersion = TBSettingsOculus.GetGearVRVersion();
		gameObject.AddComponent<OVRManager>();
		gameObject.AddComponent<OVRPlatformMenu>();
		gameObject.AddComponent<OculusFocusManager>();
		if (mLayer == null)
		{
			mLayer = gameObject.AddComponent<PostProcessLayer>();
			mLayer.volumeTrigger = transform;
			mLayer.stopNaNPropagation = true;
		}
		OVRManager.instance.enableAdaptiveResolution = TBSettings.GetDisplaySettings().useAdaptiveResolution;
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			OVRManager.instance.minRenderScale = TBSettings.GetDisplaySettings().renderScales[(int)gearVRVersion];
			OVRManager.instance.maxRenderScale = TBSettings.GetDisplaySettings().renderScales[(int)gearVRVersion];
			OVRManager.tiledMultiResLevel = (OVRManager.TiledMultiResLevel)TBSettings.GetDisplaySettings().foveatedLevels[(int)gearVRVersion];
			OVRManager.cpuLevel = TBSettings.GetDisplaySettings().cpuLevels[(int)gearVRVersion];
			OVRManager.gpuLevel = TBSettings.GetDisplaySettings().gpuLevels[(int)gearVRVersion];
			QualitySettings.antiAliasing = (int)TBSettings.GetDisplaySettings().antiAliasingLevels[(int)gearVRVersion];
		}
		else
		{
			OVRManager.instance.minRenderScale = TBSettings.GetDisplaySettings().minRenderScale;
			OVRManager.instance.maxRenderScale = TBSettings.GetDisplaySettings().maxRenderScale;
		}
		OVRManager.instance.useRecommendedMSAALevel = false;
		base.Initialize();
	}

	protected override void SetTrackingOrigin(TBSettings.TBTrackingOrigin origin)
	{
		base.SetTrackingOrigin(origin);
		switch (origin)
		{
		case TBSettings.TBTrackingOrigin.Eye:
			OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
			break;
		case TBSettings.TBTrackingOrigin.Floor:
			OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
			break;
		}
	}

	protected override bool HeadsetHasPositionTracking()
	{
		return OVRPlugin.positionSupported;
	}

	public void SetPostProcessingResources(PostProcessResources s, LayerMask postProcessLayer)
	{
		if (mLayer == null)
		{
			mLayer = gameObject.AddComponent<PostProcessLayer>();
			mLayer.volumeTrigger = transform;
			mLayer.volumeLayer = postProcessLayer;
			mLayer.stopNaNPropagation = true;
		}
		mLayer.m_Resources = s;
	}
}
