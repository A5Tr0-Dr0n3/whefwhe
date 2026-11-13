using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class ItemDisplaySwapper : MonoBehaviour
{
	public delegate void FlipAction();

	[Header("Item Groups")]
	[SerializeField]
	private ItemGroup[] allItemGroups;

	[Header("Scene References")]
	[SerializeField]
	private Transform meshRoot;

	[SerializeField]
	private ItemTrackingTrigger trackedItems;

	[SerializeField]
	private VVRDialogueSource chazDialogueSource;

	[SerializeField]
	private HubScriptableObject hubContent;

	[SerializeField]
	private ObjectTrigger playerAndChazTrigger;

	[SerializeField]
	private GameObject relaxationMonologueTrigger;

	[SerializeField]
	private ObjectBase flipperLever;

	[SerializeField]
	private ChazController chazReference;

	[SerializeField]
	private Transform chazMoveTarget;

	[SerializeField]
	private Transform outsideParent;

	[Header("Tweakables")]
	[SerializeField]
	private AnimationCurve rotationCurve;

	[SerializeField]
	private float activationPercent = 0.85f;

	[SerializeField]
	internal float rotationTime = 1.5f;

	internal int currentGroup;

	private bool snackZoneIntro;

	private bool relaxationZoneIntro;

	private bool vrZoneIntro;

	private bool buttonsOff;

	[HideInInspector]
	public bool isFlipping;

	private SafeCoroutine swapThread;

	public static event FlipAction OnFlip;

	private void Awake()
	{
		init();
	}

	private void Start()
	{
		snackZoneIntro = PersistentDataManager.GetSnackZoneIntro();
		relaxationZoneIntro = PersistentDataManager.GetRelaxationZoneIntro();
		vrZoneIntro = PersistentDataManager.GetVrZoneIntro();
	}

	public float GetRotationTime()
	{
		return rotationTime;
	}

	private void Update()
	{
		if (buttonsOff)
		{
			if (!playerAndChazTrigger.playerInside && !playerAndChazTrigger.objectInside && VVR_HUB.instance.lightsAreOn)
			{
				toggleButtons(setState: false);
				buttonsOff = false;
			}
		}
		else if (playerAndChazTrigger.playerInside || playerAndChazTrigger.objectInside)
		{
			toggleButtons(setState: true);
			buttonsOff = true;
			if (playerAndChazTrigger.objectInside)
			{
				chazReference.WalkTo(chazMoveTarget.position);
			}
		}
	}

	public void FlipToSnackZone(bool right = true)
	{
		if (!playerAndChazTrigger.playerInside && !playerAndChazTrigger.objectInside && swapThread == null)
		{
			if (!VVR_HUB.instance.GetComponent<HubExt>().settingUpFlipper)
			{
				relaxationMonologueTrigger.SetActive(value: false);
				GlobalCoroutineContainer.AddThread(TriggerChazCommentOnDelay(3.6f, 1), this);
				snackZoneIntro = true;
				PersistentDataManager.OverwriteSnackZoneIntro(1);
			}
			swapThread = GlobalCoroutineContainer.AddThread(SwitchActiveGroup(1, rotationTime, right), this);
		}
	}

	public void FlipToTeaParty()
	{
		if (!playerAndChazTrigger.playerInside && !playerAndChazTrigger.objectInside && swapThread == null)
		{
			relaxationMonologueTrigger.SetActive(value: false);
			swapThread = GlobalCoroutineContainer.AddThread(SwitchActiveGroup(0, rotationTime), this);
		}
	}

	public void FlipToRelaxationZone(bool right = true)
	{
		if (!playerAndChazTrigger.playerInside && !playerAndChazTrigger.objectInside && swapThread == null)
		{
			if (!VVR_HUB.instance.GetComponent<HubExt>().settingUpFlipper)
			{
				relaxationMonologueTrigger.SetActive(value: true);
				GlobalCoroutineContainer.AddThread(TriggerChazCommentOnDelay(3.6f, 2), this);
				PersistentDataManager.OverwriteRelaxationZoneIntro(1);
				relaxationZoneIntro = true;
			}
			swapThread = GlobalCoroutineContainer.AddThread(SwitchActiveGroup(2, rotationTime, right), this);
		}
	}

	public void FlipToVrZone(bool right = true)
	{
		if (!playerAndChazTrigger.playerInside && !playerAndChazTrigger.objectInside && swapThread == null)
		{
			relaxationMonologueTrigger.SetActive(value: false);
			if (!VVR_HUB.instance.GetComponent<HubExt>().settingUpFlipper)
			{
				GlobalCoroutineContainer.AddThread(TriggerChazCommentOnDelay(3.6f, 2), this);
				PersistentDataManager.OverwriteVrZoneIntro(1);
				vrZoneIntro = true;
			}
			swapThread = GlobalCoroutineContainer.AddThread(SwitchActiveGroup(3, rotationTime, right), this);
		}
	}

	public void FlipToNothing(bool right = true)
	{
		if (!playerAndChazTrigger.playerInside && !playerAndChazTrigger.objectInside && swapThread == null)
		{
			relaxationMonologueTrigger.SetActive(value: false);
			swapThread = GlobalCoroutineContainer.AddThread(SwitchActiveGroup(0, rotationTime, right), this);
		}
	}

	public IEnumerator SwitchActiveGroup(int group, float time = 1f, bool right = true)
	{
		if (!VVR_HUB.instance.lightsAreOn)
		{
			yield return null;
			swapThread = null;
			isFlipping = false;
			yield break;
		}
		if (group == currentGroup)
		{
			swapThread = null;
			FlipToNothing();
			yield break;
		}
		isFlipping = true;
		float t = 0f;
		Quaternion start = meshRoot.rotation;
		Quaternion end = Quaternion.AngleAxis(180f, Vector3.right);
		end = start * end;
		AddTrackedItemsToGroup();
		setGroupKinematic(s: true);
		yield return null;
		setGroupTrigger(s: true);
		bool firstFlip = true;
		bool objectsActivated = false;
		while (t <= 1f)
		{
			t += Time.deltaTime / time;
			float angle = 0f;
			angle = ((!right) ? Mathf.LerpUnclamped(0f, -360f, rotationCurve.Evaluate(t)) : Mathf.LerpUnclamped(0f, 360f, rotationCurve.Evaluate(t)));
			meshRoot.rotation = Quaternion.Euler(angle, 0f, 0f);
			AddTrackedItemsToGroup();
			if (t >= 0.5f && firstFlip)
			{
				if (OnFlip != null)
				{
					OnFlip();
				}
				disableGroup();
				prepareGroupForFlip(group);
				currentGroup = group;
				firstFlip = false;
			}
			if (t >= activationPercent && !objectsActivated)
			{
				objectsActivated = true;
				activateCurrentGroup();
			}
			yield return null;
		}
		swapThread = null;
		isFlipping = false;
	}

	public void AddTrackedItemsToGroup()
	{
		for (int i = 0; i < trackedItems.trackedObj.Count; i++)
		{
			allItemGroups[currentGroup].AddObject(trackedItems.trackedObj[i]);
		}
		trackedItems.clearList();
	}

	public ItemGroup GetCurrentGroup()
	{
		return allItemGroups[currentGroup];
	}

	private void setGroupKinematic(bool s)
	{
		allItemGroups[currentGroup].SetObjectsKinematic(s);
	}

	private void resetGroupVelocity()
	{
		allItemGroups[currentGroup].ResetVelocity();
	}

	private void disableGroup()
	{
		resetGroupVelocity();
		allItemGroups[currentGroup].DisableAllObjects();
	}

	private void prepareGroupForFlip(int group)
	{
		allItemGroups[group].EnableAllObjects();
		allItemGroups[group].SetObjectsTrigger(s: true);
		allItemGroups[group].SetObjectsKinematic(s: true);
	}

	private void setGroupTrigger(bool s)
	{
		allItemGroups[currentGroup].SetObjectsTrigger(s);
	}

	private void activateCurrentGroup()
	{
		allItemGroups[currentGroup].EnableAllObjects();
		allItemGroups[currentGroup].SetObjectsTrigger(s: false);
		allItemGroups[currentGroup].ResetVelocity();
		allItemGroups[currentGroup].SetObjectsKinematic(s: false);
	}

	private void disableAllGroups()
	{
		for (int i = 1; i < allItemGroups.Length; i++)
		{
			allItemGroups[i].DisableAllObjects();
		}
	}

	private void init()
	{
		for (int i = 0; i < allItemGroups.Length; i++)
		{
			List<ObjectBase> list = new List<ObjectBase>(allItemGroups[i].root.GetComponentsInChildren<ObjectBase>());
			for (int j = 0; j < list.Count; j++)
			{
				TrackedItem trackedItem = new TrackedItem(list[j].gameObject, outsideParent);
				allItemGroups[i].objects.Add(trackedItem);
				list[j].gameObject.SetActive(value: false);
				trackedItem.originalParent = outsideParent;
			}
			allItemGroups[i].root.gameObject.SetActive(value: true);
			disableAllGroups();
		}
	}

	private void toggleButtons(bool setState)
	{
		flipperLever.ForceDropByPlayer();
		flipperLever.ungrabbable = setState;
	}

	private IEnumerator TriggerChazCommentOnDelay(float s, int zone)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(s, this);
		if (currentGroup == zone)
		{
			switch (zone)
			{
			case 1:
				VVR_HUB.instance.GetComponent<HubExt>().CommentSnackZone();
				break;
			case 2:
				VVR_HUB.instance.GetComponent<HubExt>().CommentRelaxationZone();
				break;
			case 3:
				VVR_HUB.instance.GetComponent<HubExt>().CommentVrZone();
				break;
			}
		}
	}

	public bool IsObjectInFlipper(GameObject obj)
	{
		bool result = false;
		for (int i = 0; i < allItemGroups.Length; i++)
		{
			if (allItemGroups[i].listContains(obj))
			{
				result = true;
			}
		}
		return result;
	}

	public void RemoveObjectFromFlipper(ObjectBase obj)
	{
		for (int i = 0; i < allItemGroups.Length; i++)
		{
			allItemGroups[i].removeObject(obj);
		}
	}
}
