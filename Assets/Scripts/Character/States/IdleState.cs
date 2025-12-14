public class IdleState : IState
{
	public bool CanExit { get; private set; } = true;

	private Character character;

	public IdleState(Character character)
	{
		this.character = character;
	}

	public void Enter()
	{
		character.animator.SetFloat("Index", character.animationIndex);
		character.animator.SetTrigger("Idle");
	}

	public void Exit()
	{
	}

	public void Update()
	{
	}
}