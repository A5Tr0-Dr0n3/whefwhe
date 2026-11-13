using UnityEngine;

namespace OVRTouchSample;

public class DistanceGrabDemo : MonoBehaviour
{
	private bool m_useSpherecast;

	[SerializeField]
	private GameObject m_GrabThroughWalls;

	private bool m_allowGrabThroughWalls;

	[SerializeField]
	private DistanceGrabber[] m_grabbers;

	public bool UseSpherecast
	{
		get
		{
			return m_useSpherecast;
		}
		set
		{
			m_useSpherecast = value;
			for (int i = 0; i < m_grabbers.Length; i++)
			{
				m_grabbers[i].UseSpherecast = m_useSpherecast;
			}
		}
	}

	public bool AllowGrabThroughWalls
	{
		get
		{
			return m_allowGrabThroughWalls;
		}
		set
		{
			m_allowGrabThroughWalls = value;
			for (int i = 0; i < m_grabbers.Length; i++)
			{
				m_grabbers[i].m_preventGrabThroughWalls = !m_allowGrabThroughWalls;
			}
		}
	}
}
