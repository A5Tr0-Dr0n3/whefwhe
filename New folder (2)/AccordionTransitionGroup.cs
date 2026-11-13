using System;
using UnityEngine;

public class AccordionTransitionGroup : MonoBehaviour
{
	[Serializable]
	public class AccordionTransitionLine
	{
		[SerializeField]
		private AccordionTransitionElement[] lineElements;

		public void ApplyT(float t, float inZ, float outZ)
		{
			for (int i = 0; i < lineElements.Length; i++)
			{
				lineElements[i].ApplyTransitionT(t, inZ, outZ);
			}
		}
	}

	[SerializeField]
	private AccordionTransitionLine[] lines;

	[SerializeField]
	private GameObject[] objectsToDisable;

	public void ApplyTransitionT(float t, float overlapPercentage, float inZ, float outZ)
	{
		for (int i = 0; i < objectsToDisable.Length; i++)
		{
			objectsToDisable[i].SetActive(value: true);
		}
		float num = 1f / (1f + overlapPercentage * (float)(lines.Length - 1));
		for (int j = 0; j < lines.Length; j++)
		{
			float a = num * ((float)j * overlapPercentage);
			float b = num * ((float)j * overlapPercentage + 1f);
			float t2 = Mathf.InverseLerp(a, b, t);
			lines[j].ApplyT(t2, inZ, outZ);
		}
	}

	public void Hide()
	{
		for (int i = 0; i < lines.Length; i++)
		{
			lines[i].ApplyT(0f, 0f, 0f);
		}
		for (int j = 0; j < objectsToDisable.Length; j++)
		{
			objectsToDisable[j].SetActive(value: false);
		}
	}
}
