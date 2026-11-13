using UnityEngine;

[RequireComponent(typeof(OVRRaycaster))]
public class OVRMousePointer : MonoBehaviour
{
	public enum MouseShowPolicy
	{
		always,
		withGaze,
		withActivity
	}

	[Tooltip("Period of inactivity before mouse disappears")]
	public float inactivityTimeout = 1f;

	[Tooltip("Policy regarding when mouse pointer should be shown")]
	public MouseShowPolicy mouseShowPolicy;

	[Tooltip("Should the mouse pointer being active cause the gaze pointer to fade")]
	public bool hideGazePointerWhenActive;

	[Tooltip("Move the pointer in response to mouse movement")]
	public bool defaultMouseMovement = true;

	public float mouseMoveSpeed = 5f;

	private float lastMouseActivityTime;

	private OVRRaycaster raycaster;

	public GameObject pointerObject => GetComponent<OVRRaycaster>().pointer;

	private void Awake()
	{
		raycaster = GetComponent<OVRRaycaster>();
	}

	private void Update()
	{
		if (mouseShowPolicy == MouseShowPolicy.withActivity)
		{
			pointerObject.SetActive(HasMovedRecently() && raycaster.IsFocussed());
		}
		else if (mouseShowPolicy == MouseShowPolicy.withGaze)
		{
			pointerObject.SetActive(raycaster.IsFocussed());
		}
		if (hideGazePointerWhenActive && HasMovedRecently() && raycaster.IsFocussed())
		{
			OVRGazePointer.instance.RequestHide();
		}
		if (defaultMouseMovement && raycaster.IsFocussed())
		{
			Vector2 vector = pointerObject.GetComponent<RectTransform>().localPosition;
			vector += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseMoveSpeed;
			float width = GetComponent<RectTransform>().rect.width;
			float height = GetComponent<RectTransform>().rect.height;
			vector.x = Mathf.Clamp(vector.x, (0f - width) / 2f, width / 2f);
			vector.y = Mathf.Clamp(vector.y, (0f - height) / 2f, height / 2f);
			SetLocalPosition(vector);
		}
	}

	public bool HasMovedRecently()
	{
		return lastMouseActivityTime + inactivityTimeout > Time.time;
	}

	public void SetLocalPosition(Vector3 pos)
	{
		if ((pointerObject.GetComponent<RectTransform>().localPosition - pos).magnitude > 0.001f)
		{
			lastMouseActivityTime = Time.time;
		}
		pointerObject.GetComponent<RectTransform>().localPosition = pos;
	}
}
