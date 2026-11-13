using System.Collections.Generic;
using UnityEngine;

public class ConsumableGroup : MonoBehaviour
{
	[SerializeField]
	private VVRConsumable[] mConsumables;

	public float GetConsumableCompletionPercentage(bool countPartials)
	{
		if (mConsumables == null || mConsumables.Length <= 0)
		{
			return 0f;
		}
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < mConsumables.Length; i++)
		{
			if (mConsumables[i] != null && mConsumables[i].gameObject.activeInHierarchy)
			{
				if (mConsumables[i].currentObjectState == VVRConsumable.objectState.wholeShaking || mConsumables[i].currentObjectState == VVRConsumable.objectState.wholeStatic)
				{
					num2++;
				}
				else if (mConsumables[i].totalVacuumValue == 0f)
				{
					num2++;
				}
				else if (countPartials)
				{
					num2 += mConsumables[i].currentVacuumValueCapped / mConsumables[i].totalVacuumValue;
				}
			}
			else
			{
				num++;
			}
		}
		return num2 / (float)(mConsumables.Length - num);
	}

	public void CleanGroup()
	{
		int num = mConsumables.Length;
		List<VVRConsumable> list = new List<VVRConsumable>();
		for (int i = 0; i < mConsumables.Length; i++)
		{
			if (mConsumables[i] != null && mConsumables[i].gameObject.activeInHierarchy)
			{
				list.Add(mConsumables[i]);
			}
		}
		mConsumables = list.ToArray();
		SafeDebug.Log("Cleaned ConsumableList. Old length: " + num + ", New length: " + mConsumables.Length);
	}
}
