using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TButt;

public static class TBTracking
{
	public delegate void TrackingEvent(XRNode node, Transform nodeTransform);

	private static Dictionary<XRNode, Transform> _nodes;

	public static TrackingEvent OnNodeConnected;

	public static void Initialize()
	{
		if (_nodes != null)
		{
			_nodes.Clear();
		}
		if (TBSettings.GetControlSettings().supportsHandControllers)
		{
			switch (TBCore.GetActivePlatform())
			{
			case VRPlatform.OculusPC:
			case VRPlatform.SteamVR:
			case VRPlatform.PlayStationVR:
			case VRPlatform.WindowsMR:
				AddTrackedDeviceForNode(XRNode.LeftHand);
				AddTrackedDeviceForNode(XRNode.RightHand);
				break;
			}
		}
		if (TBSettings.GetControlSettings().supports3DOFControllers)
		{
			VRPlatform activePlatform = TBCore.GetActivePlatform();
			if (activePlatform == VRPlatform.OculusMobile || activePlatform == VRPlatform.Daydream)
			{
				TBCameraRig.instance.GetTrackingVolume().gameObject.AddComponent<TB3DOFArmModel>().Initialize();
				AddTrackedDeviceForNode(XRNode.GameController);
			}
			else if (TBCore.UsingEditorMode() && TBSettings.GetControlSettings().emulate3DOFArmModel)
			{
				TBCameraRig.instance.GetTrackingVolume().gameObject.AddComponent<TB3DOFArmModel>().Initialize();
				AddTrackedDeviceForNode(XRNode.GameController);
			}
		}
	}

	public static void AddTrackedDeviceForNode(XRNode node)
	{
		if (_nodes == null)
		{
			_nodes = new Dictionary<XRNode, Transform>();
		}
		GameObject gameObject = new GameObject();
		_nodes.Add(node, gameObject.transform);
		if (node == XRNode.GameController)
		{
			gameObject.AddComponent<TBTrackingNode3DOF>().TrackNode(node);
			return;
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			gameObject.AddComponent<TBTrackingNodeOculus>().TrackNode(node);
			break;
		case VRPlatform.SteamVR:
		case VRPlatform.WindowsMR:
			gameObject.AddComponent<TBTrackingNodeBase>().TrackNode(node);
			break;
		case VRPlatform.PlayStationVR:
			gameObject.AddComponent<TBTrackingNodePSVR>().TrackNode(node);
			break;
		default:
			Debug.LogError("TButt doesn't support tracked device nodes on this platform yet!");
			break;
		}
	}

	public static Transform GetTransformForNode(XRNode node)
	{
		if (_nodes.TryGetValue(node, out var value))
		{
			return value;
		}
		return null;
	}
}
