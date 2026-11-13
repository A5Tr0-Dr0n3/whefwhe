using System.Collections;
using UnityEngine;

public class DelugeSnailInvader : ObjectBase
{
	public float moveTimeMin;

	public float moveTimeMax;

	public float moveSpeedMin = 5f;

	public float moveSpeedMax = 15f;

	private GameObject player;

	private WaitForSeconds snailWait;

	protected override void Start()
	{
		base.Start();
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}
		StartCoroutine(MoveSnail());
	}

	private IEnumerator MoveSnail()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			SafeDebug.Log("JUMP");
			rb.AddForce((player.transform.position - transform.position).normalized * Random.Range(moveSpeedMin, moveSpeedMax));
		}
	}
}
