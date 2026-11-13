using UnityEngine;

[ExecuteInEditMode]
public class GvrControllerTooltipsSimple : MonoBehaviour, IGvrArmModelReceiver
{
	private MeshRenderer tooltipRenderer;

	private MaterialPropertyBlock materialPropertyBlock;

	private int colorId;

	public GvrBaseArmModel ArmModel { get; set; }

	private void Awake()
	{
		Initialize();
	}

	private void OnEnable()
	{
		GvrControllerInput.OnPostControllerInputUpdated += OnPostControllerInputUpdated;
	}

	private void OnDisable()
	{
		GvrControllerInput.OnPostControllerInputUpdated -= OnPostControllerInputUpdated;
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			Initialize();
			OnVisualUpdate();
		}
	}

	private void Initialize()
	{
		if (tooltipRenderer == null)
		{
			tooltipRenderer = GetComponent<MeshRenderer>();
		}
		if (materialPropertyBlock == null)
		{
			materialPropertyBlock = new MaterialPropertyBlock();
		}
		colorId = Shader.PropertyToID("_Color");
	}

	private void OnPostControllerInputUpdated()
	{
		OnVisualUpdate();
	}

	protected void OnVisualUpdate()
	{
		float a = ((!(ArmModel != null)) ? 1f : ArmModel.TooltipAlphaValue);
		materialPropertyBlock.SetColor(colorId, new Color(1f, 1f, 1f, a));
		tooltipRenderer.SetPropertyBlock(materialPropertyBlock);
	}
}
