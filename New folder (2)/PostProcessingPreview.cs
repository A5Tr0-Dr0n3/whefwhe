using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class PostProcessingPreview : MonoBehaviour
{
	public static PostProcessingPreview instance;

	public ValveCameraSettingsObject cameraSettings;

	public GameObject cameraRoot;

	internal PostProcessVolume mVolume;

	internal ValveCamera valveCamera;

	internal PostProcessLayer mLayer;

	private bool dontUpdateRoot;

	public void CleanUp()
	{
		instance = null;
		ValveCamera[] components = cameraRoot.GetComponents<ValveCamera>();
		for (int i = 0; i < components.Length; i++)
		{
			Object.DestroyImmediate(components[i]);
		}
		PostProcessLayer[] components2 = cameraRoot.GetComponents<PostProcessLayer>();
		for (int j = 0; j < components2.Length; j++)
		{
			Object.DestroyImmediate(components2[j]);
		}
		PostProcessVolume[] components3 = cameraRoot.GetComponents<PostProcessVolume>();
		for (int k = 0; k < components3.Length; k++)
		{
			components3[k].enabled = false;
			Object.DestroyImmediate(components3[k]);
		}
		dontUpdateRoot = true;
	}
}
