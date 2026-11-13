using UnityEngine;

public class UnderhubHeadsetWrangler : HeadsetWrangler
{
	[SerializeField]
	private Transform headsetAnchor;

	[SerializeField]
	private bool isSceneSelectWrangler;

	[SerializeField]
	private bool forcedNotKinematic;

	private bool disableWrangler;

	protected override void Start()
	{
	}

	public void ReactToSceneActivated()
	{
		if (!disableWrangler)
		{
			if (headsetGO == null && EagerScenes.instance.lastHeadsetRemoved == null && isSceneSelectWrangler)
			{
				disableWrangler = true;
			}
			else if (headsetGO == null && EagerScenes.instance.lastHeadsetRemoved != null)
			{
				headsetGO = EagerScenes.instance.lastHeadsetRemoved.gameObject;
				headsetRB = headsetGO.GetComponent<Rigidbody>();
				sqrRegrab = regrabDistance * regrabDistance;
				hTransform = headsetGO.transform;
				initialHPos = headsetAnchor.position;
				initialHRot = headsetAnchor.rotation;
			}
		}
	}

	protected override void Update()
	{
		if (!(headsetGO == null))
		{
			base.Update();
			if (forcedNotKinematic && cachedIsKinematic)
			{
				cachedIsKinematic = false;
			}
		}
	}
}
