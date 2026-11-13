using UnityEngine;

public class MergingObject : MonoBehaviour
{
	private int gameObjectIndex;

	[SerializeField]
	private GameObject[] gameObjects;

	private void Start()
	{
		gameObjects[0].SetActive(value: false);
		gameObjectIndex = Random.Range(0, gameObjects.Length);
		NextGameObject();
	}

	public void NextGameObject()
	{
		gameObjects[gameObjectIndex].SetActive(value: false);
		gameObjectIndex++;
		if (gameObjectIndex >= gameObjects.Length)
		{
			gameObjectIndex = 0;
		}
		gameObjects[gameObjectIndex].SetActive(value: true);
	}
}
