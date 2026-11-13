using System;
using UnityEngine;

[Serializable]
public struct TrailPoint(Vector3 pos, float creationTime)
{
	public Vector3 pos = pos;

	public float creationTime = creationTime;
}
