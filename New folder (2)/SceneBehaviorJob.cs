using System.Collections;
using System.Collections.Generic;
using GVR.Gfx;
using UnityEngine;

public class SceneBehaviorJob : SceneBehaviorManager
{
	[Header("Job to Backstage Variables:")]
	[SerializeField]
	private GameObject hideOnBackstageLoad;

	[SerializeField]
	private GameObject unhideOnBackstageLoad;

	[SerializeField]
	protected List<VVRConsumable> boidCleanup;

	[SerializeField]
	private GameObject[] destroyList;

	private EnvironmentProperties environmentScript;

	private MaterialUpdateController materialUpdateScript;

	public override void ActivateSceneBehavior()
	{
		base.ActivateSceneBehavior();
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		if (VVR_HUB.instance != null)
		{
			VVR_HUB.instance.hasStartedJob = true;
		}
		else
		{
			SafeDebug.LogError("User has launched from job scene, hub is not loaded");
		}
		SafeDebug.Log("<color=blue>Job Started</color>");
		if (ToolManager.AllManipulators.HasVacuum)
		{
			EagerScenes.instance.LoadQueue(EagerScenes.BACKSTAGE_LEVEL);
		}
	}

	public void CompleteJob()
	{
		VVR_HUB.instance.hasCompletedJob = true;
		EagerHeadsetManager.instance.RemoveHeadset();
	}

	public void GoToBackstage()
	{
		GlobalCoroutineContainer.AddThread(GoToBackstageCoroutine(), this);
	}

	private IEnumerator GoToBackstageCoroutine()
	{
		while (!EagerScenes.SceneIsLoaded(EagerScenes.BACKSTAGE_LEVEL))
		{
			yield return null;
		}
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		VVR_HUB.instance.wasInBackstage = true;
		environmentScript = GetComponent<EagerScene>().mEnvironmentProperties;
		materialUpdateScript = GetComponent<EagerScene>().defaultMaterialProperties;
		EagerScene backstage = EagerScenes.SceneToEagerScene(EagerScenes.BACKSTAGE_LEVEL);
		EagerMaster.instance.flatSource.PlayOneShot(Backstage.instance.transitionSound);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		bool lightsFlipped = false;
		int lightFlickerCount = 7;
		while (lightFlickerCount > 0)
		{
			lightFlickerCount--;
			lightsFlipped = !lightsFlipped;
			if (lightsFlipped)
			{
				environmentScript.Activate();
				if (materialUpdateScript != null)
				{
					materialUpdateScript.PushMaterials();
				}
			}
			else
			{
				backstage.mEnvironmentProperties.Activate();
				if (backstage.defaultMaterialProperties != null)
				{
					backstage.defaultMaterialProperties.PushMaterials();
				}
			}
			yield return null;
		}
		backstage.mEnvironmentProperties.Activate();
		if (materialUpdateScript != null)
		{
			backstage.defaultMaterialProperties.PushMaterials();
		}
		SafeDebug.Log("Broken out to backstage");
		if ((bool)unhideOnBackstageLoad)
		{
			unhideOnBackstageLoad.SetActive(value: true);
		}
		if ((bool)hideOnBackstageLoad)
		{
			hideOnBackstageLoad.SetActive(value: false);
		}
		if (destroyList != null)
		{
			for (int i = 0; i < destroyList.Length; i++)
			{
				if (destroyList[i] != null)
				{
					Object.Destroy(destroyList[i]);
				}
			}
		}
		List<VVRConsumable> cleanup = new List<VVRConsumable>();
		GetComponentsInChildren(cleanup);
		backstage.GetComponent<Backstage>().mainSwarmController.AddToConsumableList(toConstruct: false, cleanup, "JobCleanup");
		EagerScenes.instance.MergeScene(EagerScenes.BACKSTAGE_LEVEL);
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: true);
		enabled = false;
	}
}
