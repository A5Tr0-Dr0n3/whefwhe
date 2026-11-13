using System;
using UnityEngine;
using UnityEngine.Events;

namespace GoogleVR.VideoDemo;

[Serializable]
public class GameObjectEvent : UnityEvent<GameObject>
{
}
