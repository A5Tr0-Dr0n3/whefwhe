using UnityEngine;

public class ElevatorDoorController : MonoBehaviour
{
	[SerializeField]
	private GameObject doors;

	[SerializeField]
	private ObjectTrigger playerTrigger;

	[SerializeField]
	private GameObject[] leftDoors;

	[SerializeField]
	private GameObject[] rightDoors;

	[SerializeField]
	private AudioEvent doorOpen;

	[SerializeField]
	private AudioEvent doorClose;

	private bool doorsClosed;

	private bool closingDoors;

	private bool animatingDoors;

	private Vector3 openPos = Vector3.up * 2.25f;

	private Vector3 closePos = Vector3.zero;

	[SerializeField]
	private VVR_ElevatorNew elevatorScript;

	private float openTime = 1.5f;

	private float lerp;

	private float lerpCount;

	private Vector3 newPos;

	private void Start()
	{
		AnimateDoors(closingDoors: false);
	}

	public void DoorButtonPressed()
	{
		if (!animatingDoors && !elevatorScript.isMoving)
		{
			if (doorsClosed)
			{
				AnimateDoors(closingDoors: false);
				doorClose.Play();
			}
			else if (playerTrigger.playerInside)
			{
				AnimateDoors(closingDoors: true);
				doorOpen.Play();
			}
		}
	}

	public void AnimateDoors(bool closingDoors)
	{
		lerp = 0f;
		lerpCount = 0f;
		animatingDoors = true;
		this.closingDoors = closingDoors;
		if (!closingDoors)
		{
			elevatorScript.CheckButtonStates();
		}
	}

	private void Update()
	{
		if (!animatingDoors)
		{
			return;
		}
		lerpCount += Time.deltaTime;
		lerp = Mathf.Clamp01(lerpCount / openTime);
		lerp *= lerp;
		if (closingDoors)
		{
			for (int i = 0; i < leftDoors.Length; i++)
			{
				newPos = leftDoors[i].transform.position;
				newPos.x = 1f - lerp;
				leftDoors[i].transform.position = newPos;
				newPos.x = -1f + lerp;
				rightDoors[i].transform.position = newPos;
			}
			doors.transform.position = Vector3.Lerp(openPos, closePos, lerp);
		}
		else
		{
			for (int j = 0; j < leftDoors.Length; j++)
			{
				newPos = leftDoors[j].transform.position;
				newPos.x = lerp;
				leftDoors[j].transform.position = newPos;
				newPos.x = 0f - lerp;
				rightDoors[j].transform.position = newPos;
			}
			doors.transform.position = Vector3.Lerp(closePos, openPos, lerp);
		}
		if (lerpCount / openTime > 1f)
		{
			animatingDoors = false;
			if (closingDoors)
			{
				doorsClosed = true;
			}
			else
			{
				doorsClosed = false;
			}
			elevatorScript.CheckButtonStates();
		}
	}

	public bool DoorsAreMoving()
	{
		return animatingDoors;
	}

	public bool DoorsAreClosed()
	{
		if (!animatingDoors && doorsClosed)
		{
			return true;
		}
		return false;
	}
}
