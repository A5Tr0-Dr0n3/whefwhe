using UnityEngine;

public class Datastream : MonoBehaviour
{
	public float moveSpeed = 10f;

	private float randomMoveSpeed;

	private float codeDistance;

	private VVRConsumable[] snippets;

	private VVRConsumable topSnippet;

	private void Start()
	{
		snippets = transform.GetComponentsInChildren<VVRConsumable>();
		topSnippet = snippets[0];
	}

	private void Update()
	{
		randomMoveSpeed = moveSpeed * Random.Range(0.75f, 1.25f);
		for (int i = 0; i < snippets.Length; i++)
		{
			snippets[i].transform.localPosition -= Vector3.up * randomMoveSpeed * Time.deltaTime;
			if (snippets[i].transform.localPosition.y < -14f)
			{
				snippets[i].transform.localPosition = topSnippet.transform.localPosition + Vector3.up * 7f;
				topSnippet = snippets[i];
			}
		}
	}
}
