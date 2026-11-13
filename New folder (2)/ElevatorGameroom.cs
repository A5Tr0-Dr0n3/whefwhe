using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class ElevatorGameroom : MonoBehaviour
{
	public List<ChazController> chazBoid;

	public List<ChazController> chazWalk;

	public AudioLoop music;

	public List<PingPongMatch> matches;

	private SafeCoroutine pingPongThread;

	private void Update()
	{
		if (pingPongThread == null)
		{
			pingPongThread = GlobalCoroutineContainer.AddThread(pingPongSequence(), this);
		}
		if (pingPongThread != null && !pingPongThread.IsRunning)
		{
		}
	}

	private IEnumerator pingPongSequence()
	{
		music.Play();
		float vacFill = EagerMaster.instance.vacuumFill;
		while (EagerMaster.instance.vacuumFill == vacFill && !ToolManager.PlayerGrabbingSomething() && !ToolManager.IsTeleporting)
		{
			yield return null;
		}
		if (ToolManager.IsTeleporting)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		}
		music.Stop();
		for (int i = 0; i < matches.Count; i++)
		{
			if (matches[i] != null)
			{
				matches[i].EndMatch();
			}
		}
		for (int j = 0; j < chazBoid.Count; j++)
		{
			if (chazBoid[j] != null)
			{
				chazBoid[j].EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(0.1f, 0.3f), this);
		}
		for (int k = 0; k < chazWalk.Count; k++)
		{
			if (chazWalk[k] != null)
			{
				chazWalk[k].EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(0.1f, 0.3f), this);
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		for (int l = 0; l < chazBoid.Count; l++)
		{
			if (chazBoid[l] != null)
			{
				chazBoid[l].PresentBoid();
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(0.3f, 0.4f), this);
		}
		for (int m = 0; m < chazWalk.Count; m++)
		{
			if (chazWalk[m] != null)
			{
				chazWalk[m].WalkTo((EagerMaster.instance.mainCamera.transform.position - chazWalk[m].transform.position) / 2f + chazWalk[m].transform.position);
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(0.1f, 0.4f), this);
		}
		yield return null;
	}
}
