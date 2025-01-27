using UnityEngine;

public class Projectile : MonoBehaviour
{
    int damage;

    public void Initialize(int damage) => this.damage = damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy)) enemy.TakeDamage(damage);
    }
}
