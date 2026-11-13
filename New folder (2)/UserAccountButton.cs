using TMPro;
using UnityEngine;

public class UserAccountButton : MonoBehaviour
{
	public delegate void UserButtonClickAction(UserAccountButton button);

	[SerializeField]
	private float aestheticAngVel = 60f;

	[Header("Scene References")]
	[SerializeField]
	private VVR_Point pointLight;

	[SerializeField]
	private Transform aestheticTransitionScalingLayer;

	[SerializeField]
	private Transform newUserTransitionScalingLayer;

	[SerializeField]
	private Transform aestheticPositionOffsetLayer;

	[SerializeField]
	private Transform aestheticHoverScaleLayer;

	[SerializeField]
	private Transform newUserHoverScaleLayer;

	[SerializeField]
	private Transform aestheticRotationLayer;

	[SerializeField]
	private MeshFilter aestheticMesh;

	[SerializeField]
	private MeshFilter aestheticMeshBackground;

	[SerializeField]
	private Transform glowCardHoverScaleLayer;

	[SerializeField]
	private Transform glowCardRoot;

	[SerializeField]
	internal TextSwapper userLabel;

	[SerializeField]
	private TextMeshPro adminLabel;

	[SerializeField]
	private ObjectButton mButton;

	[SerializeField]
	private MeshRenderer newPlayerIconRenderer;

	[Header("Aesthetic Tweakables")]
	[SerializeField]
	private float glowCardOffset;

	[SerializeField]
	private float plantScale = 5f;

	[SerializeField]
	private Vector3 plantOffset;

	[SerializeField]
	private float ballScale = 5f;

	[SerializeField]
	private Vector3 ballOfset;

	[SerializeField]
	private float fishScale = 6.5f;

	[SerializeField]
	private Vector3 fishOffset;

	[Header("Transformation Tweakables")]
	[SerializeField]
	private float scaleOutZPush = 2f;

	[SerializeField]
	private float aestheticHoverScaleMultiplier = 1.4f;

	[SerializeField]
	private float newUserHoverScaleMultiplier = 1.2f;

	[SerializeField]
	private float glowCardHoverScaleMultiplier = 2f;

	[SerializeField]
	private float glowCardSelectedScaleMultiplier = 3f;

	[SerializeField]
	private float hoverScaleTime = 0.09f;

	[Header("Light Tweakables")]
	[SerializeField]
	private float hoverPointLightMultiplier = 1.4f;

	[SerializeField]
	private float newUserPointLightIntensity = 0.6f;

	[SerializeField]
	private float newUserPointLightRange = 1.17f;

	[SerializeField]
	private float existingUserPointLightIntensity = 0.6f;

	[SerializeField]
	private float existingUserPointLightRange = 1.17f;

	[SerializeField]
	private float selectedUserPointLightIntensity = 0.6f;

	[SerializeField]
	private float selectedUserPointLightRange = 1.17f;

	private UserButtonClickAction mClickAction;

	private VVR_SAVEDATA mSave;

	private int saveSlot;

	private bool isNewUserButton;

	private bool isHoverFocused;

	private float hoverScalingT;

	private float hoverScalingVelocity;

	private float selectedT;

	private bool selectingIn;

	public void Awake()
	{
		mButton.onHoverEnter.AddListener(() =>
		{
			isHoverFocused = true;
		});
		mButton.onHoverExit.AddListener(() =>
		{
			isHoverFocused = false;
		});
		mButton.onClickEvent.AddListener(() =>
		{
			executeClick();
		});
	}

	public void InitializeWithSave(VVR_SAVEDATA saveIn)
	{
		gameObject.SetActive(value: true);
		mSave = saveIn;
		isNewUserButton = saveIn == null;
		if (isNewUserButton)
		{
			aestheticRotationLayer.gameObject.SetActive(value: false);
			aestheticMeshBackground.gameObject.SetActive(value: false);
			newPlayerIconRenderer.gameObject.SetActive(value: true);
			glowCardRoot.gameObject.SetActive(value: false);
			userLabel.SetNewKey("TEXT_243");
		}
		else
		{
			mSave.WriteMissingDictionaryValues();
			aestheticRotationLayer.gameObject.SetActive(value: true);
			aestheticMeshBackground.gameObject.SetActive(value: true);
			newPlayerIconRenderer.gameObject.SetActive(value: false);
			aestheticMesh.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(saveIn.aestheticObject, mSave.arbitraryIntDictionary["BEE SLOT"]);
			if (saveIn.aestheticObject == TutorialLevelSequence.AestheticObjects.Ball)
			{
				aestheticPositionOffsetLayer.transform.localPosition = ballOfset;
				aestheticMesh.transform.localScale = Vector3.one * ballScale;
			}
			else if (saveIn.aestheticObject == TutorialLevelSequence.AestheticObjects.Plant)
			{
				aestheticPositionOffsetLayer.transform.localPosition = plantOffset;
				aestheticMesh.transform.localScale = Vector3.one * plantScale;
			}
			else if (saveIn.aestheticObject == TutorialLevelSequence.AestheticObjects.Fish)
			{
				aestheticPositionOffsetLayer.transform.localPosition = fishOffset;
				aestheticMesh.transform.localScale = Vector3.one * fishScale;
			}
			aestheticMeshBackground.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticBackgroundSphere(saveIn.aestheticObject, mSave.arbitraryIntDictionary["BEE SLOT"]);
			userLabel.UseAltString(mSave.arbitraryStringDictionary["USERNAME"]);
		}
		ShowText(show: true);
	}

	internal void SetButtonAction(UserButtonClickAction newAction)
	{
		mClickAction = newAction;
		mButton.ungrabbable = newAction == null;
	}

	public void ShowText(bool show)
	{
		userLabel.gameObject.SetActive(show);
		adminLabel.gameObject.SetActive(show && !isNewUserButton && mSave != null && mSave.hasAdminAccess);
	}

	public void ScaleInTransition(float t)
	{
		aestheticTransitionScalingLayer.transform.localScale = Vector3.one * t;
		aestheticTransitionScalingLayer.transform.localPosition = Vector3.forward * (1f - t) * scaleOutZPush;
		newUserTransitionScalingLayer.transform.localScale = Vector3.one * t;
		mButton.SetCollidersEnabled(t.AlmostEquals(1f));
		mButton.ungrabbable = !t.AlmostEquals(1f) || mClickAction == null;
	}

	public void SelectTransition(float t, bool inOrOut)
	{
		selectedT = t;
		selectingIn = inOrOut;
	}

	public void TransformIntoNewUserButtonTransition(float t)
	{
		newPlayerIconRenderer.gameObject.SetActive(value: true);
		aestheticTransitionScalingLayer.transform.localScale = Vector3.one * (1f - t);
		newUserTransitionScalingLayer.transform.localScale = Vector3.one * 1f;
		mButton.ungrabbable = !t.AlmostEquals(1f) || mClickAction == null;
		if (t.AlmostEquals(1f))
		{
			mButton.ungrabbable = false;
			InitializeWithSave(null);
		}
	}

	private void executeClick()
	{
		if (mClickAction != null)
		{
			mClickAction(this);
		}
	}

	private void Update()
	{
		aestheticRotationLayer.transform.Rotate(Vector3.up, aestheticAngVel * Time.deltaTime);
		hoverScalingT = Mathf.SmoothDamp(hoverScalingT, ((!isHoverFocused || mButton.ungrabbable) && !selectingIn) ? 0f : 1f, ref hoverScalingVelocity, hoverScaleTime);
		aestheticHoverScaleLayer.transform.localScale = Vector3.one * Mathf.Lerp(1f, aestheticHoverScaleMultiplier, hoverScalingT);
		newUserHoverScaleLayer.transform.localScale = Vector3.one * Mathf.Lerp(1f, newUserHoverScaleMultiplier, hoverScalingT);
		updateGlowCard();
		updatePointLights();
	}

	private void updateGlowCard()
	{
		Vector3 normalized = (aestheticTransitionScalingLayer.position - EagerMaster.instance.mainCamera.transform.position).normalized;
		glowCardRoot.position = aestheticTransitionScalingLayer.position + glowCardOffset * normalized;
		glowCardRoot.rotation = Quaternion.LookRotation(-normalized);
		if (selectingIn)
		{
			glowCardHoverScaleLayer.transform.localScale = Vector3.one * Mathf.Lerp(glowCardHoverScaleMultiplier, glowCardSelectedScaleMultiplier, selectedT);
		}
		else if (selectedT.AlmostEquals(0f))
		{
			glowCardHoverScaleLayer.transform.localScale = Vector3.one * Mathf.Lerp(1f, glowCardHoverScaleMultiplier, hoverScalingT);
		}
		else
		{
			glowCardHoverScaleLayer.transform.localScale = Vector3.one * Mathf.Lerp(1f, glowCardSelectedScaleMultiplier, selectedT);
		}
	}

	private void updatePointLights()
	{
		pointLight.intensity = ((!isNewUserButton) ? aestheticTransitionScalingLayer.localScale.x : newUserTransitionScalingLayer.localScale.x) * ((!isNewUserButton) ? Mathf.Lerp(existingUserPointLightIntensity, selectedUserPointLightIntensity, selectedT) : newUserPointLightIntensity) * Mathf.Lerp(1f, hoverPointLightMultiplier, hoverScalingT) * ((!pointLight.gameObject.activeInHierarchy) ? 0f : 1f);
		pointLight.range = ((!isNewUserButton) ? aestheticTransitionScalingLayer.localScale.x : newUserTransitionScalingLayer.localScale.x) * ((!isNewUserButton) ? Mathf.Lerp(existingUserPointLightRange, selectedUserPointLightRange, selectedT) : newUserPointLightRange) * Mathf.Lerp(1f, hoverPointLightMultiplier, hoverScalingT) * ((!pointLight.gameObject.activeInHierarchy) ? 0f : 1f);
	}
}
