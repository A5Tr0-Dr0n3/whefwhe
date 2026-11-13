using UnityEngine;

public class TinaEatTrigger : MonoBehaviour
{
	private bool quitting;

	[SerializeField]
	private JobTina sceneRef;

	private void OnApplicationQuit()
	{
		quitting = true;
	}

	private void OnDestroy()
	{
		if (sceneRef.isActiveAndEnabled && !quitting)
		{
			sceneRef.tinaEatTrigger = null;
			sceneRef.EndLevel(TinaEndState.Vacuum);
		}
	}
}
