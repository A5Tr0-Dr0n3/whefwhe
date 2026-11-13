using TMPro;
using UnityEngine;

public class VacLabel : MonoBehaviour
{
	public enum labelStatus
	{
		unconsumable,
		unconsumableDenied,
		insufficient,
		transferring,
		fillOrConsume,
		consumeValue,
		fillValue
	}

	[HideInInspector]
	public VVRConsumable consumable;

	[HideInInspector]
	public Collider col;

	public bool tastiest;

	public Vector2 labelSize;

	public GameObject labelBack;

	public TextMeshPro labelText;

	public GameObject labelTextGO;

	public GameObject upGO;

	public GameObject downGO;

	public GameObject unconsumableGO;

	public Material LabelMat;

	public Color upColor;

	public Color downColor;

	public Color neutralUpColor;

	public Color neutralDownColor;

	public Color unconsumableColor;

	public MeshFilter stemMeshFilter;

	public MeshRenderer stemMeshRenderer;

	private Material stemMeshMat;

	private int stemPointPosId;

	private int colId;

	[SerializeField]
	private Vector3 defaultTextScale = Vector3.one;

	[SerializeField]
	private Vector3 insufficientTextScale = Vector3.one;

	public labelStatus currentLabelStatus;

	private float roundedValue1;

	private float roundedValue2;

	private EagerTouch.VacInputStatus workingTouchStatus;

	private void Awake()
	{
		CreateLabelStemMesh();
		stemMeshMat = new Material(stemMeshRenderer.sharedMaterial);
		stemMeshRenderer.sharedMaterial = stemMeshMat;
		stemPointPosId = Shader.PropertyToID("_LabelStemPointPos");
		colId = Shader.PropertyToID("_Color");
	}

	private void CreateLabelStemMesh()
	{
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[3];
		Vector2[] array2 = new Vector2[3];
		int[] array3 = new int[3];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, labelSize.y * 0.5f, 0f);
		ref Vector2 reference2 = ref array2[0];
		reference2 = new Vector2(0f, 1f);
		ref Vector3 reference3 = ref array[1];
		reference3 = new Vector3(0f, (0f - labelSize.y) * 0.5f, 0f);
		ref Vector2 reference4 = ref array2[1];
		reference4 = new Vector2(0f, 0f);
		ref Vector3 reference5 = ref array[2];
		reference5 = Vector3.zero;
		ref Vector2 reference6 = ref array2[2];
		reference6 = new Vector2(1f, 0f);
		array3[0] = 0;
		array3[1] = 2;
		array3[2] = 1;
		mesh.vertices = array;
		mesh.triangles = array3;
		mesh.uv = array2;
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5f);
		stemMeshFilter.sharedMesh = mesh;
	}

	public void SetStemEndPoint(Vector3 worldEndPoint)
	{
		if (stemMeshMat != null)
		{
			stemMeshMat.SetVector(stemPointPosId, transform.InverseTransformPoint(worldEndPoint));
		}
	}

	public void SetColor(Color color)
	{
		LabelMat.SetColor(colId, color);
		if (stemMeshMat != null)
		{
			stemMeshMat.SetColor(colId, color * 0.65f);
		}
	}

	public void SetLabelStatus(labelStatus newStatus)
	{
		switch (newStatus)
		{
		case labelStatus.unconsumable:
			unconsumableGO.SetActive(value: true);
			labelTextGO.SetActive(value: false);
			downGO.SetActive(value: false);
			upGO.SetActive(value: false);
			break;
		case labelStatus.unconsumableDenied:
			unconsumableGO.SetActive(value: true);
			labelTextGO.SetActive(value: false);
			downGO.SetActive(value: false);
			upGO.SetActive(value: false);
			break;
		case labelStatus.insufficient:
			labelText.text = "INSUFFICIENT";
			labelText.transform.localScale = insufficientTextScale;
			unconsumableGO.SetActive(value: false);
			labelTextGO.SetActive(value: true);
			downGO.SetActive(value: false);
			upGO.SetActive(value: false);
			break;
		case labelStatus.fillOrConsume:
			roundedValue1 = Mathf.Round(consumable.currentVacuumValueCapped);
			roundedValue2 = Mathf.Round(consumable.totalVacuumValue);
			labelText.text = roundedValue1 + "/" + roundedValue2 + " TB";
			labelText.transform.localScale = defaultTextScale;
			unconsumableGO.SetActive(value: false);
			labelTextGO.SetActive(value: true);
			downGO.SetActive(value: true);
			upGO.SetActive(value: true);
			break;
		case labelStatus.consumeValue:
			roundedValue1 = Mathf.Round(consumable.currentVacuumValueCapped);
			roundedValue2 = Mathf.Round(consumable.totalVacuumValue);
			labelText.text = roundedValue1 + "/" + roundedValue2 + " TB";
			labelText.transform.localScale = defaultTextScale;
			unconsumableGO.SetActive(value: false);
			labelTextGO.SetActive(value: true);
			downGO.SetActive(value: true);
			upGO.SetActive(value: false);
			break;
		case labelStatus.fillValue:
			roundedValue1 = Mathf.Round(consumable.currentVacuumValueCapped);
			roundedValue2 = Mathf.Round(consumable.totalVacuumValue);
			labelText.text = roundedValue1 + "/" + roundedValue2 + " TB";
			labelText.transform.localScale = defaultTextScale;
			unconsumableGO.SetActive(value: false);
			labelTextGO.SetActive(value: true);
			downGO.SetActive(value: false);
			upGO.SetActive(value: true);
			break;
		}
		currentLabelStatus = newStatus;
	}

	public void UpdateLabelColors(EagerTouch.VacInputStatus newTouchStatus, bool force = false)
	{
		if (workingTouchStatus == newTouchStatus && !force)
		{
			return;
		}
		switch (newTouchStatus)
		{
		case EagerTouch.VacInputStatus.Off:
			if (currentLabelStatus == labelStatus.fillValue)
			{
				SetColor(neutralUpColor);
			}
			else if (currentLabelStatus == labelStatus.consumeValue)
			{
				SetColor(neutralDownColor);
			}
			else
			{
				SetColor(unconsumableColor);
			}
			break;
		case EagerTouch.VacInputStatus.previewOut:
			if (currentLabelStatus == labelStatus.fillValue || currentLabelStatus == labelStatus.fillOrConsume)
			{
				SetColor(upColor);
			}
			else if (currentLabelStatus == labelStatus.consumeValue)
			{
				SetColor(neutralDownColor);
			}
			else
			{
				SetColor(unconsumableColor);
			}
			break;
		case EagerTouch.VacInputStatus.PreviewIn:
			if (currentLabelStatus == labelStatus.fillValue)
			{
				SetColor(neutralUpColor);
			}
			else if (currentLabelStatus == labelStatus.consumeValue || currentLabelStatus == labelStatus.fillOrConsume)
			{
				SetColor(downColor);
			}
			else
			{
				SetColor(unconsumableColor);
			}
			break;
		}
		workingTouchStatus = newTouchStatus;
	}
}
