public class DeadState : IState
{
	public bool CanExit { get; private set; } = false;

	private Character character;

	public DeadState(Character character)
	{
		this.character = character;
	}

	public void Enter()
	{
		character.animator.SetTrigger("Dead");
		character.Die();
	}

	public void Exit()
	{
	}

	public void Update()
	{
	}
}