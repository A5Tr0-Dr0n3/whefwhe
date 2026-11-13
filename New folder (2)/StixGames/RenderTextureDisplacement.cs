using System.Collections;
using UnityEngine;

namespace StixGames;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("StixGames/Render Texture Displacement")]
public class RenderTextureDisplacement : MonoBehaviour
{
	public float borderSmoothingArea = 2f;

	private Camera cam;

	private void OnEnable()
	{
		cam = GetComponent<Camera>();
		if (!cam.orthographic)
		{
			Debug.LogError("Your displacment camera should be in orthographic mode, or grass displacement will look strange.");
		}
		if (cam.targetTexture == null)
		{
			Debug.LogError("Your displacment camera needs a render texture, or grass displacement will not work.");
		}
		cam.backgroundColor = new Color(0f, 0f, 1f, 1f);
		cam.clearFlags = CameraClearFlags.Color;
		Shader.EnableKeyword("GRASS_RENDERTEXTURE_DISPLACEMENT");
		Shader.SetGlobalTexture("_GrassRenderTextureDisplacement", cam.targetTexture);
	}

	private void OnDisable()
	{
		Shader.DisableKeyword("GRASS_RENDERTEXTURE_DISPLACEMENT");
	}

	private IEnumerator OnPreCull()
	{
		Shader.SetGlobalFloat("_GrassDisplacementBorderArea", borderSmoothingArea);
		Vector3 realPosition = transform.position;
		float pixelWidth = 2f * cam.orthographicSize / (float)cam.pixelWidth;
		float pixelHeight = 2f * cam.orthographicSize / (float)cam.pixelHeight;
		Vector3 pos = realPosition;
		pos.x -= pos.x % pixelWidth;
		pos.z -= pos.z % pixelHeight;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		Vector3 bottomLeft = cam.ScreenToWorldPoint(Vector3.zero);
		Vector4 renderArea = new Vector4(z: 2f * cam.orthographicSize * cam.aspect, w: 2f * cam.orthographicSize / cam.aspect, x: bottomLeft.x, y: bottomLeft.z);
		Shader.SetGlobalVector("_GrassRenderTextureArea", renderArea);
		yield return new WaitForEndOfFrame();
		transform.position = realPosition;
	}
}
