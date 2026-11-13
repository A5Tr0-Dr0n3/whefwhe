using UnityEngine;

public class HoverButtonColorLerper : MonoBehaviour
{
	[SerializeField]
	private Color fromColor = Color.white;

	[SerializeField]
	private Color toColor = Color.white;

	[SerializeField]
	private ObjectButton mButton;

	[SerializeField]
	private MeshRenderer hoverRenderer;

	private void Awake()
	{
		hoverRenderer.sharedMaterial = Object.Instantiate(hoverRenderer.sharedMaterial);
		hoverRenderer.sharedMaterial.color = fromColor;
		mButton.onHoverEnter.AddListener(() =>
		{
			hoverRenderer.sharedMaterial.color = toColor * new Color(1f, 1f, 1f, hoverRenderer.sharedMaterial.color.a);
		});
		mButton.onHoverExit.AddListener(() =>
		{
			hoverRenderer.sharedMaterial.color = fromColor * new Color(1f, 1f, 1f, hoverRenderer.sharedMaterial.color.a);
		});
	}
}
