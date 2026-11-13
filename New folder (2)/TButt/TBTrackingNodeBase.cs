using System;
using UnityEngine;
using UnityEngine.XR;

namespace TButt;

public class TBTrackingNodeBase : MonoBehaviour
{
	[HideInInspector]
	public new Transform transform;

	protected bool _subscribedToUpdate;

	protected XRNode _node = XRNode.CenterEye;

	public virtual void TrackNode(XRNode node)
	{
		transform = GetComponent<Transform>();
		transform.MakeZeroedChildOf(TBCameraRig.instance.GetTrackingVolume());
		_node = node;
		if (TBTracking.OnNodeConnected != null)
		{
			TBTracking.OnNodeConnected(node, transform);
		}
	}

	protected virtual void OnEnable()
	{
		SubscribeToUpdate(on: true);
	}

	protected virtual void OnDisable()
	{
		SubscribeToUpdate(on: false);
	}

	protected virtual void OnUpdatePoses()
	{
		transform.localPosition = InputTracking.GetLocalPosition(_node);
		transform.localRotation = InputTracking.GetLocalRotation(_node);
	}

	protected void SubscribeToUpdate(bool on)
	{
		if (on != _subscribedToUpdate)
		{
			if (on)
			{
				TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(OnUpdatePoses));
			}
			else
			{
				TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Remove(TBCore.OnUpdate, new TBCore.TBCoreEvent(OnUpdatePoses));
			}
			_subscribedToUpdate = on;
		}
	}
}
