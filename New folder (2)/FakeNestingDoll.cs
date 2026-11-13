using UnityEngine;

public class FakeNestingDoll : MonoBehaviour
{
	public VVRConsumable[] minuteTens;

	public VVRConsumable[] minuteOnes;

	public VVRConsumable[] secondTens;

	public VVRConsumable[] secondOnes;

	public VVRConsumable colon;

	public VVRConsumable activeMinuteTens;

	public VVRConsumable activeMinuteOnes;

	public VVRConsumable activeSecondTens;

	public VVRConsumable activeSecondOnes;

	private string time;

	private HRMurder murderScene;

	private void Start()
	{
		if (HRMurder.instance != null)
		{
			murderScene = HRMurder.instance;
		}
	}

	private void Update()
	{
		updateTimeText();
		processVacuum();
		if (time.Length > 2 && time.Substring(2, 2) == "00")
		{
			colon.unconsumable = false;
		}
	}

	private void updateTimeText()
	{
		time = murderScene.timeString;
		string[] array = new string[4];
		if (time.Length > 6)
		{
			array[0] = time.Substring(2, 1);
			array[1] = time.Substring(3, 1);
			array[2] = time.Substring(5, 1);
			array[3] = time.Substring(6, 1);
		}
		VVRConsumable[] array2 = new VVRConsumable[0];
		for (int i = 0; i < 4; i++)
		{
			switch (i)
			{
			case 0:
				array2 = minuteTens;
				break;
			case 1:
				array2 = minuteOnes;
				break;
			case 2:
				array2 = secondTens;
				break;
			case 3:
				array2 = secondOnes;
				break;
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] != activeMinuteOnes && array2[j] != activeMinuteTens && array2[j] != activeSecondOnes && array2[j] != activeSecondTens)
				{
					array2[j].gameObject.SetActive(value: false);
				}
				if (j.ToString() == array[i])
				{
					array2[j].gameObject.SetActive(value: true);
					switch (i)
					{
					case 0:
						activeMinuteTens = array2[j];
						break;
					case 1:
						activeMinuteOnes = array2[j];
						break;
					case 2:
						activeSecondTens = array2[j];
						break;
					case 3:
						activeSecondOnes = array2[j];
						break;
					}
				}
			}
		}
	}

	private void processVacuum()
	{
		if (activeMinuteOnes == null || activeMinuteTens == null || activeSecondOnes == null || activeSecondTens == null || murderScene == null)
		{
			return;
		}
		if (activeMinuteOnes == minuteOnes[0] && activeMinuteTens == minuteTens[0])
		{
			activeMinuteOnes.unconsumable = false;
			if (activeMinuteOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeMinuteOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic)
			{
				activeMinuteOnes.gameObject.SetActive(value: false);
			}
		}
		if (activeMinuteTens == minuteTens[0])
		{
			activeMinuteTens.unconsumable = false;
			if (activeMinuteTens.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeMinuteTens.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic)
			{
				activeMinuteTens.gameObject.SetActive(value: false);
			}
		}
		if (activeSecondOnes == secondOnes[0] && activeSecondTens == secondTens[0] && activeMinuteOnes == minuteOnes[0] && activeMinuteTens == minuteTens[0])
		{
			activeSecondOnes.unconsumable = false;
			if (activeSecondOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeSecondOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic)
			{
				activeSecondOnes.gameObject.SetActive(value: false);
			}
		}
		if (activeSecondTens == secondTens[0] && activeMinuteOnes == minuteOnes[0] && activeMinuteTens == minuteTens[0])
		{
			activeSecondTens.unconsumable = false;
			if (activeSecondTens.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeSecondTens.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic)
			{
				activeSecondTens.gameObject.SetActive(value: false);
			}
		}
		if ((activeMinuteOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeMinuteOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic) && activeMinuteOnes.isActiveAndEnabled)
		{
			murderScene.SubtractMinutesByOne();
		}
		if ((activeMinuteTens.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeMinuteTens.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic) && activeMinuteTens.isActiveAndEnabled)
		{
			murderScene.SubtractMinutesByTen();
		}
		if ((activeSecondTens.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeSecondTens.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic) && activeSecondTens.isActiveAndEnabled)
		{
			murderScene.SubtractSecondsByTen();
		}
		if ((activeSecondOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || activeSecondOnes.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic) && activeSecondOnes.isActiveAndEnabled)
		{
			murderScene.SubtractSecondsByOne();
		}
	}
}
