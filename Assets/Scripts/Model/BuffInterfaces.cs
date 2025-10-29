public interface IOnHit
{
	void OnHit(Character owner, Character target, float damage);
}

public interface IOnTakeHit
{
	void OnTakeHit(Character owner, Character attacker, float damage);
}

public interface IOnKill
{
	void OnKill(Character owner, Character killer);
}

public interface IOnDeath
{
	void OnDeath(Character owner, Character killer);
}

public interface IOnTick
{
	void Tick(Character owner, float deltaTime);
}
