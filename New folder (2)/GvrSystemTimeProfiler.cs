using System;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

public class GvrSystemTimeProfiler : MonoBehaviour
{
	[Tooltip("A reference to the game objects to be enabled / disabled.")]
	[SerializeField]
	private GameObject[] display;

	[Tooltip("A reference to the mesh filter which will display the profiler.")]
	[SerializeField]
	private MeshFilter mf;

	[Tooltip("A reference to the mesh that will display framerate.")]
	[SerializeField]
	private Mesh sourceMesh;

	[Tooltip("TextMesh to display renderscale.")]
	[SerializeField]
	private TextMesh renderScale;

	[Tooltip("TextMesh to display viewport scale.")]
	[SerializeField]
	private TextMesh viewportScale;

	private Color32[] colors;

	private Vector3[] vertices;

	private float[] times = new float[60];

	private int idx;

	private bool activated;

	private DateTime awakeTime;

	private double lastTime;

	private const float FPS_60 = 1f / 60f;

	private const float FPS_58 = 1f / 58f;

	private const float FPS_45 = 1f / 45f;

	private const float FPS_30 = 1f / 30f;

	private const float FPS_15 = 1f / 15f;

	private static readonly Color32 purple = new Color32(123, 22, 247, 200);

	private static readonly Color32 blue = new Color32(38, 5, 250, 200);

	private static readonly Color32 aqua = new Color32(5, 205, byte.MaxValue, 200);

	private static readonly Color32 green = new Color32(5, 252, 88, 200);

	private static readonly Color32 orange = new Color32(252, 225, 5, 200);

	private static readonly Color32 red = new Color32(252, 75, 5, 200);

	private static readonly Color32 black = new Color32(5, 5, 5, 200);

	private float lastRenderScale;

	private float lastViewportScale;

	private StringBuilder sb = new StringBuilder(50, 50);

	private char[] resolutionChars = new char[6];

	private const int CHAR_OFFSET = 48;

	public bool Activated => activated;

	public void IncreaseViewportScale()
	{
		float num = (XRSettings.renderViewportScale = Mathf.Clamp(XRSettings.renderViewportScale + 0.05f, 0.1f, 1f));
		lastViewportScale = num;
		UpdateText();
	}

	public void DecreaseViewportScale()
	{
		float num = (XRSettings.renderViewportScale = Mathf.Clamp(XRSettings.renderViewportScale - 0.05f, 0.1f, 1f));
		lastViewportScale = num;
		UpdateText();
	}

	public void DecreaseRenderScale()
	{
		float num = (XRSettings.eyeTextureResolutionScale = Mathf.Clamp(XRSettings.eyeTextureResolutionScale - 0.05f, 0.1f, 1f));
		lastRenderScale = num;
		UpdateText();
	}

	public void IncreaseRenderScale()
	{
		float num = (XRSettings.eyeTextureResolutionScale = Mathf.Clamp(XRSettings.eyeTextureResolutionScale + 0.05f, 0.1f, 1f));
		lastRenderScale = num;
		UpdateText();
	}

	public void ToggleMSAA()
	{
		if (QualitySettings.antiAliasing == 0)
		{
			QualitySettings.antiAliasing = 2;
		}
		else if (QualitySettings.antiAliasing == 2)
		{
			QualitySettings.antiAliasing = 4;
		}
		else
		{
			QualitySettings.antiAliasing = 0;
		}
		UpdateText();
	}

	public void EnableProfiler(bool force = false)
	{
		if (!activated || force)
		{
			activated = true;
			for (int i = 0; i < display.Length; i++)
			{
				display[i].SetActive(value: true);
			}
		}
	}

	public void DisableProfiler(bool force = false)
	{
		if (activated || force)
		{
			activated = false;
			for (int i = 0; i < display.Length; i++)
			{
				display[i].SetActive(value: false);
			}
		}
	}

	private void Awake()
	{
		awakeTime = DateTime.Now;
		vertices = sourceMesh.vertices;
		colors = new Color32[244];
		int num = 0;
		ref Color32 reference = ref colors[num];
		reference = black;
		ref Color32 reference2 = ref colors[num + 1];
		reference2 = black;
		ref Color32 reference3 = ref colors[num + 2];
		reference3 = black;
		ref Color32 reference4 = ref colors[num + 3];
		reference4 = black;
		lastViewportScale = XRSettings.renderViewportScale;
		lastRenderScale = XRSettings.eyeTextureResolutionScale;
		UpdateText();
	}

	private void Update()
	{
		if (activated)
		{
			double time = GetTime(awakeTime);
			float dt = (float)(time - lastTime);
			lastTime = time;
			UpdateGraph(dt, times, sourceMesh, colors, vertices, ref idx);
			if (lastViewportScale != XRSettings.renderViewportScale || lastRenderScale != XRSettings.eyeTextureResolutionScale)
			{
				lastViewportScale = XRSettings.renderViewportScale;
				lastRenderScale = XRSettings.eyeTextureResolutionScale;
				UpdateText();
			}
		}
	}

	private double GetTime(DateTime previous)
	{
		TimeSpan timeSpan = DateTime.Now - previous;
		return (double)timeSpan.Milliseconds * 0.001 + (double)timeSpan.Seconds + (double)(60 * timeSpan.Minutes) + (double)(3600 * (timeSpan.Hours + 24 * timeSpan.Days));
	}

	private int UShortToCharBuffer(ushort x, char[] buffer)
	{
		int num = 1;
		char c = (char)(x % 10 + 48);
		char c2 = '0';
		char c3 = '0';
		char c4 = '0';
		char c5 = '0';
		if (x > 9)
		{
			c2 = (char)(x / 10 % 10 + 48);
			num = 2;
		}
		if (x > 99)
		{
			c3 = (char)(x / 100 % 10 + 48);
			num = 3;
		}
		if (x > 999)
		{
			c4 = (char)(x / 1000 % 10 + 48);
			num = 4;
		}
		if (x > 9999)
		{
			c5 = (char)(x / 10000 % 10 + 48);
			num = 5;
		}
		if (num == 5)
		{
			buffer[0] = c5;
			buffer[1] = c4;
			buffer[2] = c3;
			buffer[3] = c2;
			buffer[4] = c;
		}
		if (num == 4)
		{
			buffer[0] = c4;
			buffer[1] = c3;
			buffer[2] = c2;
			buffer[3] = c;
		}
		if (num == 3)
		{
			buffer[0] = c3;
			buffer[1] = c2;
			buffer[2] = c;
		}
		if (num == 2)
		{
			buffer[0] = c2;
			buffer[1] = c;
		}
		if (num == 1)
		{
			buffer[0] = c;
		}
		return num;
	}

	private void FloatToCharBuffer(float x, char[] buffer)
	{
		int num = Mathf.RoundToInt(x * 100f);
		if (num > 99)
		{
			buffer[0] = (char)(num / 100 % 10 + 48);
		}
		else
		{
			buffer[0] = '0';
		}
		buffer[1] = '.';
		buffer[2] = (char)(num / 10 % 10 + 48);
		buffer[3] = (char)(num % 10 + 48);
	}

	private void UpdateText()
	{
		for (int i = 0; i < 4; i++)
		{
			resolutionChars[i] = '1';
		}
		sb.Length = 0;
		sb.Append("RES: ");
		FloatToCharBuffer(XRSettings.eyeTextureResolutionScale, resolutionChars);
		sb.Append(resolutionChars, 0, 4);
		sb.Append(" (");
		int charCount = UShortToCharBuffer((ushort)XRSettings.eyeTextureWidth, resolutionChars);
		sb.Append(resolutionChars, 0, charCount);
		sb.Append(" x ");
		charCount = UShortToCharBuffer((ushort)XRSettings.eyeTextureHeight, resolutionChars);
		sb.Append(resolutionChars, 0, charCount);
		if (QualitySettings.antiAliasing == 0)
		{
			sb.Append(") - MSAA: Off");
		}
		else if (QualitySettings.antiAliasing == 2)
		{
			sb.Append(") - MSAA: 2x");
		}
		else if (QualitySettings.antiAliasing == 4)
		{
			sb.Append(") - MSAA: 4x");
		}
		else if (QualitySettings.antiAliasing == 8)
		{
			sb.Append(") - MSAA: 8x");
		}
		renderScale.text = sb.ToString();
		sb.Length = 0;
		sb.Append("VIEW: ");
		float renderViewportScale = XRSettings.renderViewportScale;
		FloatToCharBuffer(renderViewportScale, resolutionChars);
		sb.Append(resolutionChars, 0, 4);
		sb.Append(" (");
		charCount = UShortToCharBuffer((ushort)(renderViewportScale * (float)XRSettings.eyeTextureWidth), resolutionChars);
		sb.Append(resolutionChars, 0, charCount);
		sb.Append(" x ");
		charCount = UShortToCharBuffer((ushort)(renderViewportScale * (float)XRSettings.eyeTextureHeight), resolutionChars);
		sb.Append(resolutionChars, 0, charCount);
		sb.Append(")");
		viewportScale.text = sb.ToString();
	}

	private static void UpdateGraph(float dt, float[] times, Mesh mesh, Color32[] colors, Vector3[] vertices, ref int idx)
	{
		idx = (idx - 1 + 60) % 60;
		times[idx] = dt;
		for (int i = 0; i < 60; i++)
		{
			int num = (i + idx) % 60;
			int num2 = (i + 1) * 4;
			Color32 color;
			if (times[num] < 1f / 60f)
			{
				color = purple;
			}
			else if (times[num] < 1f / 58f)
			{
				color = blue;
			}
			else if (times[num] < 1f / 45f)
			{
				color = aqua;
			}
			else if (times[num] < 1f / 30f)
			{
				color = green;
			}
			else
			{
				color = ((!(times[num] < 1f / 15f)) ? red : orange);
			}
			colors[num2] = color;
			colors[num2 + 1] = color;
			colors[num2 + 2] = color;
			colors[num2 + 3] = color;
			float y = Mathf.Clamp(120f * times[num], 0f, 4f);
			vertices[num2].y = y;
			vertices[num2 + 1].y = y;
		}
		mesh.vertices = vertices;
		mesh.colors32 = colors;
	}
}
