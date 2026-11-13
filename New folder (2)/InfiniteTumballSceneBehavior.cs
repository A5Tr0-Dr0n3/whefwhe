using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InfiniteTumballSceneBehavior : SceneBehaviorManager
{
	[SerializeField]
	private float destroyHeadsetDelay = 0.75f;

	[SerializeField]
	private float destroyHeadsetTime = 0.75f;

	[SerializeField]
	public GameObject resetObjectTemplate;

	private Headset returnHeadset;

	private GameObject cachedResetCopy;

	private GameObject oldCachedReset;

	public TumballGate[] Gates;

	public Collider PlayerTumbleweed;

	public Collider OpponentTumbleweed;

	public float GateTriangleSideLength;

	public float GateScale;

	public TextSwapper MessageTextSwap;

	public TextMeshPro ScoreTextMesh;

	private int playerScore;

	private int opponentScore;

	private float roundTimer;

	private bool isOvertime;

	[HideInInspector]
	public TumballGate lastPlayerGate;

	[HideInInspector]
	public TumballGate lastOpponentGate;

	private const float roundDuration = 180f;

	private const float overtimeDuration = 60f;

	private const int pointsToWin = 3;

	private bool timeLimitOn;

	public Light LeafBlowerLight;

	public ObjectLeafBlower LeafBlower;

	public ElevatorEnvironments ScoreEnvironment;

	public AnimationCurve GateAnimationCurve;

	public float GateAnimationDuration;

	public float BlowbackThreshold;

	public bool gameOn;

	private bool showingMessage;

	private float messageTimer;

	private float messageDuration;

	private bool blowingPlayerBack;

	private bool blowingOpponentBack;

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		EagerScenes.instance.LoadScenesUnderneathCurrentScene(new TCPlatformScene[1] { TCScenes.S.VVR_UNDERHUB_07_INFINITETUMBALL }, allowDuplicates: true);
		if (EagerScenes.instance.lastHeadsetRemoved != null)
		{
			if (returnHeadset == null || !returnHeadset.enabled)
			{
				returnHeadset = EagerScenes.instance.lastHeadsetRemoved;
			}
			else
			{
				GlobalCoroutineContainer.AddThread(destroyRedundantHeadsetSequence(EagerScenes.instance.lastHeadsetRemoved), this);
			}
		}
		if (cachedResetCopy == null)
		{
			cachedResetCopy = UnityEngine.Object.Instantiate(resetObjectTemplate);
			cachedResetCopy.transform.parent = resetObjectTemplate.transform.parent;
			cachedResetCopy.SetActive(value: false);
			oldCachedReset = resetObjectTemplate;
		}
		else
		{
			if (oldCachedReset != null)
			{
				UnityEngine.Object.Destroy(oldCachedReset);
			}
			cachedResetCopy.SetActive(value: true);
			oldCachedReset = cachedResetCopy;
			cachedResetCopy = UnityEngine.Object.Instantiate(oldCachedReset);
			cachedResetCopy.SetActive(value: false);
			cachedResetCopy.transform.parent = oldCachedReset.transform.parent;
		}
		SetUpGates();
		ResetGame();
		ScoreEnvironment.MatchEnvironment(0.1f, 3);
		if (LeafBlower.grabbedBy.Count > 0)
		{
			LeafBlower.ForceDropByPlayer();
			LeafBlower.isGrabbed = false;
			if (gameOn)
			{
				OnRoundCompleted();
				ScoreEnvironment.MatchEnvironment(2f, 3);
			}
		}
		MessageTextSwap.UseAltString(string.Empty);
		ScoreTextMesh.text = string.Empty;
	}

	private void SetUpGates()
	{
		float num = GateTriangleSideLength / Mathf.Sqrt(3f);
		for (int i = 0; i < Gates.Length; i++)
		{
			float num2 = (float)i / (float)Gates.Length;
			num2 *= (float)Math.PI * 2f;
			num2 += (float)Math.PI / 6f;
			TumballGate tumballGate = Gates[i];
			tumballGate.gameObject.SetActive(value: true);
			tumballGate.transform.localPosition = new Vector3(Mathf.Cos(num2) * num, -2.5f, Mathf.Sin(num2) * num);
			Vector3 localPosition = tumballGate.transform.localPosition;
			localPosition.y = 0f;
			tumballGate.transform.localRotation = Quaternion.LookRotation(localPosition, Vector3.up);
			tumballGate.transform.localScale = Vector3.one * GateScale;
			tumballGate.scene = this;
		}
	}

	private void ResetGame()
	{
		playerScore = 0;
		opponentScore = 0;
		roundTimer = 0f;
		isOvertime = false;
		lastPlayerGate = null;
		lastOpponentGate = null;
		for (int i = 0; i < Gates.Length; i++)
		{
			Gates[i].UpdateIndicatorLights(isLastPlayerGate: false, isLastOpponentGate: false);
		}
		blowingPlayerBack = false;
		blowingOpponentBack = false;
	}

	private void StartNewRound()
	{
		gameOn = true;
		ResetGame();
		InitializeTumbleweedPositions();
		ShowMessage("TEXT_319", 5f);
		UpdateScoreText();
		ScoreEnvironment.MatchEnvironment(0.5f, 0);
		GlobalCoroutineContainer.AddThread(startRoundGateAnim(), this);
	}

	private void InitializeTumbleweedPositions()
	{
		PlayerTumbleweed.transform.position = EagerMaster.instance.mainCamera.transform.position + Vector3.up * 10f + EagerMaster.instance.mainCamera.transform.forward * 3f;
		float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		float num = UnityEngine.Random.Range(0.25f, 0.75f) * BlowbackThreshold;
		OpponentTumbleweed.transform.position = new Vector3(Mathf.Cos(f) * num, 10f, Mathf.Sin(f) * num);
	}

	private void StartOverTime()
	{
		roundTimer = 0f;
		isOvertime = true;
	}

	private void ShowMessage(string message, float duration)
	{
		messageTimer = 0f;
		messageDuration = duration;
		showingMessage = true;
		MessageTextSwap.UseAltString(alt: false);
		MessageTextSwap.SetNewKey(message);
	}

	private void UpdateScoreText()
	{
		ScoreTextMesh.text = playerScore + " - " + opponentScore;
	}

	private void Update()
	{
		if (!gameOn)
		{
			if (LeafBlower.isGrabbed)
			{
				StartNewRound();
			}
			return;
		}
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (ToolManager.ActiveToolGroups[i].EagerTouch.GetToolSwitchDetected() || ToolManager.ActiveToolGroups[i].EagerTouch.GetToolSwitchDetected(left: false))
			{
				ObjectBase heldObjectBase = ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase;
				if (heldObjectBase is ObjectLeafBlower)
				{
					ToolManager.ActiveToolGroups[i].Manipulator.Drop();
					OnRoundCompleted();
					ScoreEnvironment.MatchEnvironment(2f, 3);
					return;
				}
			}
		}
		if (!blowingPlayerBack && PlayerTumbleweed.transform.position.magnitude > BlowbackThreshold)
		{
			GlobalCoroutineContainer.AddThread(applyBlowbackForce(PlayerTumbleweed), this);
		}
		if (!blowingOpponentBack && OpponentTumbleweed.transform.position.magnitude > BlowbackThreshold)
		{
			GlobalCoroutineContainer.AddThread(applyBlowbackForce(OpponentTumbleweed), this);
		}
		if (showingMessage)
		{
			messageTimer += Time.deltaTime;
			if (messageTimer >= messageDuration)
			{
				MessageTextSwap.UseAltString(string.Empty);
				showingMessage = false;
			}
		}
		if (!timeLimitOn)
		{
			return;
		}
		roundTimer += Time.deltaTime;
		if (!isOvertime && roundTimer >= 180f)
		{
			if (playerScore == opponentScore)
			{
				StartOverTime();
			}
			else
			{
				OnRoundCompleted();
			}
		}
		else if (isOvertime && roundTimer >= 60f)
		{
			OnRoundCompleted();
		}
	}

	public void OnGateEntered(TumballGate gate, bool playerTumball)
	{
		TumballGate tumballGate = ((!playerTumball) ? lastOpponentGate : lastPlayerGate);
		if (tumballGate != gate)
		{
			int num = -1;
			if (playerTumball)
			{
				lastPlayerGate = gate;
				playerScore++;
				ShowMessage("TEXT_237", 5f);
				num = 1;
			}
			else
			{
				lastOpponentGate = gate;
				opponentScore++;
				ShowMessage("TEXT_238", 5f);
				num = 2;
			}
			for (int i = 0; i < Gates.Length; i++)
			{
				Gates[i].UpdateIndicatorLights(Gates[i] == lastPlayerGate, Gates[i] == lastOpponentGate);
			}
			UpdateScoreText();
			if (isOvertime || playerScore == 3 || opponentScore == 3)
			{
				OnRoundCompleted();
			}
			GlobalCoroutineContainer.AddThread(scoreEnvironmentSequence(num), this);
		}
	}

	private void OnRoundCompleted()
	{
		if (playerScore > opponentScore)
		{
			ShowMessage("TEXT_239!", 5f);
		}
		else if (playerScore == opponentScore)
		{
			ShowMessage("TEXT_240", 5f);
		}
		else
		{
			ShowMessage("TEXT_238", 5f);
		}
		ToolManager.DropEverything();
		LeafBlowerLight.transform.position = new Vector3(LeafBlower.transform.position.x, LeafBlowerLight.transform.position.y, LeafBlower.transform.position.z);
		gameOn = false;
		GlobalCoroutineContainer.AddThread(endRoundGateAnim(), this);
	}

	private IEnumerator destroyRedundantHeadsetSequence(Headset targetHeadset)
	{
		float timer = 0f;
		Vector3 originalScale = targetHeadset.transform.localScale;
		while (timer < destroyHeadsetDelay)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		while (timer < destroyHeadsetTime)
		{
			timer += Time.deltaTime;
			float t = timer / destroyHeadsetTime;
			targetHeadset.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
			yield return null;
		}
		UnityEngine.Object.Destroy(targetHeadset.gameObject);
	}

	private IEnumerator scoreEnvironmentSequence(int toEnvironmentIndex)
	{
		ScoreEnvironment.MatchEnvironment(0.35f, toEnvironmentIndex, changeLighting: false);
		yield return new WaitForSeconds(0.35f);
		if (gameOn)
		{
			ScoreEnvironment.MatchEnvironment(2f, 0, changeLighting: false);
		}
		else
		{
			ScoreEnvironment.MatchEnvironment(2f, 3);
		}
	}

	private IEnumerator applyBlowbackForce(Collider c)
	{
		if (c == PlayerTumbleweed)
		{
			blowingPlayerBack = true;
		}
		else
		{
			blowingOpponentBack = true;
		}
		float timer = 0f;
		float blowForce = 6f;
		Vector3 toCenter = -c.transform.position;
		toCenter.y = 0f;
		toCenter = toCenter.normalized;
		while (timer < 2f)
		{
			timer += Time.deltaTime;
			c.attachedRigidbody.AddForce(toCenter * blowForce);
			yield return null;
		}
		if (c == PlayerTumbleweed)
		{
			blowingPlayerBack = false;
		}
		else
		{
			blowingOpponentBack = false;
		}
	}

	private IEnumerator startRoundGateAnim()
	{
		float timer = 0f;
		while (timer < GateAnimationDuration)
		{
			timer += Time.deltaTime;
			float y = GateAnimationCurve.Evaluate(Mathf.Clamp01(timer / GateAnimationDuration));
			for (int i = 0; i < Gates.Length; i++)
			{
				Gates[i].transform.position = new Vector3(Gates[i].transform.position.x, y, Gates[i].transform.position.z);
			}
			yield return null;
		}
	}

	private IEnumerator endRoundGateAnim()
	{
		float timer = 0f;
		while (timer < GateAnimationDuration)
		{
			timer += Time.deltaTime;
			float y = GateAnimationCurve.Evaluate(1f - Mathf.Clamp01(timer / GateAnimationDuration));
			for (int i = 0; i < Gates.Length; i++)
			{
				Gates[i].transform.position = new Vector3(Gates[i].transform.position.x, y, Gates[i].transform.position.z);
			}
			yield return null;
		}
	}
}
