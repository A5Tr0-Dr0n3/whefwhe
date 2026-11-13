using System.Collections.Generic;
using UnityEngine;

public class PersistentCollisionIgnorer : MonoBehaviour
{
	private bool initialized;

	private List<Collider> ignoredColliders = new List<Collider>();

	public Collider mCollider { get; private set; }

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (!initialized)
		{
			mCollider = GetComponent<Collider>();
			initialized = true;
		}
	}

	public void SetIgnoreCollision(PersistentCollisionIgnorer other, bool ignore)
	{
		SetIgnoreCollision(new Collider[1] { other.mCollider }, ignore);
		other.SetIgnoreCollision(new Collider[1] { mCollider }, ignore);
	}

	public void SetIgnoreCollision(Collider[] collidersIn, bool ignore)
	{
		for (int i = 0; i < collidersIn.Length; i++)
		{
			Physics.IgnoreCollision(collidersIn[i], mCollider, ignore);
		}
		if (ignore)
		{
			for (int j = 0; j < collidersIn.Length; j++)
			{
				if (!ignoredColliders.Contains(collidersIn[j]))
				{
					ignoredColliders.Add(collidersIn[j]);
				}
			}
			return;
		}
		for (int k = 0; k < collidersIn.Length; k++)
		{
			while (ignoredColliders.Contains(collidersIn[k]))
			{
				ignoredColliders.Remove(collidersIn[k]);
			}
		}
	}

	private void OnEnable()
	{
		Initialize();
		for (int i = 0; i < ignoredColliders.Count; i++)
		{
			Physics.IgnoreCollision(ignoredColliders[i], mCollider);
		}
	}
}
