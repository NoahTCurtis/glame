using System.Collections;
using System.Collections.Generic;

public abstract class Manager : UnityEngine.MonoBehaviour
{
	[UnityEngine.SerializeField] protected bool doNotDestroy = false;

	protected virtual void Awake()
	{
		Game.Register(this, doNotDestroy);
	}

	protected virtual void OnDestroy()
	{
		Game.Unregister(this);
	}
}
