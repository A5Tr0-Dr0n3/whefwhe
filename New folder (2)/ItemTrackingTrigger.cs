using System.Collections.Generic;
using UnityEngine;

public class ItemTrackingTrigger : MonoBehaviour
{
	public List<ObjectBase> trackedObj;

	public ItemDisplaySwapper swapper;

	private void OnCollisionEnter(Collision collision)
	{
		if (trackedObj == null)
		{
			trackedObj = new List<ObjectBase>();
		}
		Collider collider = collision.collider;
		if (!collider.isTrigger)
		{
			ObjectBase component = collision.gameObject.GetComponent<ObjectBase>();
			if (component == null)
			{
				component = collision.gameObject.transform.parent.GetComponent<ObjectBase>();
			}
			if (component != null && !trackedObj.Contains(component) && !swapper.GetCurrentGroup().listContains(component.gameObject))
			{
				trackedObj.Add(component);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (trackedObj == null)
		{
			trackedObj = new List<ObjectBase>();
		}
		Collider collider = collision.collider;
		if (collider.isTrigger)
		{
			return;
		}
		ObjectBase component = collision.gameObject.GetComponent<ObjectBase>();
		if (component == null)
		{
			component = collision.gameObject.transform.parent.GetComponent<ObjectBase>();
		}
		if (component != null)
		{
			if (trackedObj.Contains(component))
			{
				trackedObj.Remove(component);
			}
			if (swapper.GetCurrentGroup().listContains(component.gameObject) && !swapper.isFlipping)
			{
				swapper.GetCurrentGroup().removeObject(component);
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (trackedObj == null)
		{
			trackedObj = new List<ObjectBase>();
		}
		if (!collision.isTrigger)
		{
			ObjectBase component = collision.gameObject.GetComponent<ObjectBase>();
			if (component == null && collision.gameObject.transform.parent != null)
			{
				component = collision.gameObject.transform.parent.GetComponent<ObjectBase>();
			}
			if (component != null && !trackedObj.Contains(component) && !swapper.GetCurrentGroup().listContains(component.gameObject))
			{
				trackedObj.Add(component);
			}
		}
	}

	private void OnTriggerStay(Collider collision)
	{
		if (trackedObj == null)
		{
			trackedObj = new List<ObjectBase>();
		}
		if (!collision.isTrigger)
		{
			ObjectBase component = collision.gameObject.GetComponent<ObjectBase>();
			if (component == null && collision.gameObject.transform.parent != null)
			{
				component = collision.gameObject.transform.parent.GetComponent<ObjectBase>();
			}
			if (component != null && !trackedObj.Contains(component) && !swapper.GetCurrentGroup().listContains(component.gameObject))
			{
				trackedObj.Add(component);
			}
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		if (trackedObj == null)
		{
			trackedObj = new List<ObjectBase>();
		}
		if (collision.isTrigger)
		{
			return;
		}
		ObjectBase component = collision.gameObject.GetComponent<ObjectBase>();
		if (component == null && collision.gameObject.transform.parent != null)
		{
			component = collision.gameObject.transform.parent.GetComponent<ObjectBase>();
		}
		if (component != null)
		{
			if (trackedObj.Contains(component))
			{
				trackedObj.Remove(component);
			}
			if (swapper.GetCurrentGroup().listContains(component.gameObject) && !swapper.isFlipping)
			{
				swapper.GetCurrentGroup().removeObject(component);
			}
		}
	}

	public void clearList()
	{
		trackedObj.Clear();
	}
}
