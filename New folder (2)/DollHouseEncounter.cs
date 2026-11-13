using System;

[Serializable]
public struct DollHouseEncounter
{
	public DollhouseRoom targetRoom;

	public float timeSpentInRoom;

	public int dialogueIndex;

	public DollhouseOrientation orientation;
}
