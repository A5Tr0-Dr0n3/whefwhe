using UnityEngine;

public class AccordionTransitionElement : MonoBehaviour
{
	[SerializeField]
	protected bool transitionAffectsPosition = true;

	public virtual void ApplyTransitionT(float t, float inZ, float outZ)
	{
	}
}
