using System.IO;
using System.Linq;
using UnityEngine;

public class SerializeMeshes
{
	public static void SerializeMesh(Mesh m, string name, bool uniquifyName = true)
	{
		string text = Application.dataPath + "/VVR_ASSETS/GeneratedMeshes/";
		string[] files = Directory.GetFiles(text);
		for (int i = 0; i < files.Length; i++)
		{
			string text2 = files[i].Substring(files[i].LastIndexOf('/') + 1);
			text2 = text2.Substring(0, text2.IndexOf('.'));
			files[i] = text2;
		}
		string text3 = name;
		if (uniquifyName && files.Contains(text3))
		{
			int num = 0;
			while (files.Contains(text3 + " (" + num + ")"))
			{
				num++;
			}
			string text4 = text3;
			text3 = text4 + " (" + num + ")";
		}
		string path = text + text3 + ".obj";
		using StreamWriter streamWriter = new StreamWriter(path);
		for (int j = 0; j < m.vertices.Length; j++)
		{
			Vector3 vector = m.vertices[j];
			streamWriter.WriteLine("v " + vector.x + " " + vector.y + " " + vector.z);
		}
		for (int k = 0; k < m.uv.Length; k++)
		{
			Vector2 vector2 = m.uv[k];
			streamWriter.WriteLine("vt " + vector2.x + " " + vector2.y);
		}
		for (int l = 0; l < m.triangles.Length; l += 3)
		{
			streamWriter.WriteLine("f " + m.triangles[l] + " " + m.triangles[l + 1] + " " + m.triangles[l + 2]);
		}
	}
}
