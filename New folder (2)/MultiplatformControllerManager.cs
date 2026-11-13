using TMPro;
using UnityEngine;

public class MultiplatformControllerManager : MonoBehaviour
{
	[Header("Controller References")]
	public GameObject controllerGo;

	[Header("Vacuum Animations")]
	public bool flipHasDirection = true;

	public AnimationClip transformRightAnimation;

	public AnimationClip unTransformRightAnimation;

	[Header("Hacked Controller Refs")]
	public MeshRenderer hackedControllerTop;

	public MeshRenderer hackedControllerBottom;

	public GameObject hackedControllerRoot;

	public GameObject vacFillUi;

	public TextMeshPro fillText;

	[Header("Tweakables")]
	public Vector3 leashTipOffset = Vector3.zero;
}
