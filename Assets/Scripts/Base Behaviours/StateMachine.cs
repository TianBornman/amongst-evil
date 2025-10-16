using System;
using UnityEngine;

public class StateMachine<T> : MonoBehaviour where T : Enum
{
	public T State { get; private set; }

	protected virtual void SetState(T state)
	{
		State = state;
	}
}