using UnityEngine;

public class SceneLoadTest : MonoBehaviour
{
	private bool startedQueue;

	private void Update()
	{
		if (!startedQueue)
		{
			startedQueue = true;
			EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_TURBINE);
		}
	}
}
