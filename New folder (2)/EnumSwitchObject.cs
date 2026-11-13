using System;
using System.Collections.Generic;
using TButt;
using UnityEngine;

public class EnumSwitchObject : ObjectBase
{
	public delegate void OnSelectionChangedHandler();

	[SerializeField]
	private string[] options = new string[3] { "Option 1", "Option 2", "Option 3" };

	[SerializeField]
	private float startAngle;

	[SerializeField]
	private float endAngle;

	[SerializeField]
	private TextMesh labelPrototype;

	[SerializeField]
	private float labelRadius = 0.5f;

	private Dictionary<float, TextMesh> anglesToOptions;

	[SerializeField]
	private Color unlitColor = Color.white;

	[SerializeField]
	private Color litColor = Color.red;

	[SerializeField]
	private HingeJoint switchJoint;

	public int currentIndex = -1;

	private Quaternion controllerTwistComparision;

	public event OnSelectionChangedHandler onSelectionChanged;

	private void OnDrawGizmos()
	{
		Vector3 vector = Quaternion.AngleAxis(startAngle, transform.forward) * transform.right;
		Vector3 vector2 = Quaternion.AngleAxis(endAngle, transform.forward) * transform.right;
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, transform.position + vector * labelRadius);
		Gizmos.DrawSphere(transform.position + vector * labelRadius, 0.05f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, transform.position + vector2 * labelRadius);
		Gizmos.DrawSphere(transform.position + vector2 * labelRadius, 0.05f);
	}

	protected override void Awake()
	{
		base.Awake();
		setUpOptions();
		if (currentIndex == -1)
		{
			setSelectedOption(0, ToolManager.ActiveToolGroups[0].Manipulator);
		}
	}

	private void setUpOptions()
	{
		anglesToOptions = new Dictionary<float, TextMesh>();
		float num = clockwiseDeltaAngle((float)Math.PI / 180f * startAngle, (float)Math.PI / 180f * endAngle);
		for (int i = 0; i < options.Length; i++)
		{
			TextMesh component = UnityEngine.Object.Instantiate(labelPrototype.gameObject).GetComponent<TextMesh>();
			component.transform.parent = transform.parent;
			component.text = options[i];
			component.name = "Option " + i + ": " + component.text;
			float num2 = ((options.Length <= 1) ? ((float)Math.PI / 180f * startAngle) : ((float)Math.PI / 180f * startAngle + num * ((float)i / (float)(options.Length - 1))));
			Vector3 vector = labelRadius * (transform.right * Mathf.Cos(num2) + transform.up * Mathf.Sin(num2)).normalized;
			component.transform.position = transform.position + vector;
			component.transform.rotation = transform.rotation;
			component.anchor = getTextAnchorByAngle(num2);
			anglesToOptions.Add(num2, component);
		}
		labelPrototype.gameObject.SetActive(value: false);
	}

	private TextAnchor getTextAnchorByAngle(float ang)
	{
		float num = Mathf.Sin(ang);
		float num2 = Mathf.Cos(ang);
		if ((double)num >= 0.33)
		{
			return (!(num2 >= 0f)) ? TextAnchor.LowerRight : TextAnchor.LowerLeft;
		}
		if ((double)num < 0.33 && (double)num > -0.33)
		{
			return (!(num2 >= 0f)) ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
		}
		return (!(num2 >= 0f)) ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		controllerTwistComparision = TBInput.GetRotation(grabber.GrabbingToolGroup.Controller);
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (!(grabber != grabbedBy[0]))
		{
			base.GrabUpdate(grabber);
			float num = Mathf.Abs(clockwiseDeltaAngle(startAngle * ((float)Math.PI / 180f), endAngle * ((float)Math.PI / 180f)) / (float)(options.Length - 1));
			Vector3 lhs = controllerTwistComparision * Vector3.right;
			Vector3 rhs = TBInput.GetRotation(grabber.GrabbingToolGroup.Controller) * Vector3.right;
			Vector3 lhs2 = TBInput.GetRotation(grabber.GrabbingToolGroup.Controller) * Vector3.forward;
			float num2 = (0f - Mathf.Sign(Vector3.Dot(lhs2, Vector3.Cross(lhs, rhs)))) * Mathf.Acos(Vector3.Dot(lhs, rhs));
			if (num2 > num)
			{
				setSelectedOption(currentIndex + 1, grabber);
			}
			else if (num2 < 0f - num)
			{
				setSelectedOption(currentIndex - 1, grabber);
			}
		}
	}

	private void setSelectedOption(int index, Manipulator grabber)
	{
		if (index >= 0 && index < options.Length)
		{
			int num = currentIndex;
			currentIndex = index;
			controllerTwistComparision = TBInput.GetRotation(grabber.GrabbingToolGroup.Controller);
			float[] array = new float[options.Length];
			anglesToOptions.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				anglesToOptions[array[i]].color = unlitColor;
			}
			anglesToOptions[array[index]].color = litColor;
			JointSpring spring = switchJoint.spring;
			float num2;
			for (num2 = array[index] * 57.29578f; num2 > 180f; num2 -= 360f)
			{
			}
			spring.targetPosition = num2;
			switchJoint.spring = spring;
			if (num != index && onSelectionChanged != null)
			{
				onSelectionChanged();
			}
		}
	}

	private float clockwiseDeltaAngle(float a, float b)
	{
		if (a < b)
		{
			return (0f - a - b) % ((float)Math.PI * 2f);
		}
		return -1f * ((a - b) % ((float)Math.PI * 2f));
	}

	public void OverwriteIndex(int newIndex)
	{
		setSelectedOption(Mathf.Clamp(newIndex, 0, options.Length), ToolManager.ActiveToolGroups[0].Manipulator);
	}
}
