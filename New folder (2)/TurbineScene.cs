using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using TButt;
using UnityEngine;

public class TurbineScene : SceneBehaviorManager
{
	public VVRDialogueSource chazDialogueSource;

	[Header("Chaz Dialogue Clips")]
	public NPCDialogueClip intro;

	public NPCDialogueClip notMuchElseToDo;

	public NPCDialogueClip unexpectedReturn;

	public List<NPCDialogueClip> point;

	public List<NPCDialogueClip> click;

	public List<NPCDialogueClip> hold;

	public List<NPCDialogueClip> time;

	[Header("Rift Clips")]
	public List<NPCDialogueClip> riftPoint;

	public List<NPCDialogueClip> riftClick;

	public List<NPCDialogueClip> riftHold;

	private float timer;

	private SafeCoroutine tutorialThread;

	private bool returned;

	protected override void OnSceneActivated()
	{
		if (tutorialThread == null)
		{
			tutorialThread = GlobalCoroutineContainer.AddThread(turbineCoroutine(), this);
		}
		else if (!returned)
		{
			if (tutorialThread != null && tutorialThread.IsRunning)
			{
				tutorialThread.Stop();
			}
			chazDialogueSource.ForcePlayClipOverwrite(unexpectedReturn);
			timer = -2f;
			returned = true;
		}
	}

	private IEnumerator turbineCoroutine()
	{
		yield return new WaitForSeconds(1f);
		chazDialogueSource.PlayClip(intro);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		bool hasPlayedClick = false;
		for (int i = 0; i < riftPoint.Count; i++)
		{
			hasPlayedClick = false;
			chazDialogueSource.PlayClip(riftPoint[i]);
			while (timer < 10f)
			{
				timer += Time.deltaTime;
				for (int j = 0; j < ToolManager.ActiveToolGroups.Count; j++)
				{
					if (HeadsetRemovalUIManager.instance.PointingAtHead(ToolManager.ActiveToolGroups[j].Controller) && !hasPlayedClick)
					{
						if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
						{
							chazDialogueSource.ForcePlayClipOverwrite(riftClick[i % riftClick.Count]);
							hasPlayedClick = true;
						}
						else
						{
							chazDialogueSource.ForcePlayClipOverwrite(click[i % click.Count]);
							hasPlayedClick = true;
						}
						timer = 10f;
						break;
					}
				}
				yield return null;
			}
			timer = 0f;
			while (chazDialogueSource.isPlaying)
			{
				TBInput.Controller removalAttemptController = TBInput.Controller.None;
				for (int k = 0; k < ToolManager.ActiveToolGroups.Count; k++)
				{
					if (HeadsetRemovalUIManager.instance.PointingAtHead(ToolManager.ActiveToolGroups[k].Controller) && TBInput.GetButtonDown(TBInput.Button.Action1, ToolManager.ActiveToolGroups[k].Controller))
					{
						removalAttemptController = ToolManager.ActiveToolGroups[k].Controller;
					}
				}
				bool hasPlayedPoint = false;
				if (!chazDialogueSource.isPlaying && TBInput.GetButtonUp(TBInput.Button.Action1, removalAttemptController) && removalAttemptController != TBInput.Controller.None && !hasPlayedPoint)
				{
					if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
					{
						chazDialogueSource.PlayClip(riftHold[i % riftHold.Count]);
						hasPlayedPoint = true;
					}
					else
					{
						chazDialogueSource.PlayClip(hold[i % hold.Count]);
						hasPlayedPoint = true;
					}
				}
				yield return null;
			}
		}
		chazDialogueSource.PlayClip(notMuchElseToDo);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		while (true)
		{
			timer += Time.deltaTime;
			if (timer > 10f)
			{
				chazDialogueSource.PlayClip(time[2]);
				timer = 0f;
			}
			if (!chazDialogueSource.isPlaying)
			{
				for (int l = 0; l < ToolManager.ActiveToolGroups.Count; l++)
				{
					if (HeadsetRemovalUIManager.instance.PointingAtHead(ToolManager.ActiveToolGroups[l].Controller))
					{
						if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
						{
							chazDialogueSource.PlayClip(riftClick[1]);
						}
						else
						{
							chazDialogueSource.PlayClip(click[1]);
						}
						timer = 0f;
						break;
					}
				}
			}
			yield return null;
		}
	}
}
