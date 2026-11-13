using UnityEngine;

public class BackstageGlobeAnimations : MonoBehaviour
{
	[SerializeField]
	private GameObject girder;

	private float moveSpeed;

	private bool doneMoving;

	private void Start()
	{
		girder.transform.localPosition = Vector3.up * Random.Range(5f, 25f);
		moveSpeed = Random.Range(0.25f, 0.75f);
	}

	private void Update()
	{
		if (!doneMoving)
		{
			MoveGirderIntoPosition();
		}
	}

	private void MoveGirderIntoPosition()
	{
		girder.transform.localPosition = Vector3.up * (girder.transform.localPosition.y - moveSpeed * Time.deltaTime);
		if (girder.transform.localPosition.y < 0f)
		{
			doneMoving = true;
		}
	}
}
