using UnityEngine;

public class GridSlot : MonoBehaviour
{
	public GameObject mHeadset;

	private Renderer mRenderer;

	[SerializeField]
	private Material activeMat;

	[SerializeField]
	private Material inactiveMat;

	protected void Awake()
	{
		mRenderer = GetComponent<Renderer>();
	}

	protected void Start()
	{
		SetSlotState(GridSlotState.inactive);
	}

	public void SetSlotState(GridSlotState newState)
	{
		switch (newState)
		{
		case GridSlotState.active:
			mRenderer.sharedMaterial = activeMat;
			break;
		case GridSlotState.inactive:
			mRenderer.sharedMaterial = inactiveMat;
			break;
		}
	}
}
