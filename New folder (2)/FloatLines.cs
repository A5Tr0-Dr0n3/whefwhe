using UnityEngine;

public class FloatLines : MonoBehaviour
{
	public Transform floatAnchor;

	public float boidYOffset = 0.7f;

	public float floatXOffset = 0.45f;

	public float floatYOffset = -0.5f;

	public float floatZOffset = 0.2f;

	public ObjectBoid boid1;

	public ObjectBoid boid2;

	public ObjectBoid boid3;

	public ObjectBoid boid4;

	public LineRenderer line1;

	public LineRenderer line2;

	public LineRenderer line3;

	public LineRenderer line4;

	private float xMod;

	private float zMod;

	private Vector3 boidOffset;

	private Vector3 floatOffset;

	private void Start()
	{
		LineRenderer lineRenderer = line1;
		int num = 2;
		line4.positionCount = num;
		num = num;
		line3.positionCount = num;
		num = num;
		line2.positionCount = num;
		lineRenderer.positionCount = num;
	}

	private void Update()
	{
		boidOffset = new Vector3(0f, boidYOffset, 0f);
		floatOffset = new Vector3(floatXOffset * -1f, floatYOffset, floatZOffset * -1f);
		line1.SetPosition(0, boid1.transform.position + boid1.transform.rotation * boidOffset);
		line1.SetPosition(1, floatAnchor.position + floatAnchor.rotation * floatOffset);
		floatOffset = new Vector3(floatXOffset * 1f, floatYOffset, floatZOffset * -1f);
		line2.SetPosition(0, boid2.transform.position + boid2.transform.rotation * boidOffset);
		line2.SetPosition(1, floatAnchor.position + floatAnchor.rotation * floatOffset);
		floatOffset = new Vector3(floatXOffset * -1f, floatYOffset, floatZOffset * 1f);
		line3.SetPosition(0, boid3.transform.position + boid3.transform.rotation * boidOffset);
		line3.SetPosition(1, floatAnchor.position + floatAnchor.rotation * floatOffset);
		floatOffset = new Vector3(floatXOffset * 1f, floatYOffset, floatZOffset * 1f);
		line4.SetPosition(0, boid4.transform.position + boid4.transform.rotation * boidOffset);
		line4.SetPosition(1, floatAnchor.position + floatAnchor.rotation * floatOffset);
	}
}
