using UnityEngine;

public class Skeleton : MonoBehaviour
{
	public GameObject eyeLeft;

	public GameObject eyeRight;

	public GameObject eyeThird;

	private float lookTime = 2f;

	private float lookCount;

	public float randomLookDistance = 0.1f;

	private void Update()
	{
		lookCount += Time.deltaTime;
		if (lookCount > lookTime)
		{
			lookCount = 0f;
			if ((bool)eyeLeft)
			{
				eyeLeft.transform.LookAt(EagerMaster.instance.mainCamera.transform.position + Random.insideUnitSphere * randomLookDistance);
			}
			if ((bool)eyeRight)
			{
				eyeRight.transform.LookAt(EagerMaster.instance.mainCamera.transform.position + Random.insideUnitSphere * randomLookDistance);
			}
			if ((bool)eyeThird)
			{
				eyeThird.transform.LookAt(EagerMaster.instance.mainCamera.transform.position + Random.insideUnitSphere * randomLookDistance);
			}
		}
		if ((bool)eyeLeft)
		{
			eyeLeft.transform.LookAt(EagerMaster.instance.mainCamera.transform.position + Random.insideUnitSphere * randomLookDistance);
		}
		if ((bool)eyeRight)
		{
			eyeRight.transform.LookAt(EagerMaster.instance.mainCamera.transform.position + Random.insideUnitSphere * randomLookDistance);
		}
		if ((bool)eyeThird)
		{
			eyeThird.transform.LookAt(EagerMaster.instance.mainCamera.transform.position + Random.insideUnitSphere * randomLookDistance);
		}
	}
}
