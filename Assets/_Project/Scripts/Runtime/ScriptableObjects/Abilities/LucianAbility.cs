using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Lucian", menuName = "Abilities/Lucian", order = 101)]
public class LucianAbility : Ability
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] int damage = 10;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] int numberOfShots = 20;
    [SerializeField] float projectileSpeed = 20f;
    
    Vector3 targetPoint;

    public override void Use()
    {
        Debug.Log($"{Name} used!");
        Player.Mana.Adjust(ManaCost);

        // Start the ultimate ability
        Action();
    }
    
    
    void Action()
    {
        float shootDuration = fireRate * numberOfShots;
        // increase move speed lerp over the duration of the ability
        float lerp = Mathf.Lerp(0, 15, shootDuration);
        Player.IncreaseStat(Player.Stats.MovementSpeed, Mathf.RoundToInt(lerp));

        // Start the coroutine to fire projectiles
        CoroutineHelper.StartCoroutine(FireProjectiles());
    }

    IEnumerator FireProjectiles()
    {
        for (int i = 0; i < numberOfShots; i++)
        {
            FireProjectile();
            yield return new WaitForSeconds(fireRate);
        }
        
        // Reset the player's movement speed
        Player.DecreaseStat(Player.Stats.MovementSpeed, 15);
    }

    void FireProjectile()
    {
        Player.Animator.SetTrigger("lucian");
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000); // Fallback to a point far away if no hit
        var newPoint = new Vector3(targetPoint.x, 0f, targetPoint.z);
        
        // Instantiate and fire the projectile
        var shootChild = GameObject.Find("ShootChild").transform;
        Projectile projectile = Instantiate(projectilePrefab, shootChild.position, Quaternion.identity);
        projectile.Initialize(damage);
        var rb = projectile.GetComponent<Rigidbody>();

        // Calculate the direction towards the mouse position
        Vector3 direction = (newPoint - Player.transform.position).normalized;
        rb.linearVelocity = direction * projectileSpeed;
    }
}
