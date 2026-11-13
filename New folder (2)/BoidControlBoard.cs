using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class BoidControlBoard : MonoBehaviour
{
	public BoidController swarm;

	public HackingTool hackingTool;

	public GameObject spawnButton;

	public TextSwapper powerButton;

	public TextSwapper attackButton;

	public TextSwapper sendAway;

	public TextSwapper bringOut;

	public Transform courier;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve courierEntryCurve;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve courierExitCurve;

	private float courierYMax = 90f;

	private float courierDuration = 2.5f;

	public VVR_Point pointLight;

	private bool power = true;

	private bool attack;

	private bool dock = true;

	private SafeCoroutine courierThread;

	private SafeCoroutine swarmManagerThread;

	private void Awake()
	{
		hackingTool.gameObject.SetActive(value: false);
		hackingTool.controlBoard = this;
		if (swarm.swarmBehavior == ObjectBoid.behaviorMode.roost)
		{
			courier.position = new Vector3(courier.position.x, courierYMax, courier.position.z);
			dock = false;
		}
	}

	private void Update()
	{
		if (swarm.swarmBehavior == ObjectBoid.behaviorMode.roost && dock)
		{
			if (swarm.toConstruct.Count > 0)
			{
				swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.construct);
			}
			else if (swarm.toDeconstruct.Count > 0)
			{
				swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.deconstruct);
			}
		}
		else if (swarm.swarmBehavior == ObjectBoid.behaviorMode.construct)
		{
			if (swarm.toConstruct.Count < 1)
			{
				if (swarm.toDeconstruct.Count > 0)
				{
					swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.deconstruct);
				}
				else
				{
					swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.roost);
				}
			}
		}
		else
		{
			if (swarm.swarmBehavior != ObjectBoid.behaviorMode.deconstruct)
			{
				return;
			}
			DeconstructionSwarmManager();
			if (swarm.toDeconstruct.Count < 1)
			{
				if (swarm.toConstruct.Count > 0)
				{
					swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.construct);
				}
				else
				{
					swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.roost);
				}
			}
		}
	}

	public void HackToggle()
	{
		ToolManager.ToggleManipulator(hackingTool.gameObject.activeSelf);
		hackingTool.gameObject.SetActive(!hackingTool.gameObject.activeSelf);
		spawnButton.SetActive(hackingTool.gameObject.activeSelf);
		if (hackingTool.gameObject.activeSelf)
		{
			BringOut();
		}
		else
		{
			SendAway();
		}
		if (swarm.toConstruct != hackingTool.hackToConstruct)
		{
			swarm.toConstruct = hackingTool.hackToConstruct;
		}
		if (swarm.toDeconstruct != hackingTool.hackToDeconstruct)
		{
			swarm.toDeconstruct = hackingTool.hackToDeconstruct;
			for (int i = 0; i < swarm.boidList.Count; i++)
			{
				VVRConsumer component = swarm.boidList[i].GetComponent<VVRConsumer>();
				component.capacity = 150f;
				component.fill = 75f;
			}
		}
	}

	public void SpawnToggle()
	{
		if (hackingTool.CurrToolMode == HackingTool.ToolMode.spawn)
		{
			hackingTool.CurrToolMode = HackingTool.ToolMode.delete;
		}
		else if (hackingTool.CurrToolMode == HackingTool.ToolMode.delete)
		{
			hackingTool.CurrToolMode = HackingTool.ToolMode.spawn;
		}
	}

	public void PowerToggle()
	{
		if (power)
		{
			swarm.PowerDownAll();
			power = false;
			powerButton.UseAltString("Power On");
		}
		else
		{
			swarm.PowerUpAll();
			power = true;
			powerButton.UseAltString("Power Off");
		}
	}

	public void AttackToggle()
	{
		if (attack)
		{
			swarm.canAttack = false;
			swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.construct);
			attack = false;
			attackButton.UseAltString("Attack");
		}
		else
		{
			swarm.canAttack = true;
			swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.chase);
			attack = true;
			attackButton.UseAltString("Calm");
		}
	}

	public void BringOut()
	{
		if (!dock && (courierThread == null || !courierThread.IsRunning))
		{
			courierThread = GlobalCoroutineContainer.AddThread(CourierEnter(), this);
		}
	}

	private IEnumerator CourierEnter()
	{
		float t = 0f;
		while (t < courierDuration)
		{
			courier.position = new Vector3(courier.position.x, courierEntryCurve.Evaluate(t / courierDuration) * courierYMax, courier.position.z);
			t += Time.deltaTime;
			yield return null;
		}
		swarm.ambientRoostAlternate = false;
		dock = true;
		yield return null;
	}

	public void SendAway()
	{
		if (dock && (courierThread == null || !courierThread.IsRunning))
		{
			GlobalCoroutineContainer.AddThread(CourierExit(), this);
		}
	}

	private IEnumerator CourierExit()
	{
		SafeDebug.Log("boids to roost");
		swarm.ambientRoostAlternate = false;
		swarm.SetSwarmBehavior(ObjectBoid.behaviorMode.roost);
		bool waiting;
		do
		{
			waiting = false;
			yield return null;
			for (int i = 0; i < swarm.boidList.Count; i++)
			{
				if (swarm.boidList[i].currentAction != ObjectBoid.actionMode.sitting)
				{
					waiting = true;
				}
			}
		}
		while (waiting);
		SafeDebug.Log("all boids roosted");
		float t = 0f;
		while (t < courierDuration)
		{
			courier.position = new Vector3(courier.position.x, courierExitCurve.Evaluate(t / courierDuration) * courierYMax, courier.position.z);
			t += Time.deltaTime;
			yield return null;
		}
		SafeDebug.Log("roost away");
		dock = false;
		yield return null;
	}

	private void DeconstructionSwarmManager()
	{
		if (swarmManagerThread == null || !swarmManagerThread.IsRunning)
		{
			swarmManagerThread = GlobalCoroutineContainer.AddThread(deconstructionCoroutine(), this);
		}
	}

	private IEnumerator deconstructionCoroutine()
	{
		int eatIndex = 0;
		float t = 0f;
		float swapT = 5f;
		float origND = swarm.neighborDist;
		float origRC = swarm.rotationCoeff;
		float origV = swarm.velocity;
		swarm.neighborDist = 1.1f;
		swarm.rotationCoeff = 12f;
		swarm.velocity = 3.5f;
		while (swarm.swarmBehavior == ObjectBoid.behaviorMode.deconstruct)
		{
			for (int i = 0; i < swarm.boidList.Count; i++)
			{
				if (i == eatIndex || i == eatIndex + 1)
				{
					swarm.boidList[i].SetBehaviorMode(ObjectBoid.behaviorMode.deconstruct);
				}
				else if (swarm.toDeconstruct != null && swarm.toDeconstruct.Count > 0)
				{
					swarm.boidList[i].SetBehaviorMode(ObjectBoid.behaviorMode.ambient);
					swarm.transform.parent.position = swarm.toDeconstruct[0].transform.position + Vector3.up * (swarm.toDeconstruct[0].GetComponent<Collider>().bounds.max.y + 0.5f);
				}
			}
			t += Time.deltaTime;
			if (t > swapT)
			{
				eatIndex++;
				t = 0f;
			}
			yield return null;
		}
		swarm.neighborDist = origND;
		swarm.rotationCoeff = origRC;
		swarm.velocity = origV;
	}
}
