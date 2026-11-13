using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class PingPongMatch : MonoBehaviour
{
	public ChazController chazA;

	public ChazController chazB;

	public ChazController chazRef;

	public GameObject ball;

	public Transform serviceA;

	public Transform serviceB;

	public GameObject colliderParent;

	public Rigidbody paddleA;

	public Rigidbody paddleB;

	public PhysicMaterial superBouncy;

	public PhysicMaterial kindaBouncy;

	private float pointThreshold = 10f;

	private float t;

	private bool chazAServing = true;

	private Rigidbody ballBody;

	private bool matchOver;

	private Collider ballCollider;

	private ChazController server;

	private ChazController receiver;

	private Transform serviceT;

	private Transform receiverT;

	private Rigidbody serverPaddle;

	private SafeCoroutine serviceRoutine;

	private void Awake()
	{
		ballBody = ball.GetComponent<Rigidbody>();
		pointThreshold = Random.Range(0.1f, 1.1f);
		ballCollider = ball.GetComponent<Collider>();
		ballCollider.material = superBouncy;
	}

	private void Update()
	{
		if (!matchOver)
		{
			t += Time.deltaTime;
			if (t > pointThreshold)
			{
				newPoint();
				t = 0f;
				pointThreshold = Random.Range(5f, 13f);
			}
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			EndMatch();
		}
	}

	public void EndMatch()
	{
		if (serviceRoutine != null && serviceRoutine.IsRunning)
		{
			serviceRoutine.Stop();
		}
		matchOver = true;
		colliderParent.SetActive(value: false);
		ballCollider.material = kindaBouncy;
	}

	private void newPoint()
	{
		chazAServing = !chazAServing;
		server = ((!chazAServing) ? chazB : chazA);
		receiver = (chazAServing ? chazB : chazA);
		serviceT = ((!chazAServing) ? serviceB : serviceA);
		receiverT = (chazAServing ? serviceB : serviceA);
		serverPaddle = ((!chazAServing) ? paddleB : paddleA);
		chazA.EyeLookAt(ball.transform);
		chazB.EyeLookAt(ball.transform);
		if (chazRef != null)
		{
			chazRef.EyeLookAt(ball.transform);
		}
		server.LazerObject(ball);
		receiver.ReStraighten();
		if (serviceRoutine != null && serviceRoutine.IsRunning)
		{
			serviceRoutine.Stop();
		}
		serviceRoutine = GlobalCoroutineContainer.AddThread(Service(), this);
	}

	private IEnumerator Service()
	{
		while (ball.activeSelf)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.1f, this);
		ballBody.velocity = Vector3.zero;
		ball.transform.position = serviceT.position;
		ball.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.6f, this);
		server.ReStraighten();
		ballBody.AddForce(Vector3.up * Random.Range(0.2f, 0.6f), ForceMode.Impulse);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		serverPaddle.AddForce((receiverT.position - serviceT.position).normalized * Random.Range(6.5f, 8.8f), ForceMode.Impulse);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
		ballBody.AddForce((receiverT.position - serviceT.position).normalized * Random.Range(1.5f, 2.5f), ForceMode.Impulse);
		ballBody.AddForce(Random.onUnitSphere * Random.Range(0.5f, 0.7f), ForceMode.Impulse);
		serviceRoutine = null;
	}
}
