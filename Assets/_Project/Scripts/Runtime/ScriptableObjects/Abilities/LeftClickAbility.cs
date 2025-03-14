#region
using DG.Tweening;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Left Click", menuName = "Abilities/Left Click Ability", order = 104)]
public class LeftClickAbility : Ability
{
    [SerializeField] int damage = 10;
    [SerializeField] float timeToDestroy = 2;
    [SerializeField] Projectile bullet;
    [SerializeField] float speed;

    /// <summary>
    /// Left and Right Click abilities need additional logic to work properly.
    /// <para>A check must be performed to determine if the cursor is over a UI element.</para>
    /// <para>If the cursor is over a UI element, the ability should not be used.</para>
    /// </summary>
    public override void Use()
    {
        Debug.Log($"{Name} used!");
        
        Player.Animator.SetTrigger("shoot");
        Action();
    }

    Ray ray;
    RaycastHit hit;
    
    void Action()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            hit.collider.TryGetComponent(out Enemy _);
            
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2.5f);
#endif
        }

        var shootChild = GameObject.Find("ShootChild").transform;
        Projectile bulletObj = Instantiate(bullet, shootChild.position, Quaternion.identity);

        bulletObj.Initialize(damage, Player, ManaCost, timeToDestroy);

        Vector3 shootDirection = (new Vector3(hit.point.x, 0, hit.point.z) - Player.transform.position).normalized;

        var rb = bulletObj.GetComponent<Rigidbody>();

        // Calculate the direction towards the mouse position
        rb.linearVelocity = shootDirection * speed;

        //Camera.main.DOShakePosition(duration, strength, vibrato, randomness, false, randomnessMode: ShakeRandomnessMode.Harmonic);
    }

}