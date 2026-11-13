using UnityEngine;

public class VVRDialogueTester : MonoBehaviour
{
	private VVRDialogueSource mTalker;

	[SerializeField]
	private NPCDialogueClip[] testClipPool;

	private void Start()
	{
		mTalker = GetComponent<VVRDialogueSource>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			mTalker.PlayClip(getRandomAudioClip());
		}
	}

	private NPCDialogueClip getRandomAudioClip()
	{
		return testClipPool[Random.Range(0, testClipPool.Length)];
	}
}
