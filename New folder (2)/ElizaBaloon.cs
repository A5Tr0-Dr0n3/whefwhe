using UnityEngine;

public class ElizaBaloon : MonoBehaviour
{
	private Animator mAnimator;

	private float currentTime;

	private void Awake()
	{
		mAnimator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		mAnimator.StopPlayback();
		mAnimator.Play("Rise", 0, currentTime);
	}

	private void Update()
	{
		if ((double)currentTime <= 0.99)
		{
			currentTime = mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		}
	}
}
