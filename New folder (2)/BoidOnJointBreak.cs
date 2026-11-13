using UnityEngine;

public class BoidOnJointBreak : MonoBehaviour
{
	public VVRConsumable[] consumableReferences;

	public void BreakJoint()
	{
		for (int i = 0; i < consumableReferences.Length; i++)
		{
			consumableReferences[i].enabled = true;
		}
	}
}
