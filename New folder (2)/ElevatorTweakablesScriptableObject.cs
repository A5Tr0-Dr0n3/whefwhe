using UnityEngine;

[CreateAssetMenu(fileName = "New Elevator Sequence Content", menuName = "TenderClaws/Create Elevator Sequence Content", order = 1)]
public class ElevatorTweakablesScriptableObject : ScriptableObject
{
	public ElevatorDialogue dialogue;

	public ElevatorFloorCollection floors;
}
