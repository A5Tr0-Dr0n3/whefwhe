using System.Collections.Generic;
using UnityEngine;

public class WordsSceneManager : MonoBehaviour
{
	public string sentence1 = "Where";

	public List<string> sentence1List;

	public List<GameObject> deadWords;

	public Transform spawnPoint;

	public Material groundMat;

	public ObjectSpawner objSpawn;

	private int stringIndex;

	private Vector3 randomScale;

	private GameObject newText;

	private GameObject spawnedText;

	private Material textMaterial;

	private Color tColor;

	private float startHue;

	private float startSaturation;

	private float startValue;

	private float hue;

	private void Start()
	{
		sentence1 = sentence1.ToLower();
		sentence1List = new List<string>(sentence1.Split(' '));
		CreateNewWord();
	}

	private void Update()
	{
		if (spawnedText.transform.localScale.magnitude < Vector3.one.magnitude)
		{
			spawnedText.transform.localScale = Utility.TCVRLerp(spawnedText.transform.localScale, Vector3.one * 1.1f, 5f);
		}
		else if (!spawnedText.GetComponent<Collider>().enabled)
		{
			spawnedText.GetComponent<Collider>().enabled = true;
			spawnedText.GetComponent<Rigidbody>().isKinematic = false;
			spawnedText.AddComponent<ObjectBase>();
			spawnedText.AddComponent<FixedJoint>();
			spawnedText.GetComponent<FixedJoint>().breakForce = 30f;
			spawnedText.GetComponent<FixedJoint>().breakTorque = 30f;
		}
		else if (!spawnedText.GetComponent<FixedJoint>())
		{
			deadWords.Add(spawnedText);
			CreateNewWord();
		}
	}

	private void UpdateDeadWords()
	{
		foreach (GameObject deadWord in deadWords)
		{
			randomScale = new Vector3(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f));
			deadWord.transform.localScale = randomScale;
		}
	}

	private void CreateNewWord()
	{
		if (stringIndex >= sentence1List.Count)
		{
			return;
		}
		newText = FlyingText.GetObject(sentence1List[stringIndex]);
		spawnedText = newText;
		spawnedText.transform.position = spawnPoint.position;
		spawnedText.transform.rotation = spawnPoint.rotation;
		spawnedText.transform.localScale = Vector3.zero;
		spawnedText.GetComponent<Rigidbody>().isKinematic = true;
		spawnedText.GetComponent<Collider>().enabled = false;
		if (stringIndex > 0)
		{
			if (stringIndex < 5)
			{
				RandomChangeHue(spawnedText.GetComponent<Renderer>().material);
			}
			else
			{
				int num = 0;
				num = Random.Range(0, 4);
				Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
				spawnedText.GetComponent<Renderer>().sharedMaterial = FlyingText.GetObject(sentence1List[num]).GetComponent<Renderer>().sharedMaterial;
				spawnedText.GetComponent<Renderer>().sharedMaterial.color = color;
			}
			float value = RandomChangeHue(groundMat.GetFloat("_Hue"));
			groundMat.SetFloat("_Hue", value);
			objSpawn.ChangeObjects();
		}
		stringIndex++;
	}

	private void RandomChangeHue(Material mat)
	{
		textMaterial = mat;
		tColor = textMaterial.color;
		startHue = 0f;
		startSaturation = 0f;
		startValue = 0f;
		Color.RGBToHSV(tColor, out startHue, out startSaturation, out startValue);
		hue = Random.Range(0f, 1f);
		hue = startHue + hue;
		if (hue > 1f)
		{
			hue--;
		}
		textMaterial.color = Color.HSVToRGB(hue, startSaturation, startValue);
	}

	private void RandomChangeHue(Material mat, string colorProperty)
	{
		textMaterial = mat;
		tColor = textMaterial.GetColor(colorProperty);
		startHue = 0f;
		startSaturation = 0f;
		startValue = 0f;
		Color.RGBToHSV(tColor, out startHue, out startSaturation, out startValue);
		hue = Random.Range(0f, 1f);
		hue = startHue + hue;
		if (hue > 1f)
		{
			hue--;
		}
		textMaterial.SetColor(colorProperty, Color.HSVToRGB(hue, startSaturation, startValue));
	}

	private float RandomChangeHue(float _startHue)
	{
		hue = Random.Range(0f, 1f);
		hue = _startHue + hue;
		if (hue > 1f)
		{
			hue--;
		}
		return hue;
	}
}
