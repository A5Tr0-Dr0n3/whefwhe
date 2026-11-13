using System;
using UnityEngine;

[Serializable]
public class ElevatorFloor
{
	public string floorName = string.Empty;

	public AudioClip customFloorAudio;

	public GameObject floorPrefab;

	[HideInInspector]
	public ElevatorFloorGameObjectAssembly spawned;
}
