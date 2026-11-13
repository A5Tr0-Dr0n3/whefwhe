using System.Collections;
using GVR.Gfx;
using UnityEngine;

public class StartupManager : MonoBehaviour
{
	[SerializeField]
	private StartupTransitionController transitionController;

	[SerializeField]
	private UserAccountMenu accountMenu;

	[SerializeField]
	private Transform loginMenuEnvironment;

	[SerializeField]
	private Transform resetSequenceEnvironment;

	[SerializeField]
	private EnvironmentProperties loginEnvironmentProperties;

	[SerializeField]
	private EnvironmentProperties resetEnvironmentProperties;

	public bool userHasLeftMenu { get; private set; }

	public void InitializeAsLoginMenu()
	{
		PersistentDataManager.SetActiveSaveSlot(-1);
		ToolManager.ToggleManipulator(toggle: true);
		CharacterFlip.SetCanFlip(newValue: false);
		ToolManager.AllManipulators.SceneCanVacuum = false;
		Teleporter.SetTeleportEnabled(enabledIn: false);
		transitionController.Initialize();
		accountMenu.SetUpUserAccountMenu(() =>
		{
			userHasLeftMenu = true;
		});
		loginMenuEnvironment.gameObject.SetActive(value: true);
		resetSequenceEnvironment.gameObject.SetActive(value: false);
		loginEnvironmentProperties.Activate();
	}

	public void InitializeAsInstantLoader()
	{
		CharacterFlip.SetCanFlip(newValue: false);
		ToolManager.ToggleManipulator(toggle: false);
		ToolManager.AllManipulators.SceneCanVacuum = false;
		Teleporter.SetTeleportEnabled(enabledIn: false);
		transitionController.Initialize();
		accountMenu.DisableUserAccountMenu();
		loginMenuEnvironment.gameObject.SetActive(value: false);
		resetSequenceEnvironment.gameObject.SetActive(value: true);
		resetEnvironmentProperties.Activate();
	}

	public void Disable()
	{
		accountMenu.DisableUserAccountMenu();
		gameObject.SetActive(value: false);
	}

	public IEnumerator LoadingBarTransition()
	{
		ToolManager.ToggleManipulator(toggle: false);
		yield return StartCoroutine(transitionController.LoadingSequence());
	}

	public IEnumerator ExitPreloadTransition()
	{
		yield return StartCoroutine(transitionController.ExitLoadingScreenSeqeunce());
	}
}
