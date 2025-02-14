#region
using UnityEngine;
#endregion

public class Projectile : MonoBehaviour
{
    int damage;
    Player player;
    int manaCost;
    Vector3 startPos;
    
    [SerializeField] bool bullet;
    [SerializeField] bool destroyOnWallHit;
    [SerializeField] bool destroyOnEnemyHit;

    public void Initialize(int damage, Player player = null, int manaCost = 0, float timer = 3)
    {
        this.damage = damage;
        this.player = player;
        this.manaCost = manaCost;

        Destroy(gameObject, timer);
    }

    void Awake() => startPos = transform.position;

    void Update()
    {
        if (bullet) transform.forward = transform.position - startPos;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
            if(player != null && manaCost != 0)
            {
                player.Mana.Adjust(manaCost);
            }

            if (destroyOnEnemyHit)
            {
                Destroy(gameObject);
            }
        }

        if ((other.gameObject.name.ToLower().Contains("wall") || (other.TryGetComponent(out Door door) && !door.IsOpen)) && destroyOnWallHit) // Note: this was for a joke, we're not actually doing this lol
        {
            Destroy(gameObject);
        }
    }
}
