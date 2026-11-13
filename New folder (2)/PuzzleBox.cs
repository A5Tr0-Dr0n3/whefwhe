using System;
using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class PuzzleBox : MonoBehaviour
{
	[SerializeField]
	private OfficeTerminal[] horizontalTerminals;

	[SerializeField]
	private OfficeTerminal[] verticalTerminals;

	[SerializeField]
	private ObjectBase headsetButton;

	[SerializeField]
	private PuzzleGrid[] column;

	private Vector2 lastGridPos = Vector2.up;

	[SerializeField]
	private GameObject headsetClone;

	[SerializeField]
	private GvrAudioSource dispenseHeadsetSound;

	[SerializeField]
	private TerminalAnimations terminal;

	[SerializeField]
	private Transform glowCard;

	[SerializeField]
	private GameObject bigGlowCard;

	private VVRConsumable[] consumableComponents;

	private GameObject activeHeadset;

	private GameObject oldHeadset;

	private GridSlot activeGridSlot;

	[SerializeField]
	private GvrAudioSource fillSound;

	[SerializeField]
	private GvrAudioSource buzz;

	private bool skipStartupSequence;

	private int framesSinceStart;

	private SafeCoroutine startThread;

	private SafeCoroutine printHeadsetCoroutine;

	private SafeCoroutine flushHeadsetCoroutine;

	private void Start()
	{
		consumableComponents = GetComponentsInChildren<VVRConsumable>();
		skipStartupSequence = PersistentDataManager.GetHasSetupStamper();
	}

	private void Update()
	{
		if (framesSinceStart < 3)
		{
			framesSinceStart++;
		}
		if (IsFull())
		{
			if (!skipStartupSequence)
			{
				if (startThread == null)
				{
					startThread = GlobalCoroutineContainer.AddThread(ActivateSequence(), this);
					PersistentDataManager.OverwriteHasSetupStamper(s: true);
				}
				return;
			}
			headsetButton.ungrabbable = false;
			if (activeGridSlot != null)
			{
				activeGridSlot.SetSlotState(GridSlotState.active);
			}
			Vector2 vector = calculateGridPosition();
			if (vector != lastGridPos)
			{
				lastGridPos = vector;
				updateGridPosition(vector);
				if (flushHeadsetCoroutine == null && (bool)activeHeadset)
				{
					flushHeadsetCoroutine = GlobalCoroutineContainer.AddThread(FlushHeadsetCoroutine(activeHeadset), this);
					activeHeadset = null;
				}
			}
			if (activeHeadset != null && !ToolManager.PlayerGrabbingObjectBase(activeHeadset.GetComponent<ObjectBase>()) && printHeadsetCoroutine == null && !EagerHeadsetManager.instance.isAddingHeadset)
			{
				activeHeadset.GetComponent<Rigidbody>().isKinematic = true;
				activeHeadset.transform.position = Vector3.Lerp(activeHeadset.transform.position, transform.position + Vector3.up * 1.1f, 1f - Mathf.Pow(0.2f, Time.deltaTime));
			}
			else if (activeHeadset != null)
			{
				activeHeadset.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		else if (framesSinceStart >= 3)
		{
			skipStartupSequence = false;
			headsetButton.ungrabbable = true;
			if (startThread != null && !startThread.IsPaused && !startThread.IsStopped)
			{
				startThread.Stop();
				startThread = null;
				fillSound.Stop();
				terminal.StopHeadsetDistortion();
			}
			if (activeGridSlot != null)
			{
				activeGridSlot.SetSlotState(GridSlotState.inactive);
				glowCard.gameObject.SetActive(value: false);
				activeGridSlot = null;
			}
			if ((bool)activeHeadset && flushHeadsetCoroutine == null)
			{
				flushHeadsetCoroutine = GlobalCoroutineContainer.AddThread(FlushHeadsetCoroutine(activeHeadset), this);
				activeHeadset = null;
			}
		}
	}

	public void PrintHeadset()
	{
		if (activeGridSlot != null && printHeadsetCoroutine == null)
		{
			printHeadsetCoroutine = GlobalCoroutineContainer.AddThread(PrintHeadsetCoroutine(), this);
		}
	}

	private IEnumerator FlushHeadsetCoroutine(GameObject headsetToFlush)
	{
		if (!headsetToFlush)
		{
			yield return null;
			yield break;
		}
		float lerp = 0f;
		float timer = 0f;
		float moveDuration = 0.5f;
		headsetToFlush.transform.parent = transform;
		headsetToFlush.GetComponent<Rigidbody>().isKinematic = true;
		headsetToFlush.SetActive(value: true);
		while (lerp < 1f)
		{
			timer += Time.deltaTime;
			lerp = Mathf.Clamp01(timer / moveDuration);
			lerp *= lerp * lerp;
			headsetToFlush.transform.localPosition = Vector3.Lerp(Vector3.up * 1.1f, Vector3.zero, lerp);
			yield return null;
		}
		headsetToFlush.SetActive(value: false);
		flushHeadsetCoroutine = null;
	}

	private IEnumerator PrintHeadsetCoroutine()
	{
		dispenseHeadsetSound.Play();
		oldHeadset = activeHeadset;
		activeHeadset = activeGridSlot.mHeadset;
		activeHeadset.SetActive(value: false);
		activeHeadset.GetComponent<Rigidbody>().useGravity = false;
		activeHeadset.transform.parent = transform;
		if ((bool)oldHeadset && flushHeadsetCoroutine == null)
		{
			flushHeadsetCoroutine = GlobalCoroutineContainer.AddThread(FlushHeadsetCoroutine(activeHeadset), this);
			yield return flushHeadsetCoroutine;
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		float lerp = 0f;
		float timer = 0f;
		float moveDuration = 0.5f;
		while (lerp < 1f)
		{
			timer += Time.deltaTime;
			lerp = Mathf.Clamp01(timer / moveDuration);
			lerp *= lerp * lerp;
			headsetClone.transform.localPosition = Vector3.Lerp(Vector3.up * 6f, Vector3.up * 4f, lerp);
			yield return null;
		}
		if (activeHeadset != null)
		{
			activeHeadset.GetComponent<Rigidbody>().isKinematic = true;
			activeHeadset.SetActive(value: true);
		}
		lerp = 0f;
		timer = 0f;
		moveDuration = 0.5f;
		Quaternion startRotation = UnityEngine.Random.rotationUniform;
		while (lerp < 1f)
		{
			timer += Time.deltaTime;
			lerp = Mathf.Clamp01(timer / moveDuration);
			lerp = Mathf.Sin(lerp * (float)Math.PI * 0.5f);
			if (activeHeadset != null)
			{
				activeHeadset.transform.localPosition = Vector3.Lerp(Vector3.up * 2f, Vector3.up * 1.1f, lerp);
				activeHeadset.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.identity, lerp);
			}
			yield return null;
		}
		if (activeHeadset != null)
		{
			activeHeadset.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
		yield return new WaitForSeconds(0.5f);
		lerp = 0f;
		timer = 0f;
		moveDuration = 0.5f;
		while (lerp < 1f)
		{
			timer += Time.deltaTime;
			lerp = Mathf.Clamp01(timer / moveDuration);
			lerp = Mathf.Sin(lerp * (float)Math.PI * 0.5f);
			headsetClone.transform.localPosition = Vector3.Lerp(Vector3.up * 9f, Vector3.up * 6f, lerp);
			yield return null;
		}
		if (activeHeadset != null)
		{
			activeHeadset.GetComponent<Rigidbody>().isKinematic = false;
		}
		printHeadsetCoroutine = null;
	}

	private IEnumerator ActivateSequence()
	{
		fillSound.Play();
		buzz.Play();
		headsetButton.ungrabbable = true;
		column[2].row[2].SetSlotState(GridSlotState.inactive);
		terminal.DistortHeadset();
		activeGridSlot = column[0].row[0];
		activeGridSlot.SetSlotState(GridSlotState.active);
		for (int i = 0; i < column.Length; i++)
		{
			for (int j = 0; j < column[i].row.Length; j++)
			{
				if (j == 0 && i != 0)
				{
					column[Mathf.Clamp(i - 1, 0, 100)].row[column[0].row.Length - 1].SetSlotState(GridSlotState.inactive);
				}
				else
				{
					column[i].row[Mathf.Clamp(j - 1, 0, 100)].SetSlotState(GridSlotState.inactive);
				}
				Vector3 blockPos = column[i].row[j].transform.position;
				blockPos.z = glowCard.position.z;
				glowCard.position = blockPos;
				glowCard.gameObject.SetActive(value: true);
				activeGridSlot = column[i].row[j];
				activeGridSlot.SetSlotState(GridSlotState.active);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
			}
		}
		for (int k = 0; k < column.Length; k++)
		{
			for (int l = 0; l < column[k].row.Length; l++)
			{
				column[k].row[l].SetSlotState(GridSlotState.active);
			}
		}
		bigGlowCard.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		yield return null;
		terminal.StopHeadsetDistortion();
		skipStartupSequence = true;
		updateGridPosition(calculateGridPosition());
		glowCard.gameObject.SetActive(value: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.15f, this);
		bigGlowCard.SetActive(value: false);
		glowCard.gameObject.SetActive(value: true);
		headsetButton.ungrabbable = false;
		startThread = null;
	}

	private void updateGridPosition(Vector2 pos)
	{
		terminal.AnimateSwitch();
		activeGridSlot = column[(int)pos.x].row[(int)pos.y];
		for (int i = 0; i < column.Length; i++)
		{
			for (int j = 0; j < column[i].row.Length; j++)
			{
				if (column[i].row[j] != activeGridSlot)
				{
					column[i].row[j].SetSlotState(GridSlotState.inactive);
				}
			}
		}
		Vector3 position = new Vector3(activeGridSlot.transform.position.x, activeGridSlot.transform.position.y, glowCard.position.z);
		glowCard.position = position;
		glowCard.gameObject.SetActive(value: true);
		column[(int)pos.x].row[(int)pos.y].SetSlotState(GridSlotState.active);
	}

	private Vector2 calculateGridPosition()
	{
		int num = 2;
		int num2 = 2;
		for (int i = 0; i < horizontalTerminals.Length; i++)
		{
			switch (horizontalTerminals[i].GetPowerState())
			{
			case OfficeTerminal.PowerState.Off:
				num--;
				break;
			case OfficeTerminal.PowerState.On:
				num++;
				break;
			}
		}
		for (int j = 0; j < verticalTerminals.Length; j++)
		{
			switch (verticalTerminals[j].GetPowerState())
			{
			case OfficeTerminal.PowerState.Off:
				num2++;
				break;
			case OfficeTerminal.PowerState.On:
				num2--;
				break;
			}
		}
		return new Vector2(num, num2);
	}

	public bool IsFull()
	{
		if (consumableComponents == null)
		{
			return true;
		}
		bool result = true;
		for (int i = 0; i < horizontalTerminals.Length; i++)
		{
			if (!horizontalTerminals[i].IsFull() || !verticalTerminals[i].IsFull())
			{
				result = false;
			}
		}
		return result;
	}
}
