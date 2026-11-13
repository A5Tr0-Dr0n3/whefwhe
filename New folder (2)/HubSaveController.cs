using UnityEngine;

public class HubSaveController : MonoBehaviour
{
	[SerializeField]
	private Animator saveAnimator;

	[SerializeField]
	private VVRDialogueSource chazDialogueSource;

	[SerializeField]
	private NPCDialogueClip chazSaveLine;

	private ObjectButton mButton;

	private bool firedSaveDialogue;

	private void Start()
	{
		mButton = GetComponent<ObjectButton>();
		mButton.onClickEvent.AddListener(saveButtonAction);
	}

	private void saveButtonAction()
	{
		saveAnimator.SetTrigger("Save");
		if (chazDialogueSource != null && chazDialogueSource.isActiveAndEnabled && !chazDialogueSource.isPlaying && !firedSaveDialogue)
		{
			chazDialogueSource.PlayClip(chazSaveLine);
			firedSaveDialogue = true;
		}
	}
}
