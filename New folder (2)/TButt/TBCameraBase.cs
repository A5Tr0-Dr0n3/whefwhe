using System;
using UnityEngine;
using UnityEngine.XR;

namespace TButt;

public class TBCameraBase : MonoBehaviour
{
	protected Camera _camera;

	protected void Awake()
	{
		_camera = GetComponent<Camera>();
		UpdateEverything();
	}

	public virtual void Initialize()
	{
		SetTrackingOrigin(TBSettings.GetCameraSettings().trackingOrigin);
	}

	protected virtual void SetTrackingOrigin(TBSettings.TBTrackingOrigin origin)
	{
		if (!HeadsetHasPositionTracking() && origin == TBSettings.TBTrackingOrigin.Floor)
		{
			TBCameraRig.instance.GetTrackingVolume().localPosition = new Vector3(0f, TBSettings.GetCameraSettings().uncalibratedFloorHeight, 0f);
		}
	}

	protected virtual bool HeadsetHasPositionTracking()
	{
		return true;
	}

	public virtual void Recenter()
	{
		InputTracking.Recenter();
	}

	protected void OnEnable()
	{
		TBCameraRig.Events.OnClearFlagsChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnClearFlagsChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateClearFlags));
		TBCameraRig.Events.OnBackgroundColorChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnBackgroundColorChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateBackgroundColor));
		TBCameraRig.Events.OnCullingMaskChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnCullingMaskChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateCullingMask));
		TBCameraRig.Events.OnOcclusionCullingChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnOcclusionCullingChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateOcclusionCulling));
		TBCameraRig.Events.OnFarClipPlaneChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnFarClipPlaneChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateFarClip));
		TBCameraRig.Events.OnNearClipPlaneChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnNearClipPlaneChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateNearClip));
		TBCameraRig.Events.OnSortModeChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnSortModeChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateSortMode));
		TBCameraRig.Events.OnStereoConvergenceChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnStereoConvergenceChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateStereoConvergence));
		TBCameraRig.Events.OnStereoSeparationChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnStereoSeparationChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateStereoSeparation));
		TBCameraRig.Events.OnDepthChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Combine(TBCameraRig.Events.OnDepthChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateDepth));
	}

	protected void OnDisable()
	{
		TBCameraRig.Events.OnClearFlagsChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnClearFlagsChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateClearFlags));
		TBCameraRig.Events.OnBackgroundColorChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnBackgroundColorChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateBackgroundColor));
		TBCameraRig.Events.OnCullingMaskChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnCullingMaskChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateCullingMask));
		TBCameraRig.Events.OnOcclusionCullingChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnOcclusionCullingChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateOcclusionCulling));
		TBCameraRig.Events.OnFarClipPlaneChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnFarClipPlaneChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateFarClip));
		TBCameraRig.Events.OnNearClipPlaneChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnNearClipPlaneChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateNearClip));
		TBCameraRig.Events.OnSortModeChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnSortModeChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateSortMode));
		TBCameraRig.Events.OnStereoConvergenceChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnStereoConvergenceChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateStereoConvergence));
		TBCameraRig.Events.OnStereoSeparationChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnStereoSeparationChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateStereoSeparation));
		TBCameraRig.Events.OnDepthChanged = (TBCameraRig.Events.CameraSettingChangeEvent)Delegate.Remove(TBCameraRig.Events.OnDepthChanged, new TBCameraRig.Events.CameraSettingChangeEvent(UpdateDepth));
	}

	protected void UpdateEverything()
	{
		UpdateClearFlags();
		UpdateBackgroundColor();
		UpdateCullingMask();
		UpdateOcclusionCulling();
		UpdateFarClip();
		UpdateNearClip();
		UpdateSortMode();
		UpdateStereoConvergence();
		UpdateStereoSeparation();
		UpdateDepth();
	}

	protected void UpdateClearFlags()
	{
		_camera.clearFlags = TBCameraRig.instance.clearFlags;
	}

	protected void UpdateBackgroundColor()
	{
		_camera.backgroundColor = TBCameraRig.instance.backgroundColor;
	}

	protected void UpdateCullingMask()
	{
		_camera.cullingMask = TBCameraRig.instance.cullingMask;
	}

	protected void UpdateOcclusionCulling()
	{
		_camera.useOcclusionCulling = TBCameraRig.instance.useOcclusionCulling;
	}

	protected void UpdateFarClip()
	{
		_camera.farClipPlane = TBCameraRig.instance.farClipPlane;
	}

	protected void UpdateNearClip()
	{
		_camera.nearClipPlane = TBCameraRig.instance.nearClipPlane;
	}

	protected void UpdateSortMode()
	{
		_camera.opaqueSortMode = TBCameraRig.instance.sortMode;
	}

	protected void UpdateStereoConvergence()
	{
		_camera.stereoConvergence = TBCameraRig.instance.stereoConvergence;
	}

	protected void UpdateStereoSeparation()
	{
		_camera.stereoSeparation = TBCameraRig.instance.stereoSeparation;
	}

	protected void UpdateDepth()
	{
		_camera.depth = TBCameraRig.instance.depth;
	}
}
