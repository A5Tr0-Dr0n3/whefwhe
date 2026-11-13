using UnityEngine;

public class TumballGate : MonoBehaviour
{
	public InfiniteTumballSceneBehavior scene;

	private bool playerInBounds;

	private float playerEntryDirection;

	private bool opponentInBounds;

	private float opponentEntryDirection;

	public Color PlayerIndicatorColor;

	public Color OpponentIndicatorColor;

	[Range(0f, 1f)]
	public float IndicatorGlowWhite;

	private Color sourcePlayerColor;

	private Color sourceOpponentColor;

	public MeshRenderer PlayerIndicator;

	public MeshRenderer PlayerIndicatorGlowCard;

	public MeshRenderer OpponentIndicator;

	public MeshRenderer OpponentIndicatorGlowCard;

	private MaterialPropertyBlock playerIndicatorBlock;

	private MaterialPropertyBlock playerGlowCardBlock;

	private MaterialPropertyBlock opponentIndicatorBlock;

	private MaterialPropertyBlock opponentGlowCardBlock;

	private void Awake()
	{
		playerIndicatorBlock = new MaterialPropertyBlock();
		playerGlowCardBlock = new MaterialPropertyBlock();
		opponentIndicatorBlock = new MaterialPropertyBlock();
		opponentGlowCardBlock = new MaterialPropertyBlock();
		sourcePlayerColor = Color.Lerp(PlayerIndicatorColor, Color.white, IndicatorGlowWhite);
		sourceOpponentColor = Color.Lerp(OpponentIndicatorColor, Color.white, IndicatorGlowWhite);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other != scene.PlayerTumbleweed) || !(other != scene.OpponentTumbleweed))
		{
			playerInBounds = other == scene.PlayerTumbleweed;
			opponentInBounds = other == scene.OpponentTumbleweed;
			float num = Mathf.Sign(transform.InverseTransformPoint(other.transform.position).z);
			if (playerInBounds)
			{
				playerEntryDirection = num;
			}
			else if (opponentInBounds)
			{
				opponentEntryDirection = num;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		CheckColliderForScore(other);
	}

	private void OnTriggerExit(Collider other)
	{
		CheckColliderForScore(other);
	}

	private void CheckColliderForScore(Collider other)
	{
		if (!scene.gameOn)
		{
			return;
		}
		bool flag = other == scene.PlayerTumbleweed;
		bool flag2 = other == scene.OpponentTumbleweed;
		if (flag || flag2)
		{
			float num = Mathf.Sign(transform.InverseTransformPoint(other.transform.position).z);
			if (playerInBounds && flag && playerEntryDirection != num)
			{
				scene.OnGateEntered(this, playerTumball: true);
				playerInBounds = false;
			}
			if (opponentInBounds && flag2 && opponentEntryDirection != num)
			{
				scene.OnGateEntered(this, playerTumball: false);
				opponentInBounds = false;
			}
		}
	}

	public void UpdateIndicatorLights(bool isLastPlayerGate, bool isLastOpponentGate)
	{
		playerIndicatorBlock.SetColor("_Color", (!isLastPlayerGate) ? sourcePlayerColor : Color.black);
		opponentIndicatorBlock.SetColor("_Color", (!isLastOpponentGate) ? sourceOpponentColor : Color.black);
		playerGlowCardBlock.SetColor("_Color", (!isLastPlayerGate) ? PlayerIndicatorColor : Color.black);
		opponentGlowCardBlock.SetColor("_Color", (!isLastOpponentGate) ? OpponentIndicatorColor : Color.black);
		PlayerIndicator.SetPropertyBlock(playerIndicatorBlock);
		PlayerIndicatorGlowCard.SetPropertyBlock(playerGlowCardBlock);
		OpponentIndicator.SetPropertyBlock(opponentIndicatorBlock);
		OpponentIndicatorGlowCard.SetPropertyBlock(opponentGlowCardBlock);
	}
}
