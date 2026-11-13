using TMPro;
using UnityEngine;

public class KitchenScoreboardManager : MonoBehaviour
{
	public Kitchen kitchen;

	public TextMeshPro countUncooked;

	public TextMeshPro countToasted;

	public TextMeshPro countBurnt;

	public TextMeshPro timerText;

	private float timer;

	private float ticker;

	private int minutes;

	private int seconds;

	private void Update()
	{
		ticker += Time.deltaTime;
		timer += Time.deltaTime;
		if (ticker > 1f)
		{
			countUncooked.text = updateCount(0);
			countToasted.text = updateCount(1);
			countBurnt.text = updateCount(2);
			minutes = Mathf.FloorToInt(timer / 60f);
			seconds = Mathf.FloorToInt(timer % 60f);
			if (seconds < 10)
			{
				timerText.text = minutes + ":0" + seconds;
			}
			else
			{
				timerText.text = minutes + ":" + seconds;
			}
			ticker = 0f;
		}
	}

	private string updateCount(int toastIndex)
	{
		if (kitchen.toastCount[toastIndex] < 10)
		{
			return "x  0" + kitchen.toastCount[toastIndex];
		}
		return "x  " + kitchen.toastCount[toastIndex];
	}
}
