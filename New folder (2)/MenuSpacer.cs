using System.Collections;
using UnityEngine;

public class MenuSpacer : MonoBehaviour
{
	public MeshRenderer[] left;

	public MeshRenderer[] middle;

	public GameObject[] right;

	public float spacer;

	private void OnEnable()
	{
		Space();
		PersistentDataManager.onLanguageChange += Space;
	}

	private void OnDisable()
	{
		PersistentDataManager.onLanguageChange -= Space;
	}

	private void Space()
	{
		StartCoroutine("SpaceCoroutine");
	}

	private IEnumerator SpaceCoroutine()
	{
		yield return new WaitForEndOfFrame();
		float leftMax = -9999f;
		float middleMax = -9999f;
		for (int i = 0; i < left.Length; i++)
		{
			if (left[i].bounds.max.x > leftMax)
			{
				leftMax = left[i].bounds.max.x;
			}
		}
		for (int j = 0; j < middle.Length; j++)
		{
			middle[j].transform.position = new Vector3(leftMax + spacer, middle[j].transform.position.y, middle[j].transform.position.z);
			if (middle[j].bounds.max.x > leftMax)
			{
				middleMax = middle[j].bounds.max.x;
			}
		}
		for (int k = 0; k < right.Length; k++)
		{
			right[k].transform.position = new Vector3(middleMax + spacer, right[k].transform.position.y, right[k].transform.position.z);
		}
	}
}
