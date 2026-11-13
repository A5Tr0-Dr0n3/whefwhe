using UnityEngine;

public class EagerHeightManger : MonoBehaviour
{
	[SerializeField]
	private float numSamples;

	[SerializeField]
	private float maxHeadHeight = 0.1f;

	[SerializeField]
	private float minHeadHeight = -0.3f;

	[SerializeField]
	internal Transform headModel;

	private Vector3 averageHeadPos;

	private Vector3 culumativeValue;

	private Vector3 currentHeadPos;

	private float offset;

	private Vector3 startHeadHeight;

	private void Awake()
	{
		startHeadHeight = headModel.transform.localPosition;
	}

	private void Update()
	{
		updateCumulativeAverage();
	}

	private void updateCumulativeAverage()
	{
		currentHeadPos = EagerMaster.instance.mainCamera.transform.localPosition;
		culumativeValue = culumativeValue + currentHeadPos - culumativeValue / numSamples;
		averageHeadPos = culumativeValue / numSamples;
		offset = maxHeadHeight - currentHeadPos.y;
	}

	public Vector3 GetMovingAverage()
	{
		return averageHeadPos = culumativeValue / numSamples;
	}

	public float GetMovingAverageHeight()
	{
		return GetMovingAverage().y;
	}

	public void NormalizePlayerHeight()
	{
		if (GetMovingAverageHeight() > maxHeadHeight)
		{
			Vector3 vector = new Vector3(startHeadHeight.x, startHeadHeight.y + (maxHeadHeight - currentHeadPos.y), startHeadHeight.z);
			headModel.transform.localPosition = new Vector3(startHeadHeight.x, startHeadHeight.y + (maxHeadHeight - currentHeadPos.y), startHeadHeight.z);
		}
		else if (GetMovingAverageHeight() < minHeadHeight)
		{
			Vector3 localPosition = new Vector3(startHeadHeight.x, startHeadHeight.y + (minHeadHeight - currentHeadPos.y), startHeadHeight.z);
			headModel.transform.localPosition = localPosition;
		}
	}

	public float ProjectedHeight()
	{
		if (GetMovingAverageHeight() > maxHeadHeight)
		{
			return startHeadHeight.y + (maxHeadHeight - currentHeadPos.y) + currentHeadPos.y;
		}
		if (GetMovingAverageHeight() < minHeadHeight)
		{
			return startHeadHeight.y + (minHeadHeight - currentHeadPos.y) + currentHeadPos.y;
		}
		return headModel.transform.localPosition.y + currentHeadPos.y;
	}

	public Vector3 ProjectedLocalPosition()
	{
		if (GetMovingAverageHeight() > maxHeadHeight)
		{
			return new Vector3(startHeadHeight.x, startHeadHeight.y + (maxHeadHeight - currentHeadPos.y), startHeadHeight.z);
		}
		if (GetMovingAverageHeight() < minHeadHeight)
		{
			return new Vector3(startHeadHeight.x, startHeadHeight.y + (minHeadHeight - currentHeadPos.y), startHeadHeight.z);
		}
		return headModel.transform.localPosition;
	}
}
