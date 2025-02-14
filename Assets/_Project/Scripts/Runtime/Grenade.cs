using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionParticles;
    
    int damage;
    int dotDamage;
    private Projectile nanoBomb;
    private float bombScale;
    private float explosionTimer;

    public void Initialize(int damage, int dotDamage, Projectile nanoBomb, float bombScale, float explosionTimer)
    {
        this.damage = damage;
        this.dotDamage = dotDamage;
        this.nanoBomb = nanoBomb;
        this.bombScale = bombScale;
        this.explosionTimer = explosionTimer;
        Destroy(gameObject, explosionTimer);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            OnHit();
        }

        if (other.gameObject.name.ToLower().Contains("wall") || (other.TryGetComponent(out Door door) && !door.IsOpen)) // Note: this was not for a joke, we're ARE actually doing this lol
        {
            OnHit();
        }
    }

    private void OnDestroy()
    {
        OnHit();
    }

    void OnHit()
    {
        Destroy(gameObject);
        Projectile spawnedBomb = Instantiate(nanoBomb, transform.position, Quaternion.identity);
        spawnedBomb.transform.localScale = new Vector3(bombScale, bombScale, bombScale);
        spawnedBomb.Initialize(damage);
        spawnedBomb.GetComponent<DamageOverTime>().Initialize(dotDamage);
        var effect = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        effect.Play();
    }

}
