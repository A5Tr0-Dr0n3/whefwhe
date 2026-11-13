using UnityEngine;

public class ObjectHeadsetLOD : ObjectBase
{
	public Mesh highLODHeadset;

	public Mesh lowLODHeadset;

	public MeshFilter hFilter;

	public GameObject eyeGlow;

	private bool _highLOD;

	private Headset myHeadset;

	public bool highLOD
	{
		get
		{
			return _highLOD;
		}
		set
		{
			if (highLODHeadset != null && lowLODHeadset != null && hFilter != null)
			{
				if (value)
				{
					hFilter.sharedMesh = highLODHeadset;
				}
				else
				{
					hFilter.sharedMesh = lowLODHeadset;
				}
			}
			if (eyeGlow != null)
			{
				eyeGlow.SetActive(value);
			}
			_highLOD = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		highLOD = false;
	}

	public override void onTeleport()
	{
		if (myHeadset == null)
		{
			myHeadset = GetComponent<Headset>();
		}
		if ((bool)myHeadset)
		{
			myHeadset.headsetHasBeenSafelyRemoved = false;
			myHeadset.timeSinceRemoved = myHeadset.headsetDropMinTime + 0.25f;
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		highLOD = true;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		if (!EagerHeadsetManager.instance.isAddingHeadset)
		{
			highLOD = false;
		}
	}

	public void OnRemovedHeadset()
	{
		highLOD = false;
	}
}
