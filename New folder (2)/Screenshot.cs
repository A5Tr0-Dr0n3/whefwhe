using UnityEngine;

public class Screenshot : MonoBehaviour
{
	private string screenshotName;

	private int screenshotNumber;

	public int screenshotRes = 4;

	private void Update()
	{
		if (Input.GetKey(KeyCode.Tab))
		{
			screenshotNumber++;
			screenshotName = "VVR_Screenshot" + screenshotNumber + ".png";
			ScreenCapture.CaptureScreenshot(screenshotName, screenshotRes);
		}
	}
}
