using System.Collections;
using GVR.Gfx;
using UnityEngine;

public class boidAwaken : MonoBehaviour
{
	public Material lensColor;

	public BoidController mainSwarm;

	public ObjectBoid hugger;

	public EagerScene radioScene;

	public Transform whiteSceneObjectParent;

	public EnvironmentProperties whiteEnvironment;

	public MaterialUpdateController whiteMaterialProperties;

	public AudioClip lightsOn;

	public AudioClip boidOn;

	public AudioLoop radioAI;

	private bool flickering = true;

	private bool soundPlayed;

	private float flickerTime = -0.5f;

	private void Update()
	{
		flickerTime += Time.deltaTime;
		if (flickering)
		{
			if (flickerTime > 0f && !soundPlayed)
			{
				GetComponent<GvrAudioSource>().PlayOneShot(boidOn, 1f);
				soundPlayed = true;
			}
			if (Random.Range(0f, 2f) < flickerTime)
			{
				lensColor.SetColor("_Color", Color.white);
				hugger.eyeGlow.SetActive(value: true);
			}
			else
			{
				lensColor.SetColor("_Color", Color.black);
				hugger.eyeGlow.SetActive(value: false);
			}
			if ((double)flickerTime > 2.2)
			{
				flickering = false;
				hugger.enabled = true;
				StartCoroutine(turnLightsOn());
			}
		}
	}

	private IEnumerator turnLightsOn()
	{
		AudioSource.PlayClipAtPoint(lightsOn, transform.position, 1f);
		yield return new WaitForSeconds(0.3f);
		whiteSceneObjectParent.gameObject.SetActive(value: true);
		whiteEnvironment.Activate();
		whiteMaterialProperties.PushMaterials();
		radioScene.defaultMaterialProperties = whiteMaterialProperties;
		radioScene.mEnvironmentProperties = whiteEnvironment;
		if ((bool)radioAI)
		{
			radioAI.Stop();
		}
		yield return null;
	}
}
