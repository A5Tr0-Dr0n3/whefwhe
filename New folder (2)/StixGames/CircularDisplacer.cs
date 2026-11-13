using UnityEngine;

namespace StixGames;

[AddComponentMenu("StixGames/Circular Displacer")]
public class CircularDisplacer : MonoBehaviour
{
	public float pressureThreshold = 0.05f;

	public float maxAngle = 180f;

	public float radius = 0.5f;

	public bool directionalDisplacement;

	public float displacementStrength = 1f;

	public float displacementFalloff = 1f;

	public float pressureStrength = 1f;

	public float pressureFalloff = 1f;

	public Vector3 offset = Vector3.up;

	public Vector3 rayDirection = Vector3.down;

	public float maxDistance = 1.2f;

	public bool blendChanges;

	public float blendSpeed = 5f;

	public LayerMask grassLayer;

	private Vector3 lastPosition;

	private void Update()
	{
		RoundDisplacement();
		if (directionalDisplacement)
		{
			lastPosition = transform.position;
		}
	}

	private void RoundDisplacement()
	{
		Vector3 pos = transform.TransformPoint(offset);
		Transform worldToTextureSpaceMatrix = GrassManipulationUtility.GetWorldToTextureSpaceMatrix(pos, rayDirection, maxDistance, 0.1f, grassLayer, out var texCoord, out var forward, out var right);
		if (worldToTextureSpaceMatrix == null)
		{
			return;
		}
		Texture2D grassTexture = GrassManipulationUtility.GetGrassTexture(worldToTextureSpaceMatrix, getColorHeight: false);
		GrassManipulationUtility.Invert2x2Matrix(forward, right, out var inverseForward, out var inverseRight);
		inverseForward.Normalize();
		inverseRight.Normalize();
		int num = (int)(radius * forward.magnitude * (float)grassTexture.width);
		Vector2 vector = new Vector2(texCoord.x * (float)grassTexture.width, texCoord.y * (float)grassTexture.height);
		int num2 = (int)(vector.x - (float)num);
		int num3 = (int)(vector.y - (float)num);
		int num4 = Mathf.Clamp(num2, 0, grassTexture.width);
		int num5 = Mathf.Clamp(num3, 0, grassTexture.height);
		int num6 = Mathf.Min(num2 + num * 2, grassTexture.width) - num2;
		int num7 = Mathf.Min(num3 + num * 2, grassTexture.height) - num3;
		vector -= new Vector2(num4, num5);
		Color[] pixels = grassTexture.GetPixels(num4, num5, num6, num7);
		Vector3 forward2 = transform.forward;
		for (int i = 0; i < num7; i++)
		{
			for (int j = 0; j < num6; j++)
			{
				Color color = pixels[j + i * num6];
				Vector2 vector2 = (new Vector2(j, i) - vector) / num;
				vector2 = vector2.x * inverseRight + vector2.y * inverseForward;
				float b = 1f - pressureStrength * CalcFalloff(vector2.magnitude, pressureFalloff);
				Vector2 to = new Vector2(forward2.x, forward2.z);
				if (color.b > pressureThreshold && Vector2.Angle(vector2, to) < maxAngle)
				{
					Vector2 vector4;
					if (directionalDisplacement)
					{
						Vector3 vector3 = transform.position - lastPosition;
						vector4 = new Vector2(vector3.x, vector3.z);
					}
					else
					{
						vector4 = vector2;
					}
					float num8 = CalcFalloff(vector2.magnitude, displacementFalloff);
					vector2 = vector4.normalized * num8 * displacementStrength;
					vector2 = GrassManipulationUtility.VectorToColorSpace(vector2);
					Color color2 = new Color(vector2.x, vector2.y, b, 1f);
					ref Color reference = ref pixels[j + i * num6];
					reference = ((!blendChanges) ? color2 : Color.Lerp(pixels[j + i * num6], color2, Time.deltaTime * blendSpeed));
				}
			}
		}
		grassTexture.SetPixels(num4, num5, num6, num7, pixels);
		TextureUpdater textureUpdater = worldToTextureSpaceMatrix.GetComponent<TextureUpdater>();
		if (textureUpdater == null)
		{
			textureUpdater = worldToTextureSpaceMatrix.gameObject.AddComponent<TextureUpdater>();
			textureUpdater.targetTexture = grassTexture;
		}
		textureUpdater.RequestTextureUpdate();
	}

	private static float CalcFalloff(float distance, float falloff)
	{
		return Mathf.Pow(Mathf.Max(1f - distance, 0f), falloff);
	}
}
