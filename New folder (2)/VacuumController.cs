using UnityEngine;

public class VacuumController : MonoBehaviour
{
	public GameObject touchpadUp;

	public GameObject touchpadDown;

	public GameObject blueGlowCard;

	public GameObject redGlowCard;

	public GameObject[] originalTouchpad;

	public GameObject helloBeeText;

	public bool rotatesForSuck;

	public Vector3 suckEuler;

	public bool rotatesForBlow;

	public Vector3 blowEuler;

	public Renderer topButton;

	public Material highlightMat;

	public Material startMat;
}
