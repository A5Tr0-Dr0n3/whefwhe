using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class PostHernandezChazCaptureSequence : MonoBehaviour
{
	[SerializeField]
	private ChazController mChazController;

	[SerializeField]
	private VVRDialogueSource chazDialogueSource;

	[SerializeField]
	private NPCDialogueClip[] chazChewsYouOut;

	[SerializeField]
	private BoidController boidController;

	[SerializeField]
	private Vector3 forcePosition;

	private SafeCoroutine sequenceThread;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(forcePosition, 0.2f);
	}

	public void Deactivate()
	{
		gameObject.SetActive(value: false);
	}

	public void BeginCaptureSequence()
	{
		gameObject.SetActive(value: true);
		EagerMaster.instance.player.transform.position = forcePosition;
		if (sequenceThread == null)
		{
			sequenceThread = GlobalCoroutineContainer.AddThread(chazCaptureSequence(), this);
		}
	}

	private IEnumerator chazCaptureSequence()
	{
		VVR_Jail.instance.postHernandez = true;
		Teleporter.SetTeleportEnabled(enabledIn: false);
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		PersistentDataManager.OverwriteArchiveState(VVR_Archive.ArchiveStates.Eliza);
		mChazController.LazerObject(EagerScenes.instance.lastHeadsetRemoved.gameObject, destroy: true);
		for (int i = 0; i < chazChewsYouOut.Length; i++)
		{
			chazDialogueSource.PlayClip(chazChewsYouOut[i]);
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		boidController.PowerUpAll();
		boidController.SetSwarmBehavior(ObjectBoid.behaviorMode.chase);
		yield return null;
	}
}
