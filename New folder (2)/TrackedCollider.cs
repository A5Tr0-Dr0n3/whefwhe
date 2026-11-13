using System;
using UnityEngine;

[Serializable]
public class TrackedCollider
{
	public Collider col;

	public bool originallyTrigger;

	public TrackedCollider(Collider s)
	{
		col = s;
		originallyTrigger = col.isTrigger;
	}
}
