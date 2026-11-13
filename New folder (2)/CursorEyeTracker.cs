using System.Collections.Generic;
using UnityEngine;

public class CursorEyeTracker : MonoBehaviour
{
	public float xMin;

	public float xMax;

	public float yMin;

	public float yMax;

	public float zMin;

	public float zMax;

	[SerializeField]
	private float _switchThreshold = 0.1f;

	private const int N = 3;

	private List<Vector3> _cursorLastPositions = new List<Vector3>(2);

	private List<float> _cursorSpeedExpMATimesN = new List<float>(2);

	private Transform _tracked;

	private void Update()
	{
		float num = 0f;
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (i >= _cursorSpeedExpMATimesN.Count)
			{
				_cursorSpeedExpMATimesN.Add(0f);
				_cursorLastPositions.Add(ToolManager.ActiveToolGroups[i].CursorReticle.CursorTransform.position);
			}
			float sqrMagnitude = (ToolManager.ActiveToolGroups[i].CursorReticle.CursorTransform.position - _cursorLastPositions[i]).sqrMagnitude;
			_cursorSpeedExpMATimesN[i] = _cursorSpeedExpMATimesN[i] + sqrMagnitude - _cursorSpeedExpMATimesN[i] / 3f;
			float num2 = _cursorSpeedExpMATimesN[i] / 3f;
			if ((num2 > num && num > _switchThreshold) || _tracked == null)
			{
				num = num2;
				_tracked = ToolManager.ActiveToolGroups[i].CursorReticle.CursorTransform;
			}
		}
		if (_tracked != null)
		{
			transform.position = new Vector3(Mathf.Clamp(_tracked.position.x, xMin, xMax), Mathf.Clamp(_tracked.position.y, yMin, yMax), Mathf.Clamp(_tracked.position.z, zMin, zMax));
		}
	}
}
