using UnityEngine;

public class ElevatorDoorways : MonoBehaviour
{
	public ElevatorKey keyGO;

	public GameObject lockedSafe;

	public ElevatorDoorController doors;

	private void Update()
	{
		if (!doors.DoorsAreClosed() && PersistentDataManager.GetElevatorData().keyState == ElevatorSaveData.KeyStates.Elevator)
		{
			PersistentDataManager.SetElevatorKey(ElevatorSaveData.KeyStates.Unlocked);
			PersistentDataManager.WriteSaveDelay(1f);
			keyGO.animateUnlock(lockedSafe);
		}
	}
}
