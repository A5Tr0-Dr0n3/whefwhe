using System.Collections.Generic;
using UnityEngine;

public class NestingDollFast : MonoBehaviour
{
	public List<GameObject> dollList = new List<GameObject>(0);

	public GvrAudioSource eatChime;

	public AudioEvent successChime;

	private VVRConsumable consumableDoll;

	private Vector3 startScale = Vector3.one;

	private float currentLerp;

	private float appearScalar;

	private bool hasPlayedEatChime;

	private void Start()
	{
		checkDone();
		for (int i = 0; i < dollList.Count; i++)
		{
			dollList[i].SetActive(value: false);
			consumableDoll = dollList[i].GetComponent<VVRConsumable>();
			if (consumableDoll != null)
			{
				consumableDoll.unconsumable = false;
				consumableDoll.destroyOnConsume = true;
			}
		}
		dollList[0].SetActive(value: true);
		dollList[0].GetComponent<VVRConsumable>().unconsumable = false;
		startScale = dollList[1].transform.localScale;
	}

	private void Update()
	{
		checkNull();
		checkTransfer();
		checkNull();
		checkDone();
	}

	private void checkTransfer()
	{
		if (dollList[0] != null)
		{
			consumableDoll = dollList[0].GetComponent<VVRConsumable>();
		}
		if (consumableDoll != null)
		{
			currentLerp = consumableDoll.currentVacuumValueCapped / consumableDoll.totalVacuumValue;
		}
		appearScalar = Mathf.Clamp01((1f - currentLerp) * 3f);
		if ((double)currentLerp < 0.99 && dollList.Count > 1)
		{
			dollList[1].transform.localScale = startScale * appearScalar;
			dollList[1].SetActive(value: true);
		}
	}

	private void checkNull()
	{
		if (dollList[0] == null)
		{
			eatChime.pitch *= 1.05f;
			if (dollList.Count == 2)
			{
				eatChime.pitch = 0.6f;
			}
			eatChime.PlayOneShot(eatChime.clip);
			checkDone();
			consumableDoll = dollList[1].GetComponent<VVRConsumable>();
			if (consumableDoll != null)
			{
				consumableDoll.unconsumable = false;
			}
			dollList[1].transform.localScale = startScale;
			dollList.RemoveAt(0);
			if (dollList.Count > 1)
			{
				startScale = dollList[1].transform.localScale;
			}
			else if (successChime != null)
			{
				successChime.Play();
			}
		}
	}

	private void checkDone()
	{
		if (dollList.Count == 0 && dollList.Count == 1)
		{
			dollList[0].transform.parent = transform.parent;
		}
	}
}
