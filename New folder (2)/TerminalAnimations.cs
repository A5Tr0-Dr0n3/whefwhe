using System.Collections;
using UnityEngine;

public class TerminalAnimations : MonoBehaviour
{
	[SerializeField]
	private Transform topPiston;

	[SerializeField]
	private Transform leftWire;

	[SerializeField]
	private Transform rightWire;

	[SerializeField]
	private Renderer distortHeadsetX;

	[Header("Headset Distortion")]
	[SerializeField]
	private float distortionValue = 0.5f;

	[Header("Piston Parameters")]
	[SerializeField]
	private float pistonMovementDistance = 1.5f;

	[SerializeField]
	private float wireRotationAngle = 15f;

	private Vector4 noDistortion = Vector4.zero;

	private void Start()
	{
	}

	public void AnimatePrint()
	{
	}

	public void AnimateSwitch()
	{
		DistortHeadset();
		animatePiston();
		GlobalCoroutineContainer.AddThread(StopHeadsetOnDelay(0.3f), this);
	}

	private void animatePiston()
	{
		GlobalCoroutineContainer.AddThread(PistonMovement(), this);
	}

	private IEnumerator PistonMovement()
	{
		Vector3 bottomPos = topPiston.position;
		Vector3 topPos = topPiston.position;
		Quaternion startLeft = leftWire.rotation;
		Quaternion startRight = rightWire.rotation;
		Quaternion targetLeft = Quaternion.AngleAxis(wireRotationAngle, Vector3.forward);
		Quaternion targetRight = Quaternion.AngleAxis(0f - wireRotationAngle, Vector3.forward);
		topPos.y += pistonMovementDistance;
		float time = Time.time;
		while (Time.time - time < 0.15f)
		{
			float lerp = Mathf.Clamp01((Time.time - time) / 0.15f);
			topPiston.position = Vector3.Slerp(topPiston.position, topPos, lerp);
			leftWire.rotation = Quaternion.Slerp(leftWire.rotation, targetLeft, lerp);
			rightWire.rotation = Quaternion.Slerp(rightWire.rotation, targetRight, lerp);
			yield return null;
		}
		time = Time.time;
		while (Time.time - time < 0.15f)
		{
			float lerp2 = Mathf.Clamp01((Time.time - time) / 0.15f);
			topPiston.position = Vector3.Slerp(topPiston.position, bottomPos, lerp2);
			leftWire.rotation = Quaternion.Slerp(leftWire.rotation, startLeft, lerp2);
			rightWire.rotation = Quaternion.Slerp(rightWire.rotation, startRight, lerp2);
			yield return null;
		}
	}

	public void DistortHeadset()
	{
		Vector4 value = new Vector4(distortionValue, 0f, 0f, 1f);
		distortHeadsetX.sharedMaterial.SetVector("_Distortion", value);
		SafeDebug.Log("Distorting Headsets");
	}

	public void StopHeadsetDistortion()
	{
		distortHeadsetX.sharedMaterial.SetVector("_Distortion", noDistortion);
	}

	private IEnumerator StopHeadsetOnDelay(float t)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(t, this);
		StopHeadsetDistortion();
	}
}
