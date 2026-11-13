using System.Collections.Generic;
using System.Text;
using proto;
using UnityEngine;

namespace Gvr.Internal;

internal struct EmulatorTouchEvent
{
	public enum Action
	{
		kActionDown = 0,
		kActionUp = 1,
		kActionMove = 2,
		kActionCancel = 3,
		kActionPointerDown = 5,
		kActionPointerUp = 6,
		kActionHoverMove = 7,
		kActionHoverEnter = 9,
		kActionHoverExit = 10
	}

	public struct Pointer(int fingerId, float normalizedX, float normalizedY)
	{
		public readonly int fingerId = fingerId;

		public readonly float normalizedX = normalizedX;

		public readonly float normalizedY = normalizedY;

		public override string ToString()
		{
			return $"({fingerId}, {normalizedX}, {normalizedY})";
		}
	}

	private readonly int action;

	public readonly int relativeTimestamp;

	public readonly List<Pointer> pointers;

	private static readonly int ACTION_POINTER_INDEX_SHIFT = 8;

	private static readonly int ACTION_POINTER_INDEX_MASK = 65280;

	private static readonly int ACTION_MASK = 255;

	public EmulatorTouchEvent(PhoneEvent.Types.MotionEvent proto, long lastDownTimeMs)
	{
		action = proto.Action;
		relativeTimestamp = (int)(((proto.Action & ACTION_MASK) != 0) ? (proto.Timestamp - lastDownTimeMs) : 0);
		pointers = new List<Pointer>();
		foreach (PhoneEvent.Types.MotionEvent.Types.Pointer pointers in proto.PointersList)
		{
			this.pointers.Add(new Pointer(pointers.Id, pointers.NormalizedX, pointers.NormalizedY));
		}
	}

	public EmulatorTouchEvent(Action action, int pointerId, int relativeTimestamp, List<Pointer> pointers)
	{
		int num = 0;
		if (action == Action.kActionPointerDown || action == Action.kActionPointerUp)
		{
			num = findPointerIndex(pointerId, pointers);
			if (num == -1)
			{
				Debug.LogWarning("Could not find specific fingerId " + pointerId + " in the supplied list of pointers.");
				num = 0;
			}
		}
		this.action = getActionUnmasked(action, num);
		this.relativeTimestamp = relativeTimestamp;
		this.pointers = pointers;
	}

	public Action getActionMasked()
	{
		return (Action)(action & ACTION_MASK);
	}

	public Pointer getActionPointer()
	{
		int index = (action & ACTION_POINTER_INDEX_MASK) >> ACTION_POINTER_INDEX_SHIFT;
		return pointers[index];
	}

	private static int getActionUnmasked(Action action, int fingerIndex)
	{
		return (int)action | (fingerIndex << ACTION_POINTER_INDEX_SHIFT);
	}

	private static int findPointerIndex(int fingerId, List<Pointer> pointers)
	{
		int result = -1;
		for (int i = 0; i < pointers.Count; i++)
		{
			if (fingerId == pointers[i].fingerId)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("t = {0}; A = {1}; P = {2}; N = {3}; [", relativeTimestamp, getActionMasked(), getActionPointer().fingerId, pointers.Count);
		for (int i = 0; i < pointers.Count; i++)
		{
			stringBuilder.Append(pointers[i]).Append(", ");
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
