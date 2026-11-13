using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WindowShaderTester : MonoBehaviour
{
	public List<Transform> tList = new List<Transform>();

	public float sqrFalloff = 15f;

	private Vector4[] testArray = new Vector4[6];

	private void Start()
	{
		testArray = new Vector4[6];
	}

	private void Update()
	{
		for (int i = 0; i < testArray.Length; i++)
		{
			ref Vector4 reference = ref testArray[i];
			reference = new Vector4(tList[i].position.x, tList[i].position.y, tList[i].position.z, sqrFalloff);
		}
		Shader.SetGlobalVectorArray("_SourcePos", testArray);
		Shader.SetGlobalFloat("_GlobalASL", 1f);
	}
}
