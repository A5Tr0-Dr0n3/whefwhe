using UnityEngine;

public class ObjScaleTransitionElement : MonoBehaviour
{
	[SerializeField]
	private ObjectBase mObj;

	[SerializeField]
	private Transform scaleLayer;

	public void Hide()
	{
		ApplyScaleT(0f);
	}

	public void ApplyScaleT(float t)
	{
		mObj.ungrabbable = t != 1f;
		mObj.SetCollidersEnabled(t == 1f);
		scaleLayer.transform.localScale = Vector3.one * t;
	}
}
