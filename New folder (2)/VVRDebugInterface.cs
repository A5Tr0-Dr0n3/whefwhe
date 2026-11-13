using TMPro;
using UnityEngine;

public class VVRDebugInterface : MonoBehaviour
{
	[SerializeField]
	private ObjectButton saveNavLButton;

	[SerializeField]
	private ObjectButton saveNavRButton;

	[SerializeField]
	private ObjectButton saveAndResetButton;

	[SerializeField]
	private TextMeshPro saveSelectionDisplay;

	[SerializeField]
	private GameObject interfaceParent;

	[SerializeField]
	private SaveDataScriptableObject[] loadableSaveStates;

	[SerializeField]
	public DebugInterfaceHandle debugHandle;

	private int saveStateIndexSelection;

	public bool activated { get; private set; }

	private void Awake()
	{
		Activate(activeIn: false);
		saveNavLButton.onClickEvent.AddListener(navLeft);
		saveNavRButton.onClickEvent.AddListener(navRight);
		saveAndResetButton.onClickEvent.AddListener(resetToSelectedSave);
		drawSave();
	}

	public void ToggleActive()
	{
		Activate(!activated);
	}

	public void Activate(bool activeIn)
	{
		activated = activeIn;
		interfaceParent.SetActive(activated);
		debugHandle.ResetPosition();
	}

	private void resetToSelectedSave()
	{
		if (loadableSaveStates != null && loadableSaveStates.Length > 0 && loadableSaveStates[saveStateIndexSelection] != null)
		{
			PersistentDataManager.ReplaceActiveSaveData(loadableSaveStates[saveStateIndexSelection].saveData);
			EagerScenes.ResetAndSkipMenu();
		}
	}

	private void navLeft()
	{
		if (loadableSaveStates != null && loadableSaveStates.Length > 0)
		{
			saveStateIndexSelection--;
			if (saveStateIndexSelection < 0)
			{
				saveStateIndexSelection = loadableSaveStates.Length - 1;
			}
		}
		drawSave();
	}

	private void navRight()
	{
		if (loadableSaveStates != null && loadableSaveStates.Length > 0)
		{
			saveStateIndexSelection++;
			if (saveStateIndexSelection >= loadableSaveStates.Length)
			{
				saveStateIndexSelection = 0;
			}
		}
		drawSave();
	}

	private void drawSave()
	{
		string empty = string.Empty;
		if (loadableSaveStates != null && loadableSaveStates.Length > 0)
		{
			empty = empty + (saveStateIndexSelection + 1) + ": ";
			empty = ((!(loadableSaveStates[saveStateIndexSelection] != null)) ? (empty + "<Data Missing>") : (empty + loadableSaveStates[saveStateIndexSelection].name));
		}
		else
		{
			empty = "No save states hooked up";
		}
		saveSelectionDisplay.text = empty;
	}
}
