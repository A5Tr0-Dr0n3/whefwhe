using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TButt;

public class TBCameraSteamVR : TBCameraBase
{
	private PostProcessLayer mLayer;

	public void SetPostProcessingResources(PostProcessResources s, LayerMask postProcessLayer)
	{
		if (mLayer == null)
		{
			mLayer = gameObject.AddComponent<PostProcessLayer>();
			mLayer.volumeLayer = postProcessLayer;
			mLayer.volumeTrigger = transform;
			mLayer.stopNaNPropagation = true;
		}
		mLayer.m_Resources = s;
	}
}
