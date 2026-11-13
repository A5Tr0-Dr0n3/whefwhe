using UnityEngine;

namespace Gvr.Internal;

public class InstantPreview : MonoBehaviour
{
	public enum Resolutions
	{
		Big,
		Regular,
		WindowSized
	}

	private struct ResolutionSize
	{
		public int width;

		public int height;
	}

	public enum MultisampleCounts
	{
		One,
		Two,
		Four,
		Eight
	}

	public enum BitRates
	{
		_2000,
		_4000,
		_8000,
		_16000,
		_24000,
		_32000
	}

	private struct UnityRect
	{
		public float right;

		public float left;

		public float top;

		public float bottom;
	}

	private struct UnityEyeViews
	{
		public Matrix4x4 leftEyePose;

		public Matrix4x4 rightEyePose;

		public UnityRect leftEyeViewSize;

		public UnityRect rightEyeViewSize;
	}

	private const string NoDevicesFoundAdbResult = "error: no devices/emulators found";

	internal const string dllName = "instant_preview_unity_plugin";

	[Tooltip("Resolution of video stream. Higher = more expensive / better visual quality.")]
	public Resolutions OutputResolution;

	[Tooltip("Anti-aliasing for video preview. Higher = more expensive / better visual quality.")]
	public MultisampleCounts MultisampleCount;

	[Tooltip("Video codec streaming bit rate. Higher = more expensive / better visual quality.")]
	public BitRates BitRate = BitRates._16000;

	[Tooltip("Installs the Instant Preview app if it isn't found on the connected device.")]
	public bool InstallApkOnRun = true;

	public Object InstantPreviewApk;

	internal static InstantPreview Instance { get; set; }

	public bool IsCurrentlyConnected => false;
}
