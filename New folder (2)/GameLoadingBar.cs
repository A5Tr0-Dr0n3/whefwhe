using UnityEngine;

public class GameLoadingBar : MonoBehaviour
{
	[SerializeField]
	private Transform loadProgressScalingLayer;

	[SerializeField]
	private Transform transitionOutScalingLayer;

	[SerializeField]
	private AnimationCurve fakeLoadSpeedCurve = new AnimationCurve(new Keyframe(0f, 0.15f), new Keyframe(1f, 0.01f));

	[SerializeField]
	private float topOffSpeed = 0.1f;

	[SerializeField]
	private float fakeLoadMax = 0.95f;

	[SerializeField]
	private float maxFakeLoadLead = 0.2f;

	[SerializeField]
	private float smoothingTime = 0.75f;

	private float loadProgSmoothingVel;

	private float fakeLoadProg;

	private float dampedLoadProg;

	public bool TickLoadingSequence(float rawLoadProgress, out float smoothProgressValue)
	{
		loadProgressScalingLayer.gameObject.SetActive(value: true);
		loadProgressScalingLayer.localScale = new Vector3(0f, loadProgressScalingLayer.localScale.y, loadProgressScalingLayer.localScale.z);
		float num = fakeLoadSpeedCurve.Evaluate(Mathf.Clamp01((fakeLoadProg - rawLoadProgress) / maxFakeLoadLead)) * Time.deltaTime;
		fakeLoadProg = Mathf.Clamp(fakeLoadProg + num, rawLoadProgress, fakeLoadMax);
		float target = Mathf.Max(fakeLoadProg, rawLoadProgress);
		dampedLoadProg = Mathf.SmoothDamp(dampedLoadProg, target, ref loadProgSmoothingVel, smoothingTime);
		if (rawLoadProgress >= 0.975f)
		{
			dampedLoadProg += topOffSpeed * Time.deltaTime;
		}
		loadProgressScalingLayer.localScale = new Vector3(dampedLoadProg, loadProgressScalingLayer.localScale.y, loadProgressScalingLayer.localScale.z);
		smoothProgressValue = dampedLoadProg;
		return dampedLoadProg >= 1f;
	}

	public void ScaleOutY(float newScaleMultiplier)
	{
		transitionOutScalingLayer.localScale = new Vector3(transitionOutScalingLayer.localScale.x, newScaleMultiplier, transitionOutScalingLayer.localScale.z);
	}
}
