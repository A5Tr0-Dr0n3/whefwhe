using System.Collections;
using UnityEngine;

public class ObjectTinaRemote : ObjectBase
{
	[SerializeField]
	private JobTina sceneRef;

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		GlobalCoroutineContainer.AddThread(DropOnDelayCoroutine(0.25f), this);
	}

	private IEnumerator DropOnDelayCoroutine(float t)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(t, this);
		ForceDropByPlayer();
	}
}
