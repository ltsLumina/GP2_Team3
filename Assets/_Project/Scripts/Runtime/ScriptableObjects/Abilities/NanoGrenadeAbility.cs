using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Nano Grenade", menuName = "Abilities/Nano Grenade", order = 102)]
public class NanoGrenadeAbility : Ability
{
    [SerializeField] Grenade grenadePrefab;
    [SerializeField] Projectile nanoBomb;
    [Tooltip("The maximum distance the projectile will be thrown.")] 
    [SerializeField] float explosionTimer = 2f;
    [SerializeField] float distance = 10;
    [SerializeField] int damage = 20;
    [SerializeField] int dotDamage = 2;
    [SerializeField] float explosionScale = 1;
    
    Vector3 targetPoint;

    public override void Use()
    {
        Debug.Log($"{Name} used!");
        Player.Mana.Adjust(ManaCost);

        // Capture the mouse position in world space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000); // Fallback to a point far away if no hit
        targetPoint = new Vector3(targetPoint.x, 0f, targetPoint.z);

        Action();
    }

    void Action()
    {
        Player player = Player;
        //Vector3 direction = (targetPoint - player.transform.position).normalized;

        Grenade grenade = Instantiate(grenadePrefab, player.transform.position, Quaternion.identity);
        grenade.Initialize(damage, dotDamage, nanoBomb, explosionScale, explosionTimer);
        
        // dotween bounce
        grenade.transform.forward = targetPoint - grenade.transform.position;
        grenade.transform.DOLocalJump(player.transform.position + ((targetPoint - player.transform.position).normalized * distance), 1, 2, explosionTimer).SetEase(Ease.Linear);
        
        // grenade.transform.forward = direction;
        // var rb = grenade.GetComponent<Rigidbody>();
        // rb.linearVelocity = direction * projectileSpeed;

    }
}