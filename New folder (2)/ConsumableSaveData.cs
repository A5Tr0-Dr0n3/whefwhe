using System;

[Serializable]
public class ConsumableSaveData
{
	public float fillPercent;

	public VVRConsumable.objectState consumableState;

	public ConsumableSaveData()
	{
		fillPercent = 0f;
		consumableState = VVRConsumable.objectState.fullyConsumedStatic;
	}

	public ConsumableSaveData(VVRConsumable obj)
	{
		fillPercent = obj.currentVacuumValueCapped / obj.totalVacuumValue;
		consumableState = obj.currentObjectState;
	}
}
