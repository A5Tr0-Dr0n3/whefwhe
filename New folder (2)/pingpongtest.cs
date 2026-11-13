using System.Collections.Generic;
using UnityEngine;

public class pingpongtest : MonoBehaviour
{
	public List<ChazController> chazList;

	public List<GameObject> ballList;

	private float t;

	private bool ballLook;

	private bool playerLook;

	private bool attack;

	private void Start()
	{
		if (chazList == null || ballList == null)
		{
			SafeDebug.Log("Lists cannot be null");
		}
		if (chazList.Count != ballList.Count)
		{
			SafeDebug.Log("Lists must contain the same number of elements");
		}
	}

	private void Update()
	{
		t += Time.deltaTime;
		if (!ballLook && t > 0.5f)
		{
			for (int i = 0; i < chazList.Count; i++)
			{
				chazList[i].EyeLookAt(ballList[i].transform, 0f, ChazController.eyeColor.Default, immediately: true);
			}
			ballLook = true;
		}
		if (!playerLook && t > 4f)
		{
			for (int j = 0; j < chazList.Count; j++)
			{
				chazList[j].EyeLookAt(Camera.main.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			}
			playerLook = true;
		}
		if (!attack && t > 5f)
		{
			for (int k = 0; k < chazList.Count; k++)
			{
				chazList[k].PresentBoid();
			}
			attack = true;
		}
	}
}
