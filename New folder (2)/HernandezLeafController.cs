using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GvrAudioSource))]
public class HernandezLeafController : MonoBehaviour
{
	[SerializeField]
	private AudioClip attachSound;

	[SerializeField]
	private AudioClip detachSound;

	[SerializeField]
	public HernandezLeaf mouthLeaf;

	[SerializeField]
	private List<HernandezLeaf> outerLeaves;

	[SerializeField]
	private List<HernandezLeaf> innerLeaves;

	private List<HernandezLeaf> allBodyLeaves;

	[SerializeField]
	private ObjectBase innerHeadsetObj;

	[SerializeField]
	private FixedJoint innerHeadsetFixedJoint;

	private List<HernandezLeaf> freeLeaves;

	private float originalLeafCount;

	private HernandezLeafAnchor[] leafAnchors;

	private GvrAudioSource leafAttachAudioSource;

	public bool acceptReattaching { get; private set; }

	public bool isDead { get; private set; }

	private void Update()
	{
		if (ToolManager.PlayerGrabbingObjectBase(innerHeadsetObj) && innerHeadsetFixedJoint != null)
		{
			innerHeadsetFixedJoint.breakForce = 30f;
		}
		if (innerHeadsetFixedJoint == null)
		{
			DropAllLeavesAndDie();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			DropRandomLeaf();
		}
	}

	private void Start()
	{
		leafAttachAudioSource = GetComponent<GvrAudioSource>();
		freeLeaves = new List<HernandezLeaf>();
		constructLeaves();
	}

	private void constructLeaves()
	{
		allBodyLeaves = new List<HernandezLeaf>();
		allBodyLeaves.AddRange(outerLeaves);
		allBodyLeaves.AddRange(innerLeaves);
		leafAnchors = new HernandezLeafAnchor[allBodyLeaves.Count];
		for (int i = 0; i < allBodyLeaves.Count; i++)
		{
			leafAnchors[i] = allBodyLeaves[i].Initialize(this);
		}
		originalLeafCount = allBodyLeaves.Count;
		for (int j = 0; j < innerLeaves.Count; j++)
		{
			innerLeaves[j].ungrabbable = true;
		}
		innerHeadsetObj.ungrabbable = true;
		for (int k = 0; k < allBodyLeaves.Count; k++)
		{
			setCollideWithBodyLeaves(allBodyLeaves[k].mCollider, collideWithBody: false);
		}
		for (int l = 0; l < innerHeadsetObj.colliders.Length; l++)
		{
			setCollideWithBodyLeaves(innerHeadsetObj.colliders[l], collideWithBody: false);
		}
	}

	public void PlayLeafSfx(bool attach)
	{
		if (attach)
		{
			leafAttachAudioSource.PlayOneShot(attachSound);
		}
		else
		{
			leafAttachAudioSource.PlayOneShot(detachSound);
		}
	}

	public void RegisterLeaf(bool attachedToBody, HernandezLeaf leafIn, float delay = 0f)
	{
		List<HernandezLeaf> list = ((!attachedToBody) ? freeLeaves : allBodyLeaves);
		List<HernandezLeaf> list2 = ((!attachedToBody) ? allBodyLeaves : freeLeaves);
		list2.Remove(leafIn);
		if (!list.Contains(leafIn))
		{
			list.Add(leafIn);
		}
		GlobalCoroutineContainer.AddThread(setCollideWithBodyLeaves(leafIn.mCollider, !attachedToBody, delay), this);
	}

	private void animateLeafPositionToVolume(float volume)
	{
	}

	public HernandezLeaf DropClosestLeafToPlayer()
	{
		int num = 0;
		float num2 = 999f;
		for (int i = 0; i < outerLeaves.Count; i++)
		{
			if (!(outerLeaves[i] == mouthLeaf))
			{
				float num3 = Vector3.Distance(outerLeaves[i].transform.position, EagerMaster.instance.mainCamera.transform.position);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
		}
		if (num < outerLeaves.Count)
		{
			HernandezLeaf hernandezLeaf = outerLeaves[num];
			hernandezLeaf.DropFromBody();
			return hernandezLeaf;
		}
		return null;
	}

	public void DropRandomLeaf()
	{
		bool flag = false;
		for (int i = 0; i < allBodyLeaves.Count; i++)
		{
			if (allBodyLeaves[i] != mouthLeaf && !innerLeaves.Contains(allBodyLeaves[i]))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			int index = Random.Range(0, allBodyLeaves.Count);
			while (allBodyLeaves[index] == mouthLeaf || innerLeaves.Contains(allBodyLeaves[index]))
			{
				index = Random.Range(0, allBodyLeaves.Count);
			}
			allBodyLeaves[index].DropFromBody();
		}
	}

	public void DropAllLeavesAndDie()
	{
		for (int num = allBodyLeaves.Count - 1; num >= 0; num--)
		{
			allBodyLeaves[num].DropFromBody();
		}
		isDead = true;
	}

	public void SetAcceptReattaching(bool newValue)
	{
		acceptReattaching = newValue;
	}

	private IEnumerator setCollideWithBodyLeaves(Collider colliderIn, bool collideWithBody, float delay = 0f)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		for (int i = 0; i < allBodyLeaves.Count; i++)
		{
			if (allBodyLeaves[i] != null && allBodyLeaves[i].mCollider != null)
			{
				allBodyLeaves[i].mCollisionIgnorer.SetIgnoreCollision(new Collider[1] { colliderIn }, ignore: true);
			}
		}
		for (int j = 0; j < freeLeaves.Count; j++)
		{
			if (freeLeaves[j] != null && freeLeaves[j].mCollider != null)
			{
				freeLeaves[j].mCollisionIgnorer.SetIgnoreCollision(new Collider[1] { colliderIn }, ignore: true);
			}
		}
	}

	public void MakeOuterLeavesPluckable()
	{
		for (int i = 0; i < outerLeaves.Count; i++)
		{
			if (outerLeaves[i] != mouthLeaf)
			{
				outerLeaves[i].MakePluckable();
			}
		}
	}

	public void MakeInnerLeavesPluckable()
	{
		mouthLeaf.MakePluckable();
		for (int i = 0; i < innerLeaves.Count; i++)
		{
			innerLeaves[i].MakePluckable();
			innerLeaves[i].ungrabbable = false;
		}
		innerHeadsetObj.ungrabbable = false;
	}

	public float GetRemainingLeafPercentage()
	{
		return (float)(allBodyLeaves.Count - innerLeaves.Count) / (originalLeafCount - (float)innerLeaves.Count);
	}

	public void IgnoreCollisionWithObjects(Collider[] objectsIn, bool ignoreCollision)
	{
		for (int i = 0; i < allBodyLeaves.Count; i++)
		{
			allBodyLeaves[i].mCollisionIgnorer.SetIgnoreCollision(objectsIn, ignoreCollision);
		}
	}
}
