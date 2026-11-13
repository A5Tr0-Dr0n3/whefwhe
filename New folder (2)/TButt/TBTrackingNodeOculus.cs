using System;
using UnityEngine.XR;

namespace TButt;

public class TBTrackingNodeOculus : TBTrackingNodeBase
{
	protected OVRInput.Controller _controller;

	protected override void OnEnable()
	{
		base.OnEnable();
		TBCore.Events.OnSystemMenu = (TBCore.Events.TBCoreSystemEvent)Delegate.Combine(TBCore.Events.OnSystemMenu, new TBCore.Events.TBCoreSystemEvent(UpdateHandedness));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		TBCore.Events.OnSystemMenu = (TBCore.Events.TBCoreSystemEvent)Delegate.Remove(TBCore.Events.OnSystemMenu, new TBCore.Events.TBCoreSystemEvent(UpdateHandedness));
	}

	private void UpdateHandedness(bool needed)
	{
		if (!needed || _node != XRNode.GameController)
		{
			return;
		}
		switch (TBInput.Get3DOFHandedness())
		{
		case TBInput.Mobile3DOFHandedness.Left:
			if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
			{
				_controller = OVRInput.Controller.LTouch;
			}
			else
			{
				_controller = OVRInput.Controller.LTrackedRemote;
			}
			break;
		case TBInput.Mobile3DOFHandedness.Right:
			if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
			{
				_controller = OVRInput.Controller.RTouch;
			}
			else
			{
				_controller = OVRInput.Controller.RTrackedRemote;
			}
			break;
		}
	}

	public override void TrackNode(XRNode node)
	{
		switch (node)
		{
		case XRNode.LeftHand:
			_controller = OVRInput.Controller.LTouch;
			break;
		case XRNode.RightHand:
			_controller = OVRInput.Controller.RTouch;
			break;
		case XRNode.GameController:
			UpdateHandedness(needed: true);
			break;
		}
		base.TrackNode(node);
	}

	protected override void OnUpdatePoses()
	{
		transform.localPosition = OVRInput.GetLocalControllerPosition(_controller);
		transform.localRotation = OVRInput.GetLocalControllerRotation(_controller);
	}
}
