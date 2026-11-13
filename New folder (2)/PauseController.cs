using UnityEngine;

public class PauseController : MonoBehaviour
{
	[SerializeField]
	private Material normalMaterial;

	[SerializeField]
	private Material highlightMaterial;

	[SerializeField]
	private Renderer appButtonMesh;

	[SerializeField]
	private Renderer trackpadMesh;

	public void HighlightControls(bool highlight)
	{
		appButtonMesh.sharedMaterial = ((!highlight) ? normalMaterial : highlightMaterial);
		trackpadMesh.sharedMaterial = ((!highlight) ? normalMaterial : highlightMaterial);
	}
}
