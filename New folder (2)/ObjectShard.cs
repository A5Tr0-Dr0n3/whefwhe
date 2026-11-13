using System.Collections.Generic;
using UnityEngine;

public class ObjectShard : ObjectBase
{
	public static Queue<ObjectShard> shardQueue;

	private int queueCapacity = 20;

	[HideInInspector]
	public Rigidbody shardRb;

	protected override void Awake()
	{
		base.Awake();
	}

	public virtual void OnBreak()
	{
		if (shardQueue == null)
		{
			shardQueue = new Queue<ObjectShard>(queueCapacity);
		}
		shardRb = GetComponent<Rigidbody>();
		if (shardQueue.Count == queueCapacity)
		{
			Object.Destroy(shardQueue.Dequeue());
		}
		shardQueue.Enqueue(this);
	}
}
