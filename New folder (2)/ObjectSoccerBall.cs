using UnityEngine;

public class ObjectSoccerBall : ObjectBase
{
	public AudioClip touchdown;

	private void OnTriggerEnter(Collider other)
	{
		if (!isSatisfied && other.name == "SoccerGoal")
		{
			AudioSource.PlayClipAtPoint(touchdown, transform.position);
			Satisfaction();
		}
	}
}
