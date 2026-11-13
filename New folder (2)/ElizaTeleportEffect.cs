using UnityEngine;

public class ElizaTeleportEffect : MonoBehaviour
{
	public ElizaBlockyCreation[] elizaBlocks;

	public void TeleportIn(float time)
	{
		for (int i = 0; i < elizaBlocks.Length; i++)
		{
			elizaBlocks[i].Reanimate(time);
		}
	}

	public void TeleportOut(float time)
	{
		for (int i = 0; i < elizaBlocks.Length; i++)
		{
			elizaBlocks[i].ReanimateOut(time);
		}
	}

	public void StartAmbientBlocky(float time)
	{
		for (int i = 0; i < elizaBlocks.Length; i++)
		{
			elizaBlocks[i].ReanimatePartial(time);
		}
	}

	public void CompleteAmbientBlocky(float time)
	{
		for (int i = 0; i < elizaBlocks.Length; i++)
		{
			elizaBlocks[i].ReanimateComplete(time);
		}
	}
}
