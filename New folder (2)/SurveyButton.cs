using TMPro;
using UnityEngine;

public class SurveyButton : MonoBehaviour
{
	public ObjectButton responseButton;

	public TextMeshPro responseText;

	public TextSwapper swapper;

	private void OnEnable()
	{
		if (responseText.GetComponent<TextSwapper>() != null)
		{
			swapper = responseText.GetComponent<TextSwapper>();
		}
	}

	public void SetResponseText(string newResponseKey)
	{
		if (swapper != null)
		{
			swapper.SetNewKey(newResponseKey);
		}
	}
}
