using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
	[SerializeField]
	private CursorReticle _cursorReticle;

	public GameObject _startPosition;

	[HideInInspector]
	public Vector3 endPosition;

	private GameObject[] points;

	[Range(2f, 100f)]
	public int _numberOfPoints = 60;

	public GameObject[] ropeObjects;

	private RaycastHit hitInfo;

	[HideInInspector]
	public Vector3 hitPoint;

	[HideInInspector]
	public Vector3 endTangent;

	public LineRenderer line;

	private float s;

	private Vector3 p = Vector3.zero;

	[HideInInspector]
	public float ropeLength;

	public float lineWidthSpeed = 1.5f;

	public bool useLineRenderer;

	private MaterialPropertyBlock emtpyBlock;

	private float lastLineConstant;

	private float distanceLerp;

	public float maxScalingDist = 10f;

	public float scaleAtMaxDist = 0.2f;

	private float myScaleConstant = 1f;

	private GameObject pointsParent;

	private float spacing = 0.05f;

	public List<float> pointDistList = new List<float>();

	public List<Vector3> pointList = new List<Vector3>();

	public float lineWidth = 0.02f;

	public GameObject lineTip;

	private float currentLineWidth;

	private bool lineState = true;

	public bool activeLine
	{
		get
		{
			return lineState;
		}
		set
		{
			ToggleLine(value);
			lineState = value;
		}
	}

	private void Awake()
	{
		InitializeLine();
		SpawnPoints(_numberOfPoints);
		emtpyBlock = new MaterialPropertyBlock();
	}

	private void Update()
	{
		if (useLineRenderer && (points == null || points.Length != _numberOfPoints))
		{
			SpawnPoints(_numberOfPoints);
		}
		else if (!useLineRenderer && line.enabled)
		{
			line.enabled = false;
		}
		float num = ((!PlayerScaler.instance.scaled) ? 1f : (PlayerScaler.instance.currentScale.x * 0.5f));
		if (!lineState && line.enabled)
		{
			if (currentLineWidth < 0.001f)
			{
				line.enabled = false;
			}
			currentLineWidth = Mathf.Lerp(currentLineWidth, 0f, lineWidthSpeed * Time.deltaTime);
			line.startWidth = currentLineWidth * num;
			line.endWidth = currentLineWidth * lastLineConstant * num;
		}
		else if (lineState)
		{
			if (!line.enabled)
			{
				line.enabled = true;
			}
			distanceLerp = Mathf.InverseLerp(0f, maxScalingDist, _cursorReticle.dist);
			myScaleConstant = Mathf.Clamp(_cursorReticle.scaleConstant * Mathf.Lerp(1f, scaleAtMaxDist, distanceLerp), 0.5f, 99999f);
			currentLineWidth = Mathf.Lerp(currentLineWidth, lineWidth, lineWidthSpeed * Time.deltaTime);
			line.startWidth = currentLineWidth * num;
			line.endWidth = currentLineWidth * myScaleConstant * num;
			lastLineConstant = myScaleConstant;
		}
	}

	private void SpawnPoints(int numberOfPoints)
	{
		if (points != null)
		{
			GameObject[] array = points;
			foreach (GameObject obj in array)
			{
				Object.Destroy(obj);
			}
		}
		points = new GameObject[numberOfPoints];
		pointsParent = new GameObject();
		pointsParent.name = "PointsParent";
		SceneManager.MoveGameObjectToScene(pointsParent, SceneManager.GetSceneByName("VVR_ROOT"));
		for (int j = 0; j < numberOfPoints; j++)
		{
			if (points[j] == null)
			{
				if (ropeObjects.Length > 0 && !useLineRenderer)
				{
					points[j] = Object.Instantiate(ropeObjects[Random.Range(0, ropeObjects.Length)]);
				}
				else
				{
					points[j] = new GameObject();
				}
				points[j].name = "Point" + (j + 1);
				points[j].transform.SetParent(pointsParent.transform);
			}
		}
	}

	private void InitializeLine()
	{
		line = gameObject.GetComponent<LineRenderer>();
		line.startWidth = lineWidth;
		line.endWidth = lineWidth;
		line.positionCount = _numberOfPoints;
		lineTip.SetActive(value: false);
		for (int i = 0; i < _numberOfPoints; i++)
		{
			line.SetPosition(i, Vector3.zero);
		}
	}

	private void DrawHermiteCurve(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int steps)
	{
		for (int i = 0; i < steps; i++)
		{
			s = (float)i / (float)steps;
			float num = 2f * Mathf.Pow(s, 3f) - 3f * Mathf.Pow(s, 2f) + 1f;
			float num2 = -2f * Mathf.Pow(s, 3f) + 3f * Mathf.Pow(s, 2f);
			float num3 = Mathf.Pow(s, 3f) - 2f * Mathf.Pow(s, 2f) + s;
			float num4 = Mathf.Pow(s, 3f) - Mathf.Pow(s, 2f);
			Vector3 position = num * startPosition + num2 * endPosition + num3 * startTangent + num4 * endTangent;
			points[i].transform.position = position;
		}
	}

	public void DrawBezier(Vector3 startPosition, Vector3 endTangent, Vector3 endPosition, int steps)
	{
		line.positionCount = steps + 1;
		ropeLength = 0f;
		spacing = ropeLength / (float)steps;
		pointList.Clear();
		pointDistList.Clear();
		for (int i = 0; i < steps; i++)
		{
			s = (float)i / (float)steps;
			Vector3 b = p;
			p = Mathf.Pow(1f - s, 2f) * startPosition + 2f * s * (1f - s) * endTangent + Mathf.Pow(s, 2f) * endPosition;
			float num = 0f;
			if (i > 0)
			{
				num = Vector3.Distance(p, b);
				ropeLength += num;
				pointList.Add(p);
				pointDistList.Add(ropeLength);
			}
			else
			{
				num = Vector3.Distance(startPosition, p);
				ropeLength += num;
				pointList.Add(p);
				pointDistList.Add(0f);
			}
			line.SetPosition(i, p);
		}
		line.SetPosition(steps, endPosition);
		for (int j = 0; j < steps; j++)
		{
			points[j].transform.position = pointAlongCurveAt(spacing);
			spacing += ropeLength / (float)steps;
		}
	}

	private Vector3 pointAlongCurveAt(float distanceIn)
	{
		Vector3 result = Vector3.zero;
		for (int i = 1; i < pointDistList.Count; i++)
		{
			if (pointDistList[i] > distanceIn)
			{
				result = pointList[i - 1];
				float num = (distanceIn - pointDistList[i - 1]) / (pointDistList[i] - pointDistList[i - 1]);
				result += (pointList[i] - pointList[i - 1]) * num;
				break;
			}
		}
		return result;
	}

	public void SetMaterial(Material mat)
	{
		line.sharedMaterial = mat;
		line.SetPropertyBlock(emtpyBlock);
	}

	public Material GetCurrentMaterial()
	{
		return line.sharedMaterial;
	}

	public void ToggleLine(bool flag)
	{
		lineState = flag;
		points[0].transform.parent.gameObject.SetActive(flag);
		lineTip.SetActive(flag);
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(hitPoint, endTangent);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(_startPosition.transform.position, endPosition);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(_startPosition.transform.position, endTangent);
			Gizmos.color = Color.blue;
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(_startPosition.transform.position, 0.5f);
		}
	}
}
