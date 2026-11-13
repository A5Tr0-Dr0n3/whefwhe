using UnityEngine;

public class HubPauseController : MonoBehaviour
{
	[SerializeField]
	private ObjectButton mButton;

	private void Awake()
	{
		mButton = gameObject.GetComponent<ObjectButton>();
		mButton.onClickEvent.AddListener(pauseGame);
	}

	private void pauseGame()
	{
		if (PlayerScaler.instance != null && !PlayerScaler.instance.scaled)
		{
			EagerScenes.instance.GoToPauseScene();
		}
		else if (PlayerScaler.instance == null)
		{
			EagerScenes.instance.GoToPauseScene();
		}
	}
}
