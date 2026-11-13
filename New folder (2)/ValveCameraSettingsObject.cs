using UnityEngine;

[CreateAssetMenu(fileName = "New Valve Camera Settings", menuName = "TenderClaws/Actions/Create Valve Settings Object", order = 1)]
public class ValveCameraSettingsObject : ScriptableObject
{
	[Header("Lights & Shadows")]
	[Range(1024f, 8192f)]
	public int _valveShadowTextureWidth = 4096;

	[Range(1024f, 8192f)]
	public int _valveShadowTextureHeight = 4096;

	[Tooltip("Requires 'Directional Specular' General GI Mode")]
	public bool _indirectLightmapsOnly;

	[Range(0f, 8f)]
	public int _MSAALevel = 4;

	public float _minRenderTargetScale = 0.8f;

	public float _maxRenderTargetScale = 1.4f;

	public int _maxRenderTargetDimension = 4096;

	[Header("Helper")]
	public bool _cullLightsInSceneEditor;

	public bool _cullLightsFromEditorCamera;

	public bool _hideAllValveMaterials;
}
