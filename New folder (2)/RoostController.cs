using System.Collections.Generic;
using UnityEngine;

public class RoostController : MonoBehaviour
{
	public List<GameObject> unoccupiedRoostPositions;

	public List<GameObject> occupiedRoostPositions;

	private void onDisable()
	{
		unoccupiedRoostPositions.Clear();
		occupiedRoostPositions.Clear();
	}

	public void occupyRoost(GameObject roost)
	{
		if (unoccupiedRoostPositions.Contains(roost))
		{
			unoccupiedRoostPositions.Remove(roost);
			occupiedRoostPositions.Add(roost);
		}
	}

	public void vacateRoost(GameObject roost)
	{
		if (occupiedRoostPositions.Contains(roost))
		{
			occupiedRoostPositions.Remove(roost);
			unoccupiedRoostPositions.Add(roost);
		}
	}
}
