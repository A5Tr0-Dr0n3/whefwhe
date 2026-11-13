using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class JobReviewBrowsingInterface : MonoBehaviour
{
	public enum ReviewState
	{
		Hidden,
		AISelect,
		Review,
		Animating
	}

	[SerializeField]
	private ReviewTile[] reviewTiles;

	[SerializeField]
	private Transform AIReviewTransform;

	[SerializeField]
	private Transform reviewParent;

	[SerializeField]
	private TextMeshPro averageReviewText;

	[SerializeField]
	private TextSwapper aISwapper;

	[SerializeField]
	private TextSwapper reviewSwapper;

	[SerializeField]
	private GameObject[] starsFull;

	[SerializeField]
	private GameObject[] starsEmpty;

	[SerializeField]
	private JobReviewScriptableObject[] defaultJobReviews;

	[SerializeField]
	private JobReviewScriptableObject[] butterExtensionReviews;

	public bool reviewInterfaceActive = true;

	private ReviewState reviewState;

	private JobReview[] cachedUserReviews;

	private int jobReviewIndex;

	private int currentAI;

	private int numReviewsUnlocked;

	private GameObject AIAvatar;

	private bool AIScaled;

	private bool reviewScaled;

	private float scaleLerp;

	private Vector3 startPos;

	private void Start()
	{
		reviewState = ReviewState.AISelect;
		reviewParent.gameObject.SetActive(value: false);
		for (int i = 0; i < reviewTiles.Length; i++)
		{
			reviewTiles[i].HookUpToReviewSystem(i, SelectReviewTile);
		}
	}

	public void UpdateReviewData()
	{
		reviewTiles.ToList().ForEach((ReviewTile r) =>
		{
			r.Initialize();
		});
		numReviewsUnlocked = Mathf.Clamp(JobReviewUtility.JobStateToReviewIndex(PersistentDataManager.GetJobState()) + PersistentDataManager.GetKitchenLoopCounter(), 0, reviewTiles.Length);
		cachedUserReviews = JobReviewUtility.GetJobReviews(defaultJobReviews, numReviewsUnlocked, butterExtensionReviews);
		float num = 0f;
		for (int num2 = 0; num2 < numReviewsUnlocked; num2++)
		{
			num += cachedUserReviews[num2].reviewScore;
		}
		num /= (float)numReviewsUnlocked;
		num = Mathf.Round(num * 10f);
		num /= 10f;
		GlobalCoroutineContainer.AddThread(ReviewScoreChange(float.Parse(averageReviewText.text), num), this);
		for (int num3 = 0; num3 < reviewTiles.Length; num3++)
		{
			reviewTiles[num3].SetUnlocked(num3 > numReviewsUnlocked - 1);
		}
	}

	private IEnumerator ReviewScoreChange(float oldScore, float newScore)
	{
		while (oldScore - newScore > 0.1f)
		{
			oldScore = ((!(newScore < oldScore)) ? (oldScore + 0.1f) : (oldScore - 0.1f));
			oldScore = Mathf.Round(oldScore * 10f);
			oldScore /= 10f;
			averageReviewText.text = oldScore.ToString();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.35f, this);
		}
	}

	private void Update()
	{
		if (reviewState != ReviewState.Review)
		{
		}
	}

	private void UpdateReview()
	{
		AIAvatar.transform.Rotate(Vector3.up, 100f * Time.deltaTime);
	}

	private IEnumerator ShowReview()
	{
		reviewState = ReviewState.Animating;
		AIScaled = false;
		scaleLerp = 0f;
		startPos = reviewTiles[currentAI].transform.position;
		while (!AIScaled)
		{
			scaleLerp += Time.deltaTime * 10f;
			if (scaleLerp > 1f)
			{
				scaleLerp = 1f;
				AIScaled = true;
			}
			for (int i = 0; i < reviewTiles.Length; i++)
			{
				if (i == currentAI)
				{
					reviewTiles[i].transform.localScale = Vector3.Lerp(Vector3.one, AIReviewTransform.localScale, scaleLerp);
					reviewTiles[i].transform.position = Vector3.Lerp(startPos, AIReviewTransform.position, scaleLerp);
				}
				else
				{
					reviewTiles[i].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, scaleLerp);
				}
			}
			yield return null;
		}
		reviewScaled = false;
		scaleLerp = 0f;
		reviewParent.gameObject.SetActive(value: true);
		while (!reviewScaled)
		{
			scaleLerp += Time.deltaTime * 10f;
			if (scaleLerp > 1f)
			{
				scaleLerp = 1f;
				reviewScaled = true;
			}
			reviewParent.transform.localScale = Vector3.Lerp(Vector3.up + Vector3.forward, Vector3.one, scaleLerp);
			yield return null;
		}
		AIAvatar = reviewTiles[currentAI].aiMesh.gameObject;
		reviewState = ReviewState.Review;
	}

	public void HideReview()
	{
		GlobalCoroutineContainer.AddThread(HideReviewCoroutine(), this);
	}

	private IEnumerator HideReviewCoroutine()
	{
		reviewState = ReviewState.Animating;
		reviewScaled = false;
		scaleLerp = 1f;
		reviewParent.gameObject.SetActive(value: true);
		while (!reviewScaled)
		{
			scaleLerp -= Time.deltaTime * 10f;
			if (scaleLerp < 0f)
			{
				scaleLerp = 0f;
				reviewScaled = true;
			}
			reviewParent.transform.localScale = Vector3.Lerp(Vector3.up + Vector3.forward, Vector3.one, scaleLerp);
			yield return null;
		}
		reviewParent.gameObject.SetActive(value: false);
		AIScaled = false;
		scaleLerp = 1f;
		while (!AIScaled)
		{
			scaleLerp -= Time.deltaTime * 10f;
			if (scaleLerp < 0f)
			{
				scaleLerp = 0f;
				AIScaled = true;
			}
			for (int i = 0; i < reviewTiles.Length; i++)
			{
				if (i == currentAI)
				{
					reviewTiles[i].transform.localScale = Vector3.Lerp(Vector3.one, AIReviewTransform.localScale, scaleLerp);
					reviewTiles[i].transform.position = Vector3.Lerp(startPos, AIReviewTransform.position, scaleLerp);
				}
				else
				{
					reviewTiles[i].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, scaleLerp);
				}
			}
			yield return null;
		}
		starsFull[0].transform.parent.gameObject.SetActive(value: false);
		starsEmpty[0].transform.parent.gameObject.SetActive(value: false);
		reviewState = ReviewState.AISelect;
	}

	public void SelectReviewTile()
	{
		SelectReviewTile(-1);
	}

	public void SelectReviewTile(VVR_HUB.JobStates jobState)
	{
		SelectReviewTile(JobReviewUtility.JobStateToReviewIndex(jobState) - 1);
	}

	public void SelectReviewTile(int i)
	{
		if (i == -1 || !reviewInterfaceActive || reviewState == ReviewState.Animating || i >= numReviewsUnlocked)
		{
			return;
		}
		if (reviewState == ReviewState.AISelect)
		{
			currentAI = i;
			aISwapper.SetNewKey(cachedUserReviews[i].clientName);
			reviewSwapper.SetNewKey(cachedUserReviews[i].reviewText);
			starsFull[0].transform.parent.gameObject.SetActive(value: true);
			starsEmpty[0].transform.parent.gameObject.SetActive(value: true);
			for (int j = 0; j < 5; j++)
			{
				if ((float)(j + 1) > cachedUserReviews[i].reviewScore)
				{
					starsFull[j].SetActive(value: false);
					starsEmpty[j].SetActive(value: true);
				}
				else
				{
					starsFull[j].SetActive(value: true);
					starsEmpty[j].SetActive(value: false);
				}
			}
			StartCoroutine(ShowReview());
		}
		else if (reviewState == ReviewState.Review && i == currentAI)
		{
			GlobalCoroutineContainer.AddThread(HideReviewCoroutine(), this);
		}
	}

	public void SwapReviewText()
	{
	}
}
