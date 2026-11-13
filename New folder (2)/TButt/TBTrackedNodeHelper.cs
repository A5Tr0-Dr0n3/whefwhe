using System;
using UnityEngine;
using UnityEngine.XR;

namespace TButt;

public class TBTrackedNodeHelper : MonoBehaviour
{
	public XRNode nodeToAttachWith;

	private void OnEnable()
	{
		TBTracking.OnNodeConnected = (TBTracking.TrackingEvent)Delegate.Combine(TBTracking.OnNodeConnected, new TBTracking.TrackingEvent(AttachToNode));
		Transform transformForNode = TBTracking.GetTransformForNode(nodeToAttachWith);
		if (transformForNode != null)
		{
			AttachToNode(nodeToAttachWith, transformForNode);
		}
	}

	private void OnDisable()
	{
		TBTracking.OnNodeConnected = (TBTracking.TrackingEvent)Delegate.Remove(TBTracking.OnNodeConnected, new TBTracking.TrackingEvent(AttachToNode));
	}

	private void AttachToNode(XRNode node, Transform t)
	{
		if (node == nodeToAttachWith)
		{
			transform.MakeZeroedChildOf(t);
			enabled = false;
		}
	}
}
