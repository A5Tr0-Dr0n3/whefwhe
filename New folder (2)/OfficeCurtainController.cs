using UnityEngine;

public class OfficeCurtainController : MonoBehaviour
{
	[SerializeField]
	private float curtainSpeed = 1f;

	[SerializeField]
	private Collider teleportBlocker;

	private float targetCurtainT;

	private float curtainT;

	private bool opened;

	private float curtainVel;

	private Animator mAnimator;

	private void Start()
	{
		mAnimator = GetComponent<Animator>();
	}

	private void Update()
	{
		curtainT = Mathf.SmoothDamp(curtainT, targetCurtainT, ref curtainVel, 1f / curtainSpeed);
		mAnimator.Play("OfficeDoorLift", -1, curtainT);
		teleportBlocker.enabled = curtainT < 0.85f;
	}

	public void SetTargetCurtainOpenness(float newValue)
	{
		if (!opened)
		{
			targetCurtainT = Mathf.Clamp01(newValue);
		}
	}

	public void Open()
	{
		targetCurtainT = 1f;
		opened = true;
	}
}
