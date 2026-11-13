using System.Collections;
using UnityEngine;

public class __OLDTutorial : MonoBehaviour
{
	public enum TutorialState
	{
		Introduction,
		EmployeeNumber,
		ObjectSelection,
		ObjectWatering,
		Teleporting,
		Headset
	}

	public TutorialState tutorialState;

	private TutorialState previousState;

	public TextMesh employeeNumber;

	public GameObject activitudeTextParent;

	public GameObject employeeTextParent;

	public GameObject objectParent;

	public GameObject grabParent;

	public GameObject teleportParent;

	public GameObject headsetParent;

	public ObjectTrigger teleportTrigger;

	public ObjectButton[] objectSelections;

	public GameObject wateringCan;

	public GameObject fishFood;

	public GameObject plant;

	public GameObject fish;

	public GameObject shoe;

	private bool objectSelectFlag;

	private ObjectButton selectedObject;

	private WaitForSeconds titleWait = new WaitForSeconds(2f);

	private WaitForSeconds employeeNumberWait = new WaitForSeconds(2f);

	private void Start()
	{
		activitudeTextParent.SetActive(value: false);
		employeeTextParent.SetActive(value: false);
		objectParent.SetActive(value: false);
		grabParent.SetActive(value: false);
		teleportParent.SetActive(value: false);
		headsetParent.SetActive(value: false);
		Invoke("BeginCoroutine", 0.1f);
	}

	private void BeginCoroutine()
	{
		StartCoroutine(StartTutorial());
	}

	private void Update()
	{
	}

	private IEnumerator StartTutorial()
	{
		if (tutorialState == TutorialState.Introduction)
		{
			activitudeTextParent.SetActive(value: true);
			yield return titleWait;
			tutorialState = TutorialState.EmployeeNumber;
		}
		if (tutorialState == TutorialState.EmployeeNumber)
		{
			activitudeTextParent.SetActive(value: false);
			employeeTextParent.SetActive(value: true);
			yield return employeeNumberWait;
			tutorialState = TutorialState.ObjectSelection;
		}
		if (tutorialState == TutorialState.ObjectSelection)
		{
			employeeTextParent.SetActive(value: false);
			objectParent.SetActive(value: true);
			while (!objectSelectFlag)
			{
				for (int i = 0; i < objectSelections.Length; i++)
				{
					if (!objectSelections[i].hasBeenPressed)
					{
						continue;
					}
					selectedObject = objectSelections[i];
					selectedObject.enabled = false;
					objectSelectFlag = true;
					SafeDebug.Log("Object " + selectedObject.name + " has been selected");
					for (int j = 0; j < objectSelections.Length; j++)
					{
						if (objectSelections[j] != selectedObject)
						{
							objectSelections[j].transform.localScale *= 0.5f;
							objectSelections[j].enabled = false;
							objectSelections[j].GetComponent<ObjectWater>().enabled = false;
						}
					}
					break;
				}
				yield return null;
			}
			tutorialState = TutorialState.ObjectWatering;
		}
		if (tutorialState == TutorialState.ObjectWatering)
		{
			grabParent.SetActive(value: true);
			wateringCan.SetActive(value: false);
			fishFood.SetActive(value: false);
			if (selectedObject.gameObject == plant)
			{
				fishFood.SetActive(value: true);
			}
			else
			{
				wateringCan.SetActive(value: true);
			}
			while (!selectedObject.GetComponent<ObjectBase>().isSatisfied)
			{
				yield return null;
			}
			tutorialState = TutorialState.Teleporting;
		}
		if (tutorialState == TutorialState.Teleporting)
		{
			teleportParent.gameObject.SetActive(value: true);
			while (!teleportTrigger.objectInside)
			{
				yield return null;
			}
			tutorialState = TutorialState.Headset;
		}
		if (tutorialState == TutorialState.Headset)
		{
			headsetParent.SetActive(value: true);
		}
		yield return null;
	}
}
