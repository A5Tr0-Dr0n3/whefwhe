using UnityEngine;

public class ColliderFinder : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		SafeDebug.Log("I hit " + other.gameObject.name);
	}
}
