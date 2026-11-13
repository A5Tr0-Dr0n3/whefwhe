using UnityEngine;

public class ObjFadePushTransitionElement : AccordionTransitionElement
{
	[SerializeField]
	private Transform pushRoot;

	[Tooltip("OPTIONAL: Renderers that will fade 0 to 1 with the transition")]
	[SerializeField]
	private MeshRenderer[] mRenderers;

	[Tooltip("OPTIONAL: ObjectBase on which you can enable / disable grabbability depending on if t < 1")]
	[SerializeField]
	private ObjectBase mObj;

	[Tooltip("OPTIONAL: TransitionElements who get this object's values")]
	[SerializeField]
	private AccordionTransitionElement[] linkedElements = new AccordionTransitionElement[0];

	[SerializeField]
	private GameObject disableObj;

	private MaterialPropertyBlock myBlock;

	private void Awake()
	{
		myBlock = new MaterialPropertyBlock();
	}

	public void Hide()
	{
		ApplyTransitionT(0f, 0f, 0f);
		if (disableObj != null)
		{
			disableObj.SetActive(value: false);
		}
	}

	public void ApplyTransitionT(float t)
	{
		if (disableObj != null)
		{
			disableObj.SetActive(value: true);
		}
		ApplyTransitionT(t, pushRoot.position.z, pushRoot.position.z);
	}

	public override void ApplyTransitionT(float t, float inZ, float outZ)
	{
		if (disableObj != null)
		{
			disableObj.SetActive(value: true);
		}
		Vector3 position = pushRoot.transform.position;
		if (transitionAffectsPosition)
		{
			pushRoot.transform.position = new Vector3(position.x, position.y, Mathf.Lerp(inZ, outZ, t));
		}
		if (t > 0.99f)
		{
			myBlock.Clear();
		}
		else
		{
			myBlock.SetFloat("_Alpha", t);
		}
		MeshRenderer[] array = mRenderers;
		foreach (Renderer renderer in array)
		{
			renderer.SetPropertyBlock(myBlock);
		}
		if (mObj != null)
		{
			mObj.ungrabbable = !t.AlmostEquals(1f);
			mObj.SetCollidersEnabled(t.AlmostEquals(1f));
		}
		for (int j = 0; j < linkedElements.Length; j++)
		{
			linkedElements[j].ApplyTransitionT(t, inZ, outZ);
		}
	}
}
