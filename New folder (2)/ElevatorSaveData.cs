using System;

[Serializable]
public class ElevatorSaveData
{
	public enum KeyStates
	{
		Flying,
		Elevator,
		Unlocked
	}

	public bool hasUsedPhone;

	public KeyStates keyState;
}
