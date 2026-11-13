using System.Collections.Generic;
using UnityEngine;

public class VVR_STAMPER_08 : SceneBehaviorManager
{
	[SerializeField]
	private BoidController swarmController;

	private List<Collider> toBeLasered = new List<Collider>();

	[SerializeField]
	private List<ChazController> chazi = new List<ChazController>();

	private ChazController nearChaz;

	private GameObject boidObject;

	private float activeTimer;

	private float closestSqrDist;

	private bool activateAttempt;

	private Headset headset;

	private bool activatedHeadset;

	private void Update()
	{
		for (int i = 0; i < swarmController.boidList.Count; i++)
		{
			if (!swarmController.boidList[i].isActiveAndEnabled)
			{
				swarmController.boidList[i].transform.position = Random.onUnitSphere * 7f + new Vector3(0f, 10f, 0f);
				swarmController.boidList[i].gameObject.SetActive(value: true);
			}
		}
		if (EagerHeadsetManager.instance.isAddingHeadset)
		{
			for (int j = 0; j < chazi.Count; j++)
			{
				chazi[j].CancelLazer();
			}
		}
		else if (toBeLasered.Count > 0)
		{
			LaserCheck(toBeLasered[0]);
		}
		if (ToolManager.PlayerGrabbingObjectWithComponentType<Headset>())
		{
			closestSqrDist = 999f;
			for (int k = 0; k < ToolManager.ActiveToolGroups.Count; k++)
			{
				ObjectBase heldObjectBase = ToolManager.ActiveToolGroups[k].Manipulator.HeldObjectBase;
				if (heldObjectBase != null && (bool)heldObjectBase.GetComponent<Headset>())
				{
					float num = Vector3.SqrMagnitude(heldObjectBase.transform.position - EagerMaster.instance.mainCamera.transform.position);
					if (num < closestSqrDist)
					{
						closestSqrDist = num;
					}
				}
			}
			if (closestSqrDist < 20f)
			{
				activeTimer += Time.deltaTime;
				if (!(activeTimer > 1.2f) || activateAttempt)
				{
					return;
				}
				activateAttempt = true;
				for (int l = 0; l < ToolManager.ActiveToolGroups.Count; l++)
				{
					ObjectBase heldObjectBase2 = ToolManager.ActiveToolGroups[l].Manipulator.HeldObjectBase;
					if (heldObjectBase2 != null && (bool)heldObjectBase2.GetComponent<Headset>())
					{
						heldObjectBase2.GetComponent<Headset>().isActivated = true;
					}
				}
			}
			else
			{
				activeTimer = 0f;
				activateAttempt = false;
			}
		}
		else
		{
			activeTimer = 0f;
			activateAttempt = false;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!toBeLasered.Contains(col))
		{
			toBeLasered.Add(col);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (toBeLasered.Contains(col))
		{
			toBeLasered.Remove(col);
		}
	}

	private void LaserCheck(Collider col)
	{
		headset = col.gameObject.GetComponentInChildren<Headset>();
		if (headset == null && col.gameObject.GetComponentInChildren<ObjectChazLeg>() == null)
		{
			toBeLasered.Remove(col);
			return;
		}
		float num = 100000000f;
		nearChaz = null;
		for (int i = 0; i < chazi.Count; i++)
		{
			if (chazi[i].head.activeSelf && col.gameObject != chazi[i].head && chazi[i].poweredUp && !chazi[i].isLazering)
			{
				float num2 = Vector3.SqrMagnitude(col.transform.position - chazi[i].head.transform.position);
				if (num2 < num)
				{
					nearChaz = chazi[i];
					num = num2;
				}
			}
		}
		if (nearChaz != null)
		{
			nearChaz.LazerObject(col.gameObject);
			toBeLasered.Remove(col);
		}
	}
}
