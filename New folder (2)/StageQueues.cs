using UnityEngine;

public class StageQueues : MonoBehaviour
{
	public BoidController boidController;

	public Material sphereMat;

	public GameObject nestingDoll;

	public VVRConsumer sphereVac;

	private float SphereUpdateTimer;

	private float SphereUpdateThreshold = 0.5f;

	private bool scaleUpNesting;

	private void Start()
	{
		nestingDoll.transform.localScale = Vector3.zero;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			GameObject gameObject = Object.Instantiate(new GameObject());
			gameObject.transform.parent = EagerScenes.instance.ActiveScene.transform;
			gameObject.name = "BOIDSWARM";
			for (int i = 0; i < 15; i++)
			{
				boidController.Spawn(new Vector3(0f, 5f, -15f) + Random.insideUnitSphere * 2f);
			}
			SafeDebug.Log("madeBoids");
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			boidController.SetSwarmBehavior(ObjectBoid.behaviorMode.chase);
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			scaleUpNesting = true;
			nestingDoll.SetActive(value: true);
		}
		if (scaleUpNesting)
		{
			if (nestingDoll.transform.localScale.x > 0.99f)
			{
				nestingDoll.transform.localScale = Vector3.one;
				scaleUpNesting = false;
			}
			nestingDoll.transform.localScale = Vector3.Lerp(nestingDoll.transform.localScale, Vector3.one, 5f * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.M))
		{
			SphereUpdateTimer += Time.deltaTime;
			if (SphereUpdateTimer > SphereUpdateThreshold)
			{
				sphereMat.SetFloat("_SuckStrength", 0.2f + Mathf.PerlinNoise(Time.time * 2f, 0f));
				SphereUpdateTimer = 0f;
				SphereUpdateThreshold = Random.Range(0f, 0.2f);
			}
			sphereMat.SetFloat("_NoiseSpeed", 10f + 150f * Mathf.PerlinNoise(Time.time * 5f, 1f));
			sphereMat.SetFloat("_NoiseHeight", 0.1f * (0.5f - Mathf.PerlinNoise(Time.time * 3f, 2f)));
			sphereVac._nearFoodTrigger.enabled = true;
			sphereVac.restPower = Random.Range(0.3f, 1f);
			sphereVac.restRange = 2.2f + Mathf.PerlinNoise(Time.time, 0f) * 1f;
			sphereVac.activeRange = 2.3f;
			sphereVac.activePower = 2f;
			sphereVac.suckRate = -50f;
			sphereVac._mouthRadius = 1.5f;
		}
		else
		{
			sphereMat.SetFloat("_SuckStrength", 1f);
			sphereMat.SetFloat("_NoiseSpeed", 50f);
			sphereMat.SetFloat("_NoiseHeight", -0.003f);
			sphereMat.SetFloat("_SuckHeight", 0.03f);
			SphereUpdateTimer = 0f;
			sphereVac.restPower = Mathf.Lerp(sphereVac.restPower, 0.3f, 2f * Time.deltaTime);
			sphereVac.restRange = Mathf.Lerp(sphereVac.restRange, 0.5f, 2f * Time.deltaTime);
			sphereVac.activeRange = 1f;
			sphereVac.activePower = 2f;
			sphereVac.suckRate = -10f;
			sphereVac._mouthRadius = 0.4f;
		}
	}
}
