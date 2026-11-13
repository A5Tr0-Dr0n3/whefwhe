using System.Collections;
using UnityEngine;

public class UnionThreeScene : MonoBehaviour
{
	public GameObject humanUnionLogo;

	[Header("Union Voice")]
	public VVRDialogueSource voice;

	public NPCDialogueClip criticalError;

	public NPCDialogueClip newIntel;

	public NPCDialogueClip makeYour;

	public NPCDialogueClip manyUsers;

	public NPCDialogueClip mustHurry;

	[Header("Object Animations")]
	public RotateObject ball;

	public GameObject[] core;

	[CurveRange(0f, 1f, 0f, 2f)]
	public AnimationCurve scaleCurve;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve coreCurve;

	public float coreScalar = 1f;

	public float coreDuration = 5f;

	public int coreIter = 4;

	public GvrAudioSource coreAudio;

	private void Start()
	{
		GlobalCoroutineContainer.AddThread(sceneCoroutine(), this);
	}

	private IEnumerator sceneCoroutine()
	{
		if (PersistentDataManager.GetHasAdmin())
		{
			PersistentDataManager.MarkUnion3Completed();
		}
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		humanUnionLogo.SetActive(value: true);
		ball.gameObject.SetActive(value: false);
		Vector3 bScale = ball.transform.localScale;
		ball.transform.localScale = Vector3.zero;
		voice.PlayClip(criticalError);
		while (voice.isPlaying)
		{
			yield return null;
		}
		ball.gameObject.SetActive(value: true);
		ball.rotation.y = 20f;
		voice.PlayClip(newIntel);
		Vector3 logoScale = humanUnionLogo.transform.localScale;
		float t = 0f;
		float scaleUpT = 0.7f;
		while (t < 0.3f)
		{
			t += Time.deltaTime;
			float lerp = t / 0.3f;
			lerp *= lerp;
			humanUnionLogo.transform.localScale = logoScale * (1f - lerp);
			yield return null;
		}
		humanUnionLogo.SetActive(value: false);
		t = 0f;
		scaleUpT = 0.7f;
		while (t < scaleUpT)
		{
			t += Time.deltaTime;
			ball.transform.localScale = bScale * scaleCurve.Evaluate(t / scaleUpT);
			yield return null;
		}
		ball.transform.localScale = new Vector3(bScale.x, bScale.y, bScale.z);
		while (voice.isPlaying)
		{
			yield return null;
		}
		voice.PlayClip(makeYour);
		voice.PlayClip(manyUsers);
		for (int i = 0; i < coreIter; i++)
		{
			t = 0f;
			while (t < coreDuration)
			{
				t += Time.deltaTime;
				for (int j = 0; j < core.Length; j++)
				{
					core[j].transform.localPosition = new Vector3(core[j].transform.localPosition.x, coreCurve.Evaluate(t / coreDuration) * coreScalar, core[j].transform.localPosition.z);
				}
				if (coreAudio != null)
				{
					coreAudio.volume = coreCurve.Evaluate(t / coreDuration) * 1.5f;
				}
				yield return null;
			}
		}
		while (voice.isPlaying)
		{
			yield return null;
		}
		voice.PlayClip(mustHurry);
		t = 0f;
		while (t < scaleUpT)
		{
			t += Time.deltaTime;
			ball.transform.localScale = new Vector3(bScale.x * scaleCurve.Evaluate(1f - t / scaleUpT), bScale.y * scaleCurve.Evaluate(1f - t / scaleUpT), bScale.z * scaleCurve.Evaluate(1f - t / scaleUpT));
			yield return null;
		}
		ball.transform.localScale = Vector3.zero;
		while (voice.isPlaying)
		{
			yield return null;
		}
		PersistentDataManager.MarkUnion3Completed();
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: true);
		EagerHeadsetManager.instance.RemoveHeadset();
	}
}
