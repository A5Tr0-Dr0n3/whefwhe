using System.Collections;
using UnityEngine;

public class ElevatorEmergencyPhone : MonoBehaviour
{
	[SerializeField]
	private VVRDialogueSource phoneDialogueSource;

	[SerializeField]
	private ElevatorTweakablesScriptableObject tweakables;

	[SerializeField]
	private ObjectMorphToHeadset phoneToHeadsetMorpher;

	private void Start()
	{
		if (PersistentDataManager.GetElevatorData().hasUsedPhone)
		{
			Object.Destroy(gameObject);
		}
		phoneToHeadsetMorpher.mObjectBase.onBecomeGrabbed.AddListener(GrabPhone);
	}

	private void GrabPhone()
	{
		if (phoneToHeadsetMorpher.mObjectBase.grabbedBy.Count <= 1)
		{
			GlobalCoroutineContainer.AddThread(phoneGrabSequence(), this);
		}
	}

	private IEnumerator phoneGrabSequence()
	{
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (!(ToolManager.ActiveToolGroups[i].Manipulator == phoneToHeadsetMorpher.mObjectBase.grabbedBy[0]))
			{
				ToolManager.ActiveToolGroups[i].Manipulator.SwitchTools(Manipulator.manipulatorMode.vacuum);
				ToolManager.ActiveToolGroups[i].Manipulator.Grab(phoneToHeadsetMorpher.mObjectBase, phoneToHeadsetMorpher.mObjectBase.grabbedBy[0].grabLocation, playSound: false);
			}
		}
		phoneToHeadsetMorpher.mObjectBase.undroppable = true;
		GetComponent<GvrAudioSource>().Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.75f, this);
		phoneDialogueSource.PlayClip(tweakables.dialogue.chazEmergencyPhone);
		while (phoneDialogueSource.isPlaying)
		{
			yield return null;
		}
		phoneToHeadsetMorpher.MorphIntoHeadset();
		GetComponent<Collider>().enabled = false;
		ElevatorSaveData elevatorData = PersistentDataManager.GetElevatorData();
		elevatorData.hasUsedPhone = true;
		PersistentDataManager.OverwriteElevatorData(elevatorData);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		phoneToHeadsetMorpher.mHeadsetBehavior.SendToEagerHeadset();
	}
}
