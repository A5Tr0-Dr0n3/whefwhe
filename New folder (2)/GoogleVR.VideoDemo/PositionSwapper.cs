using UnityEngine;

namespace GoogleVR.VideoDemo;

public class PositionSwapper : MonoBehaviour
{
	private int currentIndex = -1;

	public Vector3[] Positions = new Vector3[0];

	public void SetConstraint(int index)
	{
	}

	public void SetPosition(int index)
	{
		currentIndex = index % Positions.Length;
		transform.localPosition = Positions[currentIndex];
	}
}
