using System.Collections.Generic;
using TButt;
using UnityEngine;

public class HackingTool : MonoBehaviour
{
	public enum ToolMode
	{
		select,
		delete,
		spawn
	}

	private ToolMode currToolMode = ToolMode.delete;

	public float raycastRange = 99f;

	public GameObject cursor;

	public List<VVRConsumable> spawnList;

	[HideInInspector]
	public List<VVRConsumable> hackToDeconstruct = new List<VVRConsumable>();

	[HideInInspector]
	public List<VVRConsumable> hackToConstruct = new List<VVRConsumable>();

	[HideInInspector]
	public BoidControlBoard controlBoard;

	private RaycastHit hitInfo;

	private VVRConsumable pointedConsumable;

	private VVRConsumable spawnConsumable;

	private Vector3 rayDirection;

	private ObjectButton hitButton;

	public ToolMode CurrToolMode
	{
		get
		{
			return currToolMode;
		}
		set
		{
			if (currToolMode == ToolMode.spawn && spawnConsumable != null)
			{
				spawnConsumable.gameObject.SetActive(value: false);
			}
			if (value != ToolMode.spawn || (spawnList != null && spawnList.Count >= 1))
			{
				currToolMode = value;
				if (value != ToolMode.select && value != ToolMode.delete && value == ToolMode.spawn)
				{
					spawnConsumable = spawnList[0];
					spawnConsumable.gameObject.SetActive(value: true);
				}
			}
		}
	}

	private void Update()
	{
		if (CurrToolMode == ToolMode.spawn && spawnConsumable == null)
		{
			CurrToolMode = ToolMode.delete;
		}
		rayDirection = EagerMaster.instance.player.transform.rotation * (TBInput.GetPointerRotation(TBInput.Controller.Active) * Vector3.forward);
		if (GoodCast())
		{
			cursor.SetActive(value: true);
			cursor.transform.position = hitInfo.point;
			hitButton = hitInfo.transform.GetComponentInChildren<ObjectButton>();
			if ((bool)hitButton && TBInput.GetButtonDown(TBInput.Button.Action1, TBInput.Controller.Active))
			{
				hitButton.OnButtonPress();
			}
			switch (CurrToolMode)
			{
			case ToolMode.delete:
				DeleteToolCast();
				break;
			case ToolMode.spawn:
				SpawnToolCast();
				break;
			}
		}
		else
		{
			cursor.SetActive(value: false);
		}
	}

	private void SpawnToolCast()
	{
		spawnConsumable.transform.position = cursor.transform.position;
		if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Teleport"))
		{
			spawnConsumable.gameObject.SetActive(value: true);
			spawnConsumable.GetComponent<MeshRenderer>().enabled = true;
			if (TBInput.GetButtonDown(TBInput.Button.Action1, TBInput.Controller.Active))
			{
				hackToConstruct.Add(spawnConsumable);
				spawnList.Remove(spawnConsumable);
				spawnConsumable = null;
				CurrToolMode = ToolMode.delete;
			}
		}
		else
		{
			spawnConsumable.gameObject.SetActive(value: false);
		}
	}

	private void DeleteToolCast()
	{
		if (!(pointedConsumable != null) || !(pointedConsumable != hitInfo.transform.GetComponentInChildren<VVRConsumable>()) || !hackToDeconstruct.Contains(pointedConsumable))
		{
		}
		pointedConsumable = hitInfo.transform.GetComponentInChildren<VVRConsumable>();
		if ((bool)pointedConsumable && !pointedConsumable.unconsumable)
		{
			if (TBInput.GetButtonDown(TBInput.Button.Action1, TBInput.Controller.Active))
			{
				if (hackToDeconstruct.Contains(pointedConsumable))
				{
					hackToDeconstruct.Remove(pointedConsumable);
				}
				else
				{
					hackToDeconstruct.Add(pointedConsumable);
				}
			}
			else if (hackToDeconstruct.Contains(pointedConsumable))
			{
			}
		}
		else
		{
			pointedConsumable = null;
		}
	}

	private bool GoodCast()
	{
		return Physics.Raycast(EagerMaster.instance.mainCamera.transform.position, rayDirection, out hitInfo, raycastRange, EagerMaster.instance.leashRayMask, QueryTriggerInteraction.Ignore);
	}
}
