using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR;

namespace Gvr.Internal;

public class AndroidNativeKeyboardProvider : IKeyboardProvider
{
	private struct gvr_clock_time_point
	{
		public long monotonic_system_time_nanos;
	}

	private struct gvr_recti
	{
		public int left;

		public int right;

		public int bottom;

		public int top;
	}

	private IntPtr renderEventFunction;

	private const string METHOD_NAME_GET_PACKAGE_MANAGER = "getPackageManager";

	private const string METHOD_NAME_GET_PACKAGE_INFO = "getPackageInfo";

	private const string PACKAGE_NAME_VRINPUTMETHOD = "com.google.android.vr.inputmethod";

	private const string FIELD_NAME_VERSION_CODE = "versionCode";

	private const int MIN_VERSION_VRINPUTMETHOD = 170509062;

	private const string dllName = "gvr_keyboard_shim_unity";

	private const int TRIGGER_NONE = 0;

	private const int TRIGGER_PRESSED = 1;

	private IntPtr keyboard_context = IntPtr.Zero;

	private const int advanceID = 1571584315;

	private const int renderLeftID = 1022982717;

	private const int renderRightID = 1022982718;

	private const string KEYBOARD_JAVA_CLASS = "com.google.vr.keyboard.GvrKeyboardUnity";

	private const long kPredictionTimeWithoutVsyncNanos = 50000000L;

	private const int kGvrControllerButtonClick = 1;

	private GvrKeyboardInputMode mode;

	private string editorText = string.Empty;

	private Matrix4x4 worldMatrix;

	private bool isValid;

	private bool isReady;

	public string EditorText
	{
		get
		{
			IntPtr ptr = gvr_keyboard_get_text(keyboard_context);
			editorText = Marshal.PtrToStringAnsi(ptr);
			return editorText;
		}
		set
		{
			editorText = value;
			IntPtr edit_text = Marshal.StringToHGlobalAnsi(editorText);
			gvr_keyboard_set_text(keyboard_context, edit_text);
		}
	}

	public AndroidNativeKeyboardProvider()
	{
		InputTracking.disablePositionalTracking = true;
		renderEventFunction = GetKeyboardRenderEventFunc();
	}

	[DllImport("gvr")]
	private static extern gvr_clock_time_point gvr_get_time_point_now();

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern GvrKeyboardInputMode gvr_keyboard_get_input_mode(IntPtr keyboard_context);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void gvr_keyboard_set_input_mode(IntPtr keyboard_context, GvrKeyboardInputMode mode);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern IntPtr gvr_keyboard_create(IntPtr closure, GvrKeyboard.KeyboardCallback callback);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void gvr_keyboard_get_recommended_world_from_keyboard_matrix(float distance_from_eye, IntPtr matrix);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void gvr_keyboard_set_world_from_keyboard_matrix(IntPtr keyboard_context, IntPtr matrix);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void gvr_keyboard_show(IntPtr keyboard_context);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void gvr_keyboard_update_button_state(IntPtr keyboard_context, int buttonIndex, bool pressed);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern bool gvr_keyboard_update_controller_ray(IntPtr keyboard_context, IntPtr vector3Start, IntPtr vector3End, IntPtr vector3Hit);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern IntPtr gvr_keyboard_get_text(IntPtr keyboard_context);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern int gvr_keyboard_set_text(IntPtr keyboard_context, IntPtr edit_text);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void gvr_keyboard_hide(IntPtr keyboard_context);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void gvr_keyboard_destroy(IntPtr keyboard_context);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void GvrKeyboardSetFrameData(IntPtr keyboard_context, gvr_clock_time_point t);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern void GvrKeyboardSetEyeData(int eye_type, Matrix4x4 modelview, Matrix4x4 projection, gvr_recti viewport);

	[DllImport("gvr_keyboard_shim_unity")]
	private static extern IntPtr GetKeyboardRenderEventFunc();

	public void SetInputMode(GvrKeyboardInputMode mode)
	{
		Debug.Log("Calling set input mode: " + mode);
		gvr_keyboard_set_input_mode(keyboard_context, mode);
		this.mode = mode;
	}

	public void OnPause()
	{
	}

	public void OnResume()
	{
	}

	public void ReadState(KeyboardState outState)
	{
		outState.editorText = editorText;
		outState.mode = mode;
		outState.worldMatrix = worldMatrix;
		outState.isValid = isValid;
		outState.isReady = isReady;
	}

	~AndroidNativeKeyboardProvider()
	{
		if (keyboard_context != IntPtr.Zero)
		{
			gvr_keyboard_destroy(keyboard_context);
		}
	}

	public bool Create(GvrKeyboard.KeyboardCallback keyboardEvent)
	{
		if (!IsVrInputMethodAppMinVersion(keyboardEvent))
		{
			return false;
		}
		keyboard_context = gvr_keyboard_create(IntPtr.Zero, keyboardEvent);
		isReady = keyboard_context != IntPtr.Zero;
		return isReady;
	}

	public void Show(Matrix4x4 userMatrix, bool useRecommended, float distance, Matrix4x4 model)
	{
		if (useRecommended)
		{
			worldMatrix = getRecommendedMatrix(distance);
		}
		else
		{
			worldMatrix = Pose3D.FlipHandedness(userMatrix).transpose;
		}
		Matrix4x4 matrix4x = worldMatrix * model.transpose;
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(matrix4x));
		Marshal.StructureToPtr(matrix4x, intPtr, fDeleteOld: true);
		gvr_keyboard_set_world_from_keyboard_matrix(keyboard_context, intPtr);
		gvr_keyboard_show(keyboard_context);
	}

	public void UpdateData()
	{
		gvr_clock_time_point t = gvr_get_time_point_now();
		t.monotonic_system_time_nanos += 50000000L;
		GvrKeyboardSetFrameData(keyboard_context, t);
		GL.IssuePluginEvent(renderEventFunction, 1571584315);
	}

	public void Render(int eye, Matrix4x4 modelview, Matrix4x4 projection, Rect viewport)
	{
		GvrKeyboardSetEyeData(viewport: new gvr_recti
		{
			left = (int)viewport.x,
			top = (int)viewport.y + (int)viewport.height,
			right = (int)viewport.x + (int)viewport.width,
			bottom = (int)viewport.y
		}, eye_type: eye, modelview: (Pose3D.FLIP_Z * modelview.inverse).transpose.inverse, projection: projection.transpose);
		GL.IssuePluginEvent(renderEventFunction, (eye != 0) ? 1022982718 : 1022982717);
	}

	public void Hide()
	{
		gvr_keyboard_hide(keyboard_context);
	}

	private Matrix4x4 getRecommendedMatrix(float inputDistance)
	{
		float distance_from_eye = Mathf.Clamp(inputDistance, 1f, 5f);
		Matrix4x4 matrix4x = default;
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(matrix4x));
		Marshal.StructureToPtr(matrix4x, intPtr, fDeleteOld: true);
		gvr_keyboard_get_recommended_world_from_keyboard_matrix(distance_from_eye, intPtr);
		return (Matrix4x4)Marshal.PtrToStructure(intPtr, typeof(Matrix4x4));
	}

	private bool IsVrInputMethodAppMinVersion(GvrKeyboard.KeyboardCallback keyboardEvent)
	{
		return true;
	}
}
