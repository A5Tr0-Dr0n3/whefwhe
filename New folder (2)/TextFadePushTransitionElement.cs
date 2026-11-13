using TMPro;
using UnityEngine;

public class TextFadePushTransitionElement : AccordionTransitionElement
{
	[SerializeField]
	private TextMeshPro mText;

	public override void ApplyTransitionT(float t, float inZ, float outZ)
	{
		mText.color = new Color(0f, 0f, 0f, t) + mText.color * new Color(1f, 1f, 1f, 0f);
		if (transitionAffectsPosition)
		{
			mText.transform.position = new Vector3(mText.transform.position.x, mText.transform.position.y, Mathf.Lerp(inZ, outZ, t));
		}
	}
}
