public interface IState
{
	bool CanExit { get; }

	void Enter();
	void Exit();
	void Update();
}