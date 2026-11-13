using UnityEngine;
using UnityEngine.UI;

public class EagerDreamManager : MonoBehaviour
{
	public GameObject controllerPivot;

	public GameObject messageCanvas;

	public Text messageText;

	public GameObject ray;

	public float moveSpeed;

	public float rotateLerp = 0.1f;

	private void Update()
	{
		UpdatePointer();
		UpdateStatusMessage();
	}

	private void UpdatePointer()
	{
	}

	private void UpdateStatusMessage()
	{
	}
}
