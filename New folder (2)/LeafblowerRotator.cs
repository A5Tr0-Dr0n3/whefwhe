using UnityEngine;

public class LeafblowerRotator : MonoBehaviour
{
	public ObjectLeafBlower[] Leafblowers;

	public RotateObject Rotator;

	public Vector3 BlowerOffRotation;

	public Vector3 BlowerOnRotation;

	public float MinDot;

	public float MaxDot;

	public float FarDistance;

	public float CloseDistance;

	public float MaxSpeedChange = 10f;

	private void Update()
	{
		float num = 0f;
		float num2 = 1f;
		for (int i = 0; i < Leafblowers.Length; i++)
		{
			if (Leafblowers[i].IsBlowing)
			{
				ObjectLeafBlower objectLeafBlower = Leafblowers[i];
				Vector3 normalized = (objectLeafBlower.transform.position - transform.position).normalized;
				float num3 = Vector3.Dot(normalized, objectLeafBlower.transform.forward);
				num = Mathf.Clamp01((num3 - MinDot) / (MaxDot - MinDot));
				float num4 = Vector3.Distance(objectLeafBlower.transform.position, transform.position);
				num2 = 1f - Mathf.Clamp01((num4 - CloseDistance) / (FarDistance - CloseDistance));
			}
		}
		Vector3 vector = Vector3.Lerp(BlowerOffRotation, BlowerOnRotation, num * num2);
		Vector3 vector2 = vector - Rotator.rotation;
		vector2 = Mathf.Min(vector2.magnitude, MaxSpeedChange) * vector2.normalized * Time.deltaTime;
		Rotator.rotation += vector2;
	}
}
