using TButt;
using UnityEngine;

[RequireComponent(typeof(VVRConsumer))]
public class ObjectBlackHole : ObjectBase
{
	private ToolGroup _toolGroup;

	public GameObject sphereHolder;

	public VVRConsumer vacConsumer;

	private float blowForce;

	private float targetVolume;

	public bool suckOnly;

	private bool firstGrab;

	public ParticleSystem particleIn;

	public ParticleSystem particleOut;

	public float _maxSuctionForce = 100f;

	[Tooltip("Should probably start at 0,1 and end at 1,0\n to make attraction near the suctionOrigin highest")]
	[SerializeField]
	[CurveRange]
	private AnimationCurve _suctionForceDistanceCurve;

	[Tooltip("Suction will only occur if the direction towards the center of an edible is >= this\nWatch the white gizmo lines for reference")]
	[Range(-1f, 1f)]
	public float _suctionForceMaxFieldOfRange = 0.5f;

	[SerializeField]
	private Renderer vacLeashRenderer;

	[SerializeField]
	private Renderer blackHoleRenderer;

	private MaterialPropertyBlock vacLeashBlock;

	private MaterialPropertyBlock blackHoleBlock;

	private int transferringInterpId;

	private int outlineColorId;

	private int colorId;

	private int targetDistanceId;

	private int targetDistInterpId;

	private int leashMatDirId;

	private int leashMatNoiseAmpId;

	public Color inactiveOutlineColor;

	public Color upOutlineColor;

	public Color downOutlineColor;

	public bool overrideColorChanging;

	public Vector3? labelOrigin;

	[Space]
	[Header("SFX")]
	public AudioEvent vacOnSFX;

	public AudioLoop vacHumLoopSFX;

	public AudioEvent vacOffSFX;

	public AudioEvent vacEatSFX;

	public AudioLoop vacEatLoopSFX;

	public AudioEvent vacBuildSFX;

	public AudioLoop vacBuildLoopSFX;

	private float eatLoopTargetVolume;

	private Collider[] consumableColliders;

	private VVRConsumable eatObject;

	private float activeScalar;

	private float firstGrabT;

	private float firstGrabTimeout = 5f;

	private float pFill;

	private float progress;

	private float easeIn;

	private float easeOut;

	private float transferringInterp;

	private float minAmp;

	private float maxAmp;

	private float amp;

	private float targetAmp;

	private float dir;

	private float targetDir;

	private int nextLeashObject;

	private int nLeashObjects = 4;

	private Vector3[] leashObjects;

	private string[] leashObjPos = new string[4] { "_LeashObjPos0", "_LeashObjPos1", "_LeashObjPos2", "_LeashObjPos3" };

	private string[] leashObjAmp = new string[4] { "_LeashObjAmp0", "_LeashObjAmp1", "_LeashObjAmp2", "_LeashObjAmp3" };

	private float fillInterp;

	private bool isActive;

	private float forceWaver;

	private Vector3 toSuctionOrigin;

	private Vector3 toSuctionOriginNormalized;

	private float force;

	private float directionFraction;

	private float closenessToRange;

	private float distanceLerp;

	private float finalForce;

	private float distance;

	private float velocityLerp;

	private float massCutoff = 2f;

	private float minVacTime = 0.05f;

	private float minVacWorking;

	private VVRConsumable edible;

	private VVRConsumable currentEdible;

	public void Initialize(ToolGroup toolGroup)
	{
		_toolGroup = toolGroup;
	}

	protected override void Awake()
	{
		base.Awake();
		if (vacConsumer == null)
		{
			vacConsumer = gameObject.GetComponent<VVRConsumer>();
		}
		vacLeashBlock = new MaterialPropertyBlock();
		blackHoleBlock = new MaterialPropertyBlock();
	}

	protected override void Start()
	{
		base.Start();
		SetShaderIds();
		leashObjects = new Vector3[nLeashObjects];
		for (int i = 0; i < nLeashObjects; i++)
		{
			ref Vector3 reference = ref leashObjects[i];
			reference = Vector3.one;
		}
		amp = minAmp;
	}

	private void VacuumParticles()
	{
		if (blowForce < 0f)
		{
			if (!particleIn.isPlaying)
			{
				particleIn.Play();
			}
			particleOut.Stop();
		}
		else
		{
			if (!particleOut.isPlaying)
			{
				particleOut.Play();
			}
			particleIn.Stop();
		}
	}

	private void SetShaderIds()
	{
		leashMatDirId = Shader.PropertyToID("_Directionality");
		leashMatNoiseAmpId = Shader.PropertyToID("_NoiseAmp");
		transferringInterpId = Shader.PropertyToID("_TransferringInterp");
		outlineColorId = Shader.PropertyToID("_OutlineColor");
		colorId = Shader.PropertyToID("_Color");
		targetDistanceId = Shader.PropertyToID("_TargetDistance");
		targetDistInterpId = Shader.PropertyToID("_TargetDistInterp");
	}

	public override void GrabWith(Manipulator grabber)
	{
		grabbedBy.Add(grabber);
		if (grabbedBy.Count > 1)
		{
			grabber.Drop();
			grabber = grabbedBy[0];
		}
		if (_toolGroup == null)
		{
			_toolGroup = grabber.GrabbingToolGroup;
			vacConsumer.tg = _toolGroup;
			firstGrab = true;
		}
		if (vacLeashRenderer == null)
		{
			vacLeashRenderer = _toolGroup.Manipulator.grabBezier.line;
		}
		isGrabbed = true;
		_toolGroup.Manipulator.heldHighVector = Vector3.zero;
		_toolGroup.Manipulator.orientDuringGrab = true;
		if (onBecomeGrabbed != null)
		{
			onBecomeGrabbed.Invoke();
		}
		if ((bool)vacHumLoopSFX)
		{
			vacHumLoopSFX.Play(_toolGroup.Controller);
		}
		if ((bool)vacOnSFX)
		{
			vacOnSFX.Play(_toolGroup.Controller);
		}
		if ((bool)vacEatLoopSFX)
		{
			vacEatLoopSFX.Play(_toolGroup.Controller);
		}
		vacEatLoopSFX.SetSourceVolume(0f, _toolGroup.Controller);
		vacHumLoopSFX.SetSourceVolume(0f, _toolGroup.Controller);
		gameObject.GetComponent<SphereCollider>().enabled = false;
		undroppable = true;
		ungrabbable = true;
		_toolGroup.CursorReticle.ToggleOnOff(flag: false);
		_toolGroup.Manipulator.grabBezier.SetMaterial(_toolGroup.Manipulator.vacuumLeashMaterial);
		_toolGroup.Manipulator.tipMesh.SetActive(value: false);
		vacConsumer._nearFoodTrigger.enabled = true;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		grabbedBy.Remove(grabber);
		isGrabbed = false;
		OnDrop();
		vacConsumer.StopTransfer();
		if ((bool)vacEatLoopSFX)
		{
			vacEatLoopSFX.Stop(_toolGroup.Controller);
		}
		_toolGroup.CursorReticle.ToggleOnOff(flag: true);
		_toolGroup.Manipulator.grabBezier.SetMaterial(_toolGroup.Manipulator.defaultLeashMaterial);
		_toolGroup.Manipulator.tipMesh.SetActive(value: true);
	}

	public void Update()
	{
		if (isGrabbed)
		{
			if (firstGrab)
			{
				if ((bool)_toolGroup)
				{
					UpdateLeashMaterial();
				}
				if (TBInput.GetButtonUp(TBInput.Button.Action1, _toolGroup.Controller))
				{
					firstGrab = false;
				}
				firstGrabT += Time.deltaTime;
				if (firstGrabT > firstGrabTimeout)
				{
					firstGrab = false;
					blowForce = ((_toolGroup.EagerTouch.CurrentVacInputStatus() != EagerTouch.VacInputStatus.In && !suckOnly) ? 1f : (-1f));
					VacuumParticles();
					if (blowForce > 0f)
					{
						vacConsumer.StartTransfer(suckBool: false, oneShot: true);
					}
					else
					{
						vacConsumer.StartTransfer(suckBool: true, oneShot: true);
					}
					isActive = true;
					activeScalar = 0f;
				}
			}
			else
			{
				if (TBInput.GetButtonDown(TBInput.Button.Action1, _toolGroup.Controller) || TBInput.GetButtonDown(TBInput.Button.Action4, _toolGroup.Controller))
				{
					blowForce = ((_toolGroup.EagerTouch.CurrentVacInputStatus() != EagerTouch.VacInputStatus.In && !suckOnly) ? 1f : (-1f));
					VacuumParticles();
					if (blowForce > 0f)
					{
						vacConsumer.StartTransfer(suckBool: false, oneShot: true);
					}
					else
					{
						vacConsumer.StartTransfer(suckBool: true, oneShot: true);
					}
					isActive = true;
					activeScalar = 0f;
				}
				else if ((TBInput.GetButtonUp(TBInput.Button.Action1, _toolGroup.Controller) && blowForce < 0f) || (TBInput.GetButtonUp(TBInput.Button.Action1, _toolGroup.Controller) && TBCore.GetActivePlatform() != VRPlatform.OculusPC) || (TBInput.GetButtonUp(TBInput.Button.Action4, _toolGroup.Controller) && blowForce > 0f))
				{
					if (TBInput.GetButtonUp(TBInput.Button.Action1, _toolGroup.Controller) && blowForce < 0f && TBInput.GetButton(TBInput.Button.Action4, _toolGroup.Controller) && TBCore.GetActivePlatform() == VRPlatform.OculusPC)
					{
						blowForce = 1f;
						vacConsumer.StartTransfer(suckBool: false, oneShot: true);
						isActive = true;
						activeScalar = 0f;
					}
					else if (TBInput.GetButtonUp(TBInput.Button.Action4, _toolGroup.Controller) && blowForce > 0f && TBInput.GetButton(TBInput.Button.Action1, _toolGroup.Controller) && TBCore.GetActivePlatform() == VRPlatform.OculusPC)
					{
						blowForce = -1f;
						vacConsumer.StartTransfer(suckBool: true, oneShot: true);
						isActive = true;
						activeScalar = 0f;
					}
					else
					{
						isActive = false;
						vacConsumer.StopTransfer();
						particleOut.Stop();
						particleIn.Stop();
					}
				}
				if (_toolGroup.EagerTouch.CurrentVacInputStatus() == EagerTouch.VacInputStatus.In || _toolGroup.EagerTouch.CurrentVacInputStatus() == EagerTouch.VacInputStatus.Out)
				{
					targetVolume = Mathf.Lerp(targetVolume, Mathf.PerlinNoise(Time.time * 2f, 0f), 5f * Time.deltaTime);
				}
				else
				{
					targetVolume = Mathf.Lerp(targetVolume, 0f, 5f * Time.deltaTime);
				}
			}
		}
		if ((bool)_toolGroup)
		{
			eatLoopTargetVolume = Mathf.Lerp(eatLoopTargetVolume, 0f, Time.deltaTime * 3f);
			vacEatLoopSFX.SetSourceVolume(eatLoopTargetVolume, _toolGroup.Controller);
			vacHumLoopSFX.SetSourceVolume(targetVolume, _toolGroup.Controller);
			UpdateLeashMaterial();
		}
	}

	private void LateUpdate()
	{
		if ((bool)_toolGroup && _toolGroup.CursorReticle.rendering && isGrabbed)
		{
			_toolGroup.CursorReticle.ToggleOnOff(flag: false);
		}
	}

	private void UpdateLeashMaterial()
	{
		targetDir = (vacConsumer.isSucking ? 1 : (-1));
		dir = Mathf.Lerp(dir, targetDir, 13f * Time.deltaTime);
		vacLeashBlock.SetFloat(leashMatDirId, dir);
		blackHoleBlock.SetFloat(leashMatDirId, dir);
		minAmp = 0.0075f;
		maxAmp = 0.03f;
		targetAmp = ((!vacConsumer.isTransfering) ? minAmp : maxAmp);
		amp = Mathf.Lerp(amp, targetAmp, 13f * Time.deltaTime) * Mathf.Abs(dir);
		vacLeashBlock.SetFloat(leashMatNoiseAmpId, amp);
		float num = Mathf.Clamp01(_toolGroup.Manipulator.grabTargetDistance / ToolManager.AllVacuums.VacMaxLength);
		transferringInterp = (amp - minAmp) / (maxAmp - minAmp);
		vacLeashBlock.SetFloat(transferringInterpId, transferringInterp);
		blackHoleBlock.SetFloat(transferringInterpId, transferringInterp);
		if (!overrideColorChanging)
		{
			Color black = Color.black;
			Color black2 = Color.black;
			if (vacConsumer.isSucking)
			{
				black = Color.Lerp(Color.black, downOutlineColor, transferringInterp);
				black2 = Color.Lerp(inactiveOutlineColor, downOutlineColor, transferringInterp);
			}
			else
			{
				black = Color.Lerp(Color.black, upOutlineColor, transferringInterp);
				black2 = Color.Lerp(inactiveOutlineColor, upOutlineColor, transferringInterp);
			}
			vacLeashBlock.SetColor(outlineColorId, black2);
			blackHoleBlock.SetColor(outlineColorId, black2);
			vacLeashBlock.SetColor(colorId, black);
			blackHoleBlock.SetColor(colorId, black);
		}
		vacLeashBlock.SetFloat(targetDistanceId, _toolGroup.Manipulator.grabTargetDistance);
		vacLeashBlock.SetFloat(targetDistInterpId, num);
		blackHoleBlock.SetFloat(targetDistInterpId, num);
		float num2 = 1f;
		for (int i = 0; i < nLeashObjects; i++)
		{
			progress = Mathf.Clamp01(leashObjects[i].x + Time.deltaTime / 0.5f);
			ref Vector3 reference = ref leashObjects[i];
			reference = new Vector3(progress, leashObjects[i].y, leashObjects[i].z);
			easeIn = 1f - Mathf.Pow(1f - progress, 5f);
			easeOut = Mathf.Pow(1f - progress, 2.5f);
			vacLeashBlock.SetFloat(leashObjPos[i], Mathf.Lerp(leashObjects[i].y, 1f - leashObjects[i].y, easeOut));
			vacLeashBlock.SetFloat(leashObjAmp[i], easeIn * easeOut * 0.45f * leashObjects[i].z);
			float num3 = 0.25f;
			if (leashObjects[i].y > 0.5f)
			{
				num3 = 0.5f;
			}
			float num4 = 1f - Mathf.Clamp01(Mathf.Abs(progress - num3) / 0.25f);
			float b = 1f + num4 * Mathf.Lerp(1.25f, 2.5f, leashObjects[i].z);
			num2 = Mathf.Max(num2, b);
		}
		float num5 = Mathf.Lerp(0.08f, 0.1f, transferringInterp) * Mathf.Lerp(0.5f, 5f, num);
		num5 *= num2;
		blackHoleBlock.SetFloat(leashMatNoiseAmpId, num5);
		vacLeashRenderer.SetPropertyBlock(vacLeashBlock);
		blackHoleRenderer.SetPropertyBlock(blackHoleBlock);
	}

	private void IncrementLeashObject()
	{
		nextLeashObject++;
		nextLeashObject %= nLeashObjects;
	}

	public void TriggerLeashObjectAnimation(bool isSucking, VVRConsumable consumable)
	{
		if (!isSucking)
		{
			vacBuildSFX.Play(_toolGroup.Controller, rumble: true);
			fillInterp = Mathf.Lerp(0.1f, 0.4f, Mathf.Clamp01((consumable.totalVacuumValue - 10f) / 120f));
			ref Vector3 reference = ref leashObjects[nextLeashObject];
			reference = new Vector3(0f, 1f, fillInterp);
			IncrementLeashObject();
		}
		else
		{
			vacEatSFX.Play(_toolGroup.Controller, rumble: true);
			fillInterp = Mathf.Lerp(0.1f, 0.4f, Mathf.Clamp01((consumable.currentVacuumValueCapped - 10f) / 120f));
			ref Vector3 reference2 = ref leashObjects[nextLeashObject];
			reference2 = new Vector3(0f, 0f, fillInterp);
			IncrementLeashObject();
		}
		eatLoopTargetVolume = Mathf.Clamp(eatLoopTargetVolume + consumable.totalVacuumValue * 0.05f, 0f, 1.5f);
	}

	public override void onTeleport()
	{
	}

	private void OnEnable()
	{
		vacConsumer.OneShotEvent += TriggerLeashObjectAnimation;
	}

	private void OnDisable()
	{
		vacConsumer.OneShotEvent -= TriggerLeashObjectAnimation;
		vacConsumer.cnt = 0;
		for (int i = 0; i < vacConsumer._nearbyEdibles.Count; i++)
		{
			currentEdible = vacConsumer._nearbyEdibles[i];
			if (currentEdible != null)
			{
				currentEdible.onExitVacuumRange(vacConsumer);
			}
		}
		vacConsumer._nearbyEdibles.Clear();
	}

	private void FixedUpdate()
	{
		if (!isActive)
		{
			return;
		}
		if (activeScalar < 1f)
		{
			activeScalar += Time.deltaTime * 2f;
		}
		minVacWorking += Time.fixedDeltaTime;
		if (!(minVacWorking > minVacTime))
		{
			return;
		}
		minVacWorking = 0f;
		for (int i = 0; i < vacConsumer._nearbyEdibles.Count; i++)
		{
			edible = vacConsumer._nearbyEdibles[i];
			if (edible != null && edible.rb != null && edible.rb.mass < massCutoff && !edible.dontStartNewTransfer)
			{
				toSuctionOrigin = edible.transform.position - vacConsumer.mouthTransform.position;
				distance = toSuctionOrigin.magnitude;
				distanceLerp = Mathf.Clamp01(distance / 2.5f);
				velocityLerp = 1f - Mathf.Clamp01(distance / 1f);
				toSuctionOriginNormalized = Vector3.Normalize(toSuctionOrigin);
				force = _suctionForceDistanceCurve.Evaluate(distanceLerp);
				forceWaver = Mathf.PerlinNoise(Time.time * 2f, 0f);
				finalForce = force * forceWaver * _maxSuctionForce * blowForce * activeScalar;
				edible.rb.AddForce(toSuctionOriginNormalized * finalForce);
				edible.rb.velocity = Vector3.Lerp(edible.rb.velocity, toSuctionOriginNormalized * finalForce, velocityLerp);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		if ((bool)vacConsumer && (bool)vacConsumer.mouthTransform)
		{
			Gizmos.DrawWireSphere(vacConsumer.mouthTransform.position, vacConsumer._mouthRadius);
			float num = Mathf.Acos(_suctionForceMaxFieldOfRange) * 57.29578f;
			Quaternion quaternion = Quaternion.AngleAxis(0f - num, vacConsumer.mouthTransform.up);
			Quaternion quaternion2 = Quaternion.Inverse(quaternion);
			Quaternion quaternion3 = Quaternion.AngleAxis(0f - num, vacConsumer.mouthTransform.right);
			Quaternion quaternion4 = Quaternion.Inverse(quaternion3);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion * vacConsumer.mouthTransform.forward * 2f);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion2 * vacConsumer.mouthTransform.forward * 2f);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion3 * vacConsumer.mouthTransform.forward * 2f);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion4 * vacConsumer.mouthTransform.forward * 2f);
		}
	}
}
