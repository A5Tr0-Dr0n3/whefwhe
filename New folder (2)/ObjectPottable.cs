using System.Collections;
using UnityEngine;

public class ObjectPottable : ObjectBase
{
	[HideInInspector]
	public bool isPotted;

	[HideInInspector]
	public bool potHasBroken;

	[HideInInspector]
	public Pot myPot;

	public Vector3 potOffset = Vector3.zero;

	public bool isPottable = true;

	[HideInInspector]
	public bool canBeSmashed;

	[HideInInspector]
	public SubtitleRenderer shatterSubtitle;

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		rb.isKinematic = false;
		if (!isPotted)
		{
		}
	}

	public void OnPotted()
	{
		isPotted = true;
		Satisfaction();
	}

	public void OnPottedBreak()
	{
		isPotted = false;
		Satisfaction();
		potHasBroken = true;
		if ((bool)shatterSubtitle)
		{
			shatterSubtitle.SetCharacter(transform);
			shatterSubtitle.gameObject.SetActive(value: true);
			GlobalCoroutineContainer.AddThread(DisableSubtitleOnDelay(0.7f), this);
		}
	}

	private IEnumerator DisableSubtitleOnDelay(float s)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(s, this);
		shatterSubtitle.gameObject.SetActive(value: false);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position + potOffset, 0.01f);
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (ToolManager.IsTeleporting)
		{
			return;
		}
		if (isPotted && (bool)myPot && collision.relativeVelocity.magnitude > myPot.smashVelocityMin)
		{
			if (myPot.dontBreakOnColliders.Count > 0)
			{
				if (!myPot.dontBreakOnColliders.Contains(collision.gameObject))
				{
					myPot.BreakPot();
				}
			}
			else
			{
				myPot.BreakPot();
			}
		}
		if (canBeSmashed && collision.relativeVelocity.magnitude > smashVelocityMin && !isPotted)
		{
			isPottable = false;
			potHasBroken = true;
		}
	}
}
