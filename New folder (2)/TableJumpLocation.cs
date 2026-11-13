using System;
using UnityEngine;

[Serializable]
public struct TableJumpLocation
{
	public Transform start;

	public BezierCurve curve;
}
