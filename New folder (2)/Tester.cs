using UnityEngine;

public class Tester : MonoBehaviour
{
	private BoidController controller;

	private void Start()
	{
		controller = Object.FindObjectOfType<BoidController>();
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Spawn (nearby the target)"))
		{
			controller.Spawn();
		}
		if (GUILayout.Button("Spawn (off-screen)"))
		{
			controller.Spawn(controller.transform.position - controller.transform.forward * 8f);
		}
	}
}
