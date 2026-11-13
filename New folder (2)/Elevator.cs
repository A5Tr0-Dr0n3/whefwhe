using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
	public ObjectButton button;

	public ObjectTrigger playerTrigger;

	private bool goingUp;

	public float moveSpeed = 2f;

	public float height = 6f;

	private float moveSpeedCounter;

	private bool isMoving;

	private Vector3 playerPosition;

	private Vector3 startPos;

	private Vector3 destination;

	private void Awake()
	{
		startPos = transform.position;
	}

	private void Update()
	{
		if (button.hasBeenPressed && playerTrigger.objectInside)
		{
			SafeDebug.Log("UPDATE");
			MoveElevator();
		}
	}

	private void MoveElevator()
	{
		SafeDebug.Log("TRYING TO MOVE ELEVATOR");
		if (!isMoving)
		{
			isMoving = true;
			goingUp = !goingUp;
			moveSpeedCounter = 0f;
			StartCoroutine(UpdateElevator());
		}
	}

	private IEnumerator UpdateElevator()
	{
		SafeDebug.Log("STARTING ELEVATOR");
		if (goingUp)
		{
			destination = startPos + Vector3.up * height;
			while (transform.position.y < height)
			{
				moveSpeedCounter += Time.deltaTime;
				transform.position = Vector3.Lerp(startPos, destination, moveSpeedCounter / moveSpeed);
				playerPosition = EagerMaster.instance.player.transform.position;
				playerPosition.y = transform.position.y;
				EagerMaster.instance.player.transform.position = playerPosition;
				yield return null;
			}
			transform.position = destination;
		}
		else
		{
			while (transform.position.y > 0f)
			{
				moveSpeedCounter += Time.deltaTime;
				transform.position = Vector3.Lerp(destination, startPos, moveSpeedCounter / moveSpeed);
				playerPosition = EagerMaster.instance.player.transform.position;
				playerPosition.y = transform.position.y;
				EagerMaster.instance.player.transform.position = playerPosition;
				yield return null;
			}
			transform.position = startPos;
		}
		isMoving = false;
	}
}
