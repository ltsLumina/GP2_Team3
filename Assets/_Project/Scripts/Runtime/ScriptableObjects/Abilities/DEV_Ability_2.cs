using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DEV_Ability_2", menuName = "Abilities/DEV_Ability_2", order = 101)]
public class DEV_Ability_2 : Ability
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] int damage = 10;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] int numberOfShots = 20;
    [SerializeField] float projectileSpeed = 20f;

    Player player;
    Vector3 targetPoint;

    public override void Use()
    {
        Debug.Log($"{Name} used!");

        // Capture the mouse position in world space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000); // Fallback to a point far away if no hit

        // Start the ultimate ability
        Action();
    }
    
    void Action()
    {
        player = FindAnyObjectByType<Player>();

        float shootDuration = fireRate * numberOfShots;
        // increase move speed lerp over the duration of the ability
        float lerp = Mathf.Lerp(0, 15, shootDuration);
        player.IncreaseStat(Player.Stats.MovementSpeed, Mathf.RoundToInt(lerp));

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
        player.DecreaseStat(Player.Stats.MovementSpeed, 15);
    }

    void FireProjectile()
    {
        // Instantiate and fire the projectile
        Projectile projectile = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
        projectile.Initialize(damage);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Calculate the direction to the target point
        Vector3 direction = (targetPoint - player.transform.position).normalized;
        rb.linearVelocity = direction * projectileSpeed;
        
        Destroy(projectile, 1f);
    }
}
