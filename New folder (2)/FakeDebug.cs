using TButt;
using TMPro;
using UnityEngine;

public class FakeDebug : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro gyroX;

	[SerializeField]
	private TextMeshPro gyroY;

	[SerializeField]
	private TextMeshPro gyroZ;

	[SerializeField]
	private TextMeshPro accelX;

	[SerializeField]
	private TextMeshPro accelY;

	[SerializeField]
	private TextMeshPro accelZ;

	[SerializeField]
	private GameObject gX;

	[SerializeField]
	private GameObject gY;

	[SerializeField]
	private GameObject gZ;

	[SerializeField]
	private GameObject accelController;

	[SerializeField]
	private TextMeshPro app;

	[SerializeField]
	private TextMeshPro click;

	[SerializeField]
	private TextMeshPro touch;

	[SerializeField]
	private TextMeshPro touchX;

	[SerializeField]
	private TextMeshPro touchY;

	[SerializeField]
	private GameObject touchController;

	[SerializeField]
	private GameObject headset;

	[SerializeField]
	private TextMeshPro headsetX;

	[SerializeField]
	private TextMeshPro headsetY;

	[SerializeField]
	private TextMeshPro headsetZ;

	private float accelMult = 0.05f;

	private void Update()
	{
		gyroX.text = TBInput.GetRotation(TBInput.Controller.Active).eulerAngles.x + string.Empty;
		gyroY.text = TBInput.GetRotation(TBInput.Controller.Active).eulerAngles.y + string.Empty;
		gyroZ.text = TBInput.GetRotation(TBInput.Controller.Active).eulerAngles.z + string.Empty;
		gX.transform.localRotation = Quaternion.Euler(Vector3.right * TBInput.GetRotation(TBInput.Controller.Active).eulerAngles.x);
		gY.transform.localRotation = Quaternion.Euler(Vector3.up * TBInput.GetRotation(TBInput.Controller.Active).eulerAngles.y);
		gZ.transform.localRotation = Quaternion.Euler(Vector3.forward * TBInput.GetRotation(TBInput.Controller.Active).eulerAngles.z);
		accelX.text = TBInput.GetAcceleration(TBInput.Controller.Active).x + string.Empty;
		accelY.text = TBInput.GetAcceleration(TBInput.Controller.Active).y + string.Empty;
		accelZ.text = TBInput.GetAcceleration(TBInput.Controller.Active).z + string.Empty;
		accelController.transform.localPosition = new Vector3(Mathf.Clamp01(TBInput.GetAcceleration(TBInput.Controller.Active).x * accelMult), Mathf.Clamp01((TBInput.GetAcceleration(TBInput.Controller.Active).y - 9.8f) * accelMult), Mathf.Clamp01(TBInput.GetAcceleration(TBInput.Controller.Active).z * accelMult));
		accelController.transform.localPosition = new Vector3(TBInput.GetAcceleration(TBInput.Controller.Active).x * accelMult, (TBInput.GetAcceleration(TBInput.Controller.Active).y - 9.8f) * accelMult, TBInput.GetAcceleration(TBInput.Controller.Active).z * accelMult);
		accelController.transform.rotation = TBInput.GetRotation(TBInput.Controller.Active);
		if (TBInput.GetButton(TBInput.Button.Action1, TBInput.Controller.Active))
		{
			click.text = "1";
		}
		else
		{
			click.text = "0";
		}
		if (TBInput.GetButton(TBInput.Button.Action2, TBInput.Controller.Active))
		{
			app.text = "1";
		}
		else
		{
			app.text = "0";
		}
		if (TBInput.GetTouch(TBInput.Button.Action1, TBInput.Controller.Active))
		{
			touch.text = "1";
		}
		else
		{
			touch.text = "0";
		}
		Vector2 axis2D = TBInput.GetAxis2D(TBInput.Button.Action1, TBInput.Controller.Active);
		axis2D = new Vector2(axis2D.x / 2f + 0.5f, axis2D.y / -2f + 0.5f);
		touchX.text = axis2D.x + " ";
		touchY.text = axis2D.y + " ";
		touchController.transform.localPosition = axis2D * 2f;
		touchController.transform.localPosition -= Vector3.up + Vector3.right;
		touchController.transform.localPosition = new Vector3(touchController.transform.localPosition.x, touchController.transform.localPosition.y * -1f, touchController.transform.localPosition.z);
		headset.transform.rotation = EagerMaster.instance.mainCamera.transform.rotation;
		headsetX.text = headset.transform.rotation.eulerAngles.x + string.Empty;
		headsetY.text = headset.transform.rotation.eulerAngles.y + string.Empty;
		headsetZ.text = headset.transform.rotation.eulerAngles.z + string.Empty;
	}
}
