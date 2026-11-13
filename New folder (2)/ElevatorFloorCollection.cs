using System;
using UnityEngine;

[Serializable]
public class ElevatorFloorCollection
{
	public GameObject defaultElevatorFloorPrefab;

	public AudioClip[] defaultElevatorAmbiances;

	public ElevatorFloor[] floorList;
}
