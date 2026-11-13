using System;

[Serializable]
public class BoidSoundObject
{
	public ObjectBoid boid;

	public float distance;

	public bool playingMusic;

	public bool playingFlying;

	public BoidSoundEmitter attachedEmitter;

	public BoidSoundObject(float d, ObjectBoid b)
	{
		boid = b;
		distance = d;
		playingMusic = false;
		playingFlying = false;
		attachedEmitter = null;
	}
}
