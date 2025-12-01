using UnityEngine;

public class StateMachine : MonoBehaviour
{
	// Private Variables
	private IState state;

	// Protected Methods
	protected virtual void Update()
	{
		state.Update();
	}

	// Public Methods
	public void SetState(IState state)
	{
		this.state?.Exit();
		this.state = state;
		state.Enter();
	}
}