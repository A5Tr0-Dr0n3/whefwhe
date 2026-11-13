using System.Collections.Generic;
using UnityEngine;

public class ObjectRespawningDome : MonoBehaviour
{
	public bool reactToAllObjects;

	public List<ObjectBase> objectsToRespawn;

	public Vector3 objectRespawnPoint;

	public bool reactToEntry = true;

	public bool reactToStay = true;

	public bool reactToExit = true;

	public bool objectHasRespawned;

	public float respawnDelay;

	private float respawnTimer;

	private void OnTriggerStay(Collider c)
	{
		if (reactToStay)
		{
			TestObjectCollision(c.gameObject.GetComponent<ObjectBase>());
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		if (reactToEntry)
		{
			TestObjectCollision(c.gameObject.GetComponent<ObjectBase>());
		}
	}

	private void OnTriggerExit(Collider c)
	{
		if (reactToExit)
		{
			TestObjectCollision(c.gameObject.GetComponent<ObjectBase>());
		}
		respawnTimer = 0f;
	}

	private void TestObjectCollision(ObjectBase obj)
	{
		if (obj != null && !ToolManager.PlayerGrabbingObjectBase(obj))
		{
			respawnTimer += Time.deltaTime;
			if (respawnTimer > respawnDelay && (objectsToRespawn.Contains(obj) || reactToAllObjects))
			{
				obj.rb.velocity = Vector3.zero;
				obj.transform.position = objectRespawnPoint;
				obj.transform.rotation = Quaternion.identity;
				objectHasRespawned = true;
				respawnTimer = 0f;
			}
		}
	}
}
