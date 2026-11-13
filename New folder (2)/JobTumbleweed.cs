using Edelweiss.Coroutine;
using TMPro;
using UnityEngine;

public class JobTumbleweed : SceneBehaviorJob
{
	public static JobTumbleweed instance;

	[Header("References:")]
	public ConveyorLooper conveyor;

	public ConveyorPropSpawner spawner;

	public GameObject leafblower;

	public GameObject pedestal;

	public TextMeshPro scoreText;

	public TextMeshPro fallText;

	public TextMeshPro timeText;

	[Header("Tumbleweed Settings:")]
	public Vector3 respawnPoint = Vector3.zero;

	[Header("Tumbleweed Settings:")]
	public Vector3 tumblePosLastFrame;

	protected SafeCoroutine resetThread;

	protected bool whistling;

	[Header("Obstacle Spawns:")]
	public float spawnRate = 0.1f;

	[Range(0f, 1f)]
	public float cactusRate = 0.3f;

	[Range(0f, 1f)]
	public float fenceRate = 0.3f;

	[Range(0f, 1f)]
	public float rockRate = 0.3f;

	[Header("Speed Setting:")]
	[Range(0f, 10f)]
	public float easySpeed = 1f;

	[Range(0f, 10f)]
	public float mediumSpeed = 1.8f;

	[Range(0f, 10f)]
	public float hardSpeed = 3.2f;

	[Range(0f, 10f)]
	public float insaneSpeed = 5f;

	[HideInInspector]
	public float currMaxSpeed;

	[Header("Stats:")]
	public int score;

	public int falls;

	[Header("Reviews:")]
	public JobReviewScriptableObject badReview;

	public JobReviewScriptableObject goodReview;

	protected float ticker;

	protected float timer;

	protected int minutes;

	protected int seconds;

	public float cactusRateNormalized => cactusRate / totalPropRate;

	public float fenceRateNormalized => fenceRate / totalPropRate;

	public float rockRateNormalized => rockRate / totalPropRate;

	public float totalPropRate => cactusRate + fenceRate + rockRate;

	private void FixedUpdate()
	{
		ticker += Time.deltaTime;
		timer += Time.deltaTime;
		if (ticker > 1f)
		{
			minutes = Mathf.FloorToInt(timer / 60f);
			seconds = Mathf.FloorToInt(timer % 60f);
			if (seconds < 10)
			{
				timeText.text = minutes + ":0" + seconds;
			}
			else
			{
				timeText.text = minutes + ":" + seconds;
			}
			ticker = 0f;
		}
	}
}
