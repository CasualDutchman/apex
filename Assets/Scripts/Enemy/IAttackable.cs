
public interface IAttackable {

    void Damage(float f, bool isWolf);
    void AddHealth(float f, float removeFood);
    bool IsAlive();
}
