using UnityEngine;

public class BakeTextureTest : MonoBehaviour
{
	public Texture2D Texture;

	public int TextureResolution;

	public float NoiseScale;

	public float Z;

	public Material NoiseMat;

	public void BakeTexture()
	{
		Texture = new Texture2D(TextureResolution, TextureResolution, TextureFormat.RGBA32, mipmap: false, linear: true);
		for (int i = 0; i < TextureResolution; i++)
		{
			for (int j = 0; j < TextureResolution; j++)
			{
				float x = (float)(j - TextureResolution / 2) * NoiseScale;
				float y = (float)(i - TextureResolution / 2) * NoiseScale;
				float num = noise(new Vector3(x, y, Z));
				Texture.SetPixel(j, i, new Color(num, num, num, 1f));
			}
		}
		Texture.Apply();
		NoiseMat.SetTexture("_MainTex", Texture);
	}

	private float hash(float n)
	{
		return frac(Mathf.Sin(n) * 43758.547f);
	}

	private float frac(float n)
	{
		if (n < 0f)
		{
			n *= -1f;
		}
		return n % 1f;
	}

	private float noise(Vector3 x)
	{
		Vector3 vector = new Vector3(Mathf.Floor(x.x), Mathf.Floor(x.y), Mathf.Floor(x.z));
		Vector3 vector2 = new Vector3(frac(x.x), frac(x.y), frac(x.z));
		vector2 = new Vector3(vector2.x * vector2.x * (3f - 2f * vector2.x), vector2.y * vector2.y * (3f - 2f * vector2.y), vector2.z * vector2.z * (3f - 2f * vector2.z));
		float num = vector.x + vector.y * 57f + 113f * vector.z;
		return Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(hash(num), hash(num + 1f), vector2.x), Mathf.Lerp(hash(num + 57f), hash(num + 58f), vector2.x), vector2.y), Mathf.Lerp(Mathf.Lerp(hash(num + 113f), hash(num + 114f), vector2.x), Mathf.Lerp(hash(num + 170f), hash(num + 171f), vector2.x), vector2.y), vector2.z);
	}
}
