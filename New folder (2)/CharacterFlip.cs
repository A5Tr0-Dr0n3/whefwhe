using System.Collections;
using Edelweiss.Coroutine;
using GVR.Gfx;
using TButt;
using UnityEngine;

public class CharacterFlip : MonoBehaviour
{
	public delegate void OnFlipEventHandler();

	private static bool flipEnabled = true;

	[CurveRange(0f, 1f, -1f, 1f)]
	public AnimationCurve fisheye;

	[Range(0f, 1f)]
	public float triggerTime = 0.3f;

	public float totalTime = 0.75f;

	public float scalar = 1f;

	public AudioEvent sfx;

	[Range(0f, 1f)]
	public float volume = 0.5f;

	private SafeCoroutine flipThread;

	private float timerExpire = 0.5f;

	private float timer;

	private int buttonCount;

	private bool counting;

	private bool cursorState;

	public static event OnFlipEventHandler OnFlipEvent;

	public static void SetCanFlip(bool newValue)
	{
		flipEnabled = newValue;
	}

	private void Update()
	{
		if (!flipEnabled)
		{
			return;
		}
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			if (ToolManager.AnyControllerGetButtonDown(TBInput.Button.Joystick))
			{
				Flip();
			}
			return;
		}
		if (TBInput.GetButtonDown(TBInput.Button.Action2, TBInput.Controller.Active))
		{
			buttonCount++;
			counting = true;
		}
		if (counting)
		{
			timer += Time.deltaTime;
			if (buttonCount >= 3)
			{
				Flip();
				timer = 0f;
				buttonCount = 0;
				counting = false;
			}
			if (timer > timerExpire)
			{
				timer = 0f;
				buttonCount = 0;
				counting = false;
			}
		}
	}

	private void Flip()
	{
		if (flipThread == null || !flipThread.IsRunning)
		{
			if (OnFlipEvent != null)
			{
				OnFlipEvent();
			}
			flipThread = GlobalCoroutineContainer.AddThread(flipCoroutine(), this);
		}
	}

	private IEnumerator flipCoroutine()
	{
		Vector3 playerStartOffset = EagerMaster.instance.mainCamera.transform.localPosition;
		sfx.Play();
		cursorState = ToolManager.AllCursorReticles.HideCursor;
		ToolManager.AllCursorReticles.HideCursor = true;
		bool flipped = false;
		float elaplsedTime = 0f;
		while (elaplsedTime < totalTime)
		{
			EnvironmentProperties.fishEyeStrength = scalar * fisheye.Evaluate(elaplsedTime / totalTime);
			if (!flipped && elaplsedTime / totalTime >= triggerTime)
			{
				transform.Rotate(Vector3.up, 180f);
				float x = (EagerMaster.instance.player.transform.localRotation * playerStartOffset).x * 2f * PlayerScaler.instance.currentScale.x;
				float z = (EagerMaster.instance.player.transform.localRotation * playerStartOffset).z * 2f * PlayerScaler.instance.currentScale.x;
				transform.localPosition -= new Vector3(x, 0f, z);
				flipped = true;
			}
			elaplsedTime += Time.deltaTime;
			ToolManager.OnTeleport();
			yield return null;
		}
		EnvironmentProperties.fishEyeStrength = 0f;
		ToolManager.AllCursorReticles.HideCursor = cursorState;
		yield return null;
	}
}
