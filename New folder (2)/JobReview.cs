using System;
using UnityEngine;

[Serializable]
public class JobReview
{
	[Range(1f, 5f)]
	public float reviewScore = 1f;

	[TextArea(3, 10)]
	public string reviewText = "Job review text";

	public string clientName = "Name of Client";
}
