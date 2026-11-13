using System.Collections;
using UnityEngine;

public class TumballOpponentAI : MonoBehaviour
{
	public float MinTimeBetweenMoves;

	public float MaxTimeBetweenMoves;

	private float timeTilNextMove;

	[Range(0f, 1f)]
	public float OffenseBalance = 1f;

	private float moveTimer;

	public InfiniteTumballSceneBehavior scene;

	public float MinBlowForce = 2f;

	public float MaxBlowForce = 5f;

	public float MinAimTweakDegrees;

	public float MaxAimTweakDegrees;

	public float RotationMult;

	public AnimationCurve BlowForceRamp;

	public float BlowForceDuration;

	public RotateObject RotationScript;

	public AnimationCurve PositionRamp;

	public ParticleSystem particles;

	private ParticleSystem.EmissionModule em;

	private ParticleSystem.MainModule psMain;

	public VVRDialogueSource DialogSource;

	public NPCDialogueClip[] DialogClips;

	private int lastDialogPlayed = -1;

	private void Start()
	{
		ResetMoveTimer();
		transform.position = Vector3.down * 4f;
		em = particles.emission;
		psMain = particles.main;
	}

	private void Update()
	{
		if (!scene.gameOn)
		{
			return;
		}
		moveTimer += Time.deltaTime;
		if (moveTimer >= timeTilNextMove)
		{
			if (OffenseBalance > Random.Range(0f, 1f))
			{
				MakeOffensiveMove();
			}
			else
			{
				MakeDefensiveMove();
			}
			int num = Random.Range(0, DialogClips.Length - 1);
			if (lastDialogPlayed == -1)
			{
				num = Random.Range(0, DialogClips.Length);
			}
			else if (num >= lastDialogPlayed)
			{
				num++;
			}
			DialogSource.ForcePlayClipOverwrite(DialogClips[num]);
			lastDialogPlayed = num;
			ResetMoveTimer();
		}
	}

	private void ResetMoveTimer()
	{
		timeTilNextMove = Random.Range(MinTimeBetweenMoves, MaxTimeBetweenMoves);
		moveTimer = 0f;
	}

	private void MakeOffensiveMove()
	{
		SafeDebug.Log("Making offensive move");
		Collider opponentTumbleweed = scene.OpponentTumbleweed;
		TumballGate tumballGate = null;
		float num = float.MaxValue;
		for (int i = 0; i < scene.Gates.Length; i++)
		{
			TumballGate tumballGate2 = scene.Gates[i];
			if (!(tumballGate2 == scene.lastOpponentGate))
			{
				float num2 = Vector3.Distance(opponentTumbleweed.transform.position, tumballGate2.transform.position);
				if (num2 < num)
				{
					num = num2;
					tumballGate = tumballGate2;
				}
			}
		}
		Vector3 vector = tumballGate.transform.position - opponentTumbleweed.transform.position;
		vector.y = 0f;
		vector = vector.normalized;
		float num3 = Mathf.Sign(Random.Range(-1f, 1f));
		vector = Quaternion.Euler(0f, Random.Range(MinAimTweakDegrees, MaxAimTweakDegrees) * num3, 0f) * vector;
		Vector3 initPos = opponentTumbleweed.transform.position - vector;
		initPos.y = -2.5f;
		transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
		GlobalCoroutineContainer.AddThread(InitiateBlowForce(vector, initPos, opponentTumbleweed.attachedRigidbody), this);
	}

	private void MakeDefensiveMove()
	{
		SafeDebug.Log("Making defensive move");
		Collider playerTumbleweed = scene.PlayerTumbleweed;
		TumballGate tumballGate = null;
		float num = float.MaxValue;
		for (int i = 0; i < scene.Gates.Length; i++)
		{
			TumballGate tumballGate2 = scene.Gates[i];
			if (!(tumballGate2 == scene.lastPlayerGate))
			{
				float num2 = Vector3.Distance(playerTumbleweed.transform.position, tumballGate2.transform.position);
				if (num2 < num)
				{
					num = num2;
					tumballGate = tumballGate2;
				}
			}
		}
		Vector3 vector = playerTumbleweed.transform.position - tumballGate.transform.position;
		vector.y = 0f;
		vector = vector.normalized;
		float num3 = Mathf.Sign(Random.Range(-1f, 1f));
		vector = Quaternion.Euler(0f, Random.Range(MinAimTweakDegrees, MaxAimTweakDegrees) * num3, 0f) * vector;
		Vector3 initPos = playerTumbleweed.transform.position - vector;
		initPos.y = -2.5f;
		transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
		GlobalCoroutineContainer.AddThread(InitiateBlowForce(vector, initPos, playerTumbleweed.attachedRigidbody), this);
	}

	private IEnumerator InitiateBlowForce(Vector3 direction, Vector3 initPos, Rigidbody tumbleweed)
	{
		float timer = 0f;
		float blowForce = Random.Range(MinBlowForce, MaxBlowForce);
		Vector3 endPos = new Vector3(initPos.x, 0f, initPos.z);
		while (timer < BlowForceDuration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / BlowForceDuration);
			float forceMult = BlowForceRamp.Evaluate(t) * blowForce;
			tumbleweed.AddForce(direction * forceMult);
			RotationScript.rotation = Vector3.forward * forceMult * RotationMult;
			transform.position = Vector3.LerpUnclamped(initPos, endPos, PositionRamp.Evaluate(t));
			psMain.startSpeed = forceMult * 1.5f;
			em.rateOverTime = forceMult * 3f;
			yield return null;
		}
	}
}
