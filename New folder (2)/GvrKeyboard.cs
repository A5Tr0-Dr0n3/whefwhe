using System;
using System.Collections;
using System.Collections.Generic;
using Gvr.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

public class GvrKeyboard : MonoBehaviour
{
	public delegate void StandardCallback();

	public delegate void EditTextCallback(string edit_text);

	public delegate void ErrorCallback(GvrKeyboardError err);

	public delegate void KeyboardCallback(IntPtr closure, GvrKeyboardEvent evt);

	private static GvrKeyboard instance;

	private static IKeyboardProvider keyboardProvider;

	private KeyboardState keyboardState = new KeyboardState();

	private IEnumerator keyboardUpdate;

	private ErrorCallback errorCallback;

	private StandardCallback showCallback;

	private StandardCallback hideCallback;

	private EditTextCallback updateCallback;

	private EditTextCallback enterCallback;

	private bool isKeyboardHidden;

	private const float kExecuterWait = 0.01f;

	private static List<GvrKeyboardEvent> threadSafeCallbacks = new List<GvrKeyboardEvent>();

	private static object callbacksLock = new object();

	public GvrKeyboardDelegateBase keyboardDelegate;

	public GvrKeyboardInputMode inputMode;

	public bool useRecommended = true;

	public float distance;

	public string EditorText => (!(instance != null)) ? string.Empty : instance.keyboardState.editorText;

	public GvrKeyboardInputMode Mode => (instance != null) ? instance.keyboardState.mode : GvrKeyboardInputMode.DEFAULT;

	public bool IsValid => instance != null && instance.keyboardState.isValid;

	public bool IsReady => instance != null && instance.keyboardState.isReady;

	public Matrix4x4 WorldMatrix => (!(instance != null)) ? Matrix4x4.zero : instance.keyboardState.worldMatrix;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one GvrKeyboard instance was found in your scene. Ensure that there is only one GvrKeyboard.");
			enabled = false;
			return;
		}
		instance = this;
		if (keyboardProvider == null)
		{
			keyboardProvider = KeyboardProviderFactory.CreateKeyboardProvider(this);
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Start()
	{
		if (keyboardDelegate != null)
		{
			errorCallback = keyboardDelegate.OnKeyboardError;
			showCallback = keyboardDelegate.OnKeyboardShow;
			hideCallback = keyboardDelegate.OnKeyboardHide;
			updateCallback = keyboardDelegate.OnKeyboardUpdate;
			enterCallback = keyboardDelegate.OnKeyboardEnterPressed;
			keyboardDelegate.KeyboardHidden += KeyboardDelegate_KeyboardHidden;
			keyboardDelegate.KeyboardShown += KeyboardDelegate_KeyboardShown;
		}
		keyboardProvider.ReadState(keyboardState);
		if (IsValid)
		{
			if (keyboardProvider.Create(OnKeyboardCallback))
			{
				keyboardProvider.SetInputMode(inputMode);
			}
		}
		else
		{
			Debug.LogError("Could not validate keyboard");
		}
	}

	private void Update()
	{
		if (keyboardProvider == null)
		{
			return;
		}
		keyboardProvider.ReadState(keyboardState);
		if (IsReady)
		{
			if (transform.hasChanged)
			{
				Show();
				transform.hasChanged = false;
			}
			keyboardProvider.UpdateData();
		}
	}

	private void OnRenderObject()
	{
		if (keyboardProvider != null && IsReady)
		{
		}
	}

	public void ClearText()
	{
		if (keyboardProvider != null)
		{
			keyboardProvider.EditorText = string.Empty;
		}
	}

	public void Show()
	{
		if (keyboardProvider == null)
		{
			return;
		}
		Quaternion q = new Quaternion(transform.rotation.x * -1f, transform.rotation.y * -1f, transform.rotation.z, transform.rotation.w);
		Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z * -1f);
		Matrix4x4 model = Matrix4x4.TRS(pos, q, Vector3.one);
		Matrix4x4 controllerMatrix = Matrix4x4.identity;
		Vector3 position = gameObject.transform.position;
		if (position.x == 0f && position.y == 0f && position.z == 0f && !useRecommended)
		{
			keyboardProvider.Show(controllerMatrix, useRecommended: true, distance, model);
			return;
		}
		if (!useRecommended)
		{
			controllerMatrix = GetKeyboardObjectMatrix(position);
		}
		keyboardProvider.Show(controllerMatrix, useRecommended, distance, model);
	}

	public void Hide()
	{
		if (keyboardProvider != null)
		{
			keyboardProvider.Hide();
		}
	}

	public void OnPointerClick(BaseEventData data)
	{
		if (isKeyboardHidden)
		{
			Show();
		}
	}

	private void OnEnable()
	{
		keyboardUpdate = Executer();
		StartCoroutine(keyboardUpdate);
	}

	private void OnDisable()
	{
		StopCoroutine(keyboardUpdate);
	}

	private void OnApplicationPause(bool paused)
	{
		if (keyboardProvider != null)
		{
			if (paused)
			{
				keyboardProvider.OnPause();
			}
			else
			{
				keyboardProvider.OnResume();
			}
		}
	}

	private IEnumerator Executer()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.01f);
			while (threadSafeCallbacks.Count > 0)
			{
				GvrKeyboardEvent keyboardEvent = threadSafeCallbacks[0];
				PoolKeyboardCallbacks(keyboardEvent);
				lock (callbacksLock)
				{
					threadSafeCallbacks.RemoveAt(0);
				}
			}
		}
	}

	private void PoolKeyboardCallbacks(GvrKeyboardEvent keyboardEvent)
	{
		switch (keyboardEvent)
		{
		case GvrKeyboardEvent.GVR_KEYBOARD_ERROR_UNKNOWN:
			errorCallback(GvrKeyboardError.UNKNOWN);
			break;
		case GvrKeyboardEvent.GVR_KEYBOARD_ERROR_SERVICE_NOT_CONNECTED:
			errorCallback(GvrKeyboardError.SERVICE_NOT_CONNECTED);
			break;
		case GvrKeyboardEvent.GVR_KEYBOARD_ERROR_NO_LOCALES_FOUND:
			errorCallback(GvrKeyboardError.NO_LOCALES_FOUND);
			break;
		case GvrKeyboardEvent.GVR_KEYBOARD_ERROR_SDK_LOAD_FAILED:
			errorCallback(GvrKeyboardError.SDK_LOAD_FAILED);
			break;
		case GvrKeyboardEvent.GVR_KEYBOARD_SHOWN:
			showCallback();
			break;
		case GvrKeyboardEvent.GVR_KEYBOARD_HIDDEN:
			hideCallback();
			break;
		case GvrKeyboardEvent.GVR_KEYBOARD_TEXT_UPDATED:
			updateCallback(keyboardProvider.EditorText);
			break;
		case GvrKeyboardEvent.GVR_KEYBOARD_TEXT_COMMITTED:
			enterCallback(keyboardProvider.EditorText);
			break;
		}
	}

	private static void OnKeyboardCallback(IntPtr closure, GvrKeyboardEvent keyboardEvent)
	{
		lock (callbacksLock)
		{
			threadSafeCallbacks.Add(keyboardEvent);
		}
	}

	private void KeyboardDelegate_KeyboardShown(object sender, EventArgs e)
	{
		isKeyboardHidden = false;
	}

	private void KeyboardDelegate_KeyboardHidden(object sender, EventArgs e)
	{
		isKeyboardHidden = true;
	}

	private Matrix4x4 GetKeyboardObjectMatrix(Vector3 position)
	{
		float f = Mathf.Atan2(position.y, position.x);
		float num = Mathf.Tan(f);
		float x = num * position.x;
		float f2 = Mathf.Atan2(position.x, position.y);
		float num2 = Mathf.Tan(f2);
		float y = num2 * position.y;
		float f3 = Mathf.Atan2(position.y, position.z);
		float num3 = Mathf.Tan(f3);
		float z = num3 * position.z;
		Vector3 pos = new Vector3(x, y, z);
		Vector3 position2 = Camera.main.transform.position;
		Quaternion q = Quaternion.LookRotation(position2);
		Matrix4x4 result = default;
		result.SetTRS(pos, q, position);
		if (result[0, 0] == 0f)
		{
			Vector4 row = result.GetRow(0);
			result.SetRow(0, new Vector4(1f, row.y, row.z, row.w));
		}
		if (result[1, 1] == 0f)
		{
			Vector4 row2 = result.GetRow(1);
			result.SetRow(1, new Vector4(row2.x, 1f, row2.z, row2.w));
		}
		if (result[2, 2] == 0f)
		{
			Vector4 row3 = result.GetRow(2);
			result.SetRow(2, new Vector4(row3.x, row3.y, 1f, row3.w));
		}
		return result;
	}
}
