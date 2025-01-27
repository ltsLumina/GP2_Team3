using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "DEV_Ability_3", menuName = "Abilities/DEV_Ability_3", order = 102)]
public class DEV_Ability_3 : Ability
{
    [SerializeField] Projectile projectilePrefab;
    [Tooltip("The maximum distance the projectile will be thrown.")] 
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float projectileSpeed = 10f;
    [Tooltip("The delay before the projectile returns to the player.")]
    [SerializeField] float returnDelay = 1f;
    [SerializeField] float returnSpeed = 15f;
    [SerializeField] int damage = 20;
    
    Vector3 targetPoint;

    public override void Use()
    {
        Debug.Log($"{Name} used!");

        // Capture the mouse position in world space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000); // Fallback to a point far away if no hit

        Action();
    }

    void Action()
    {
        Player player = Player;
        Vector3 startPosition = player.transform.position;

        // Calculate the direction to the target point
        Vector3 direction = (targetPoint - startPosition).normalized;
        Vector3 forwardPosition = startPosition + direction * maxDistance;

        Projectile projectile = Instantiate(projectilePrefab, startPosition, Quaternion.identity);
        var rb = projectile.GetComponent<Rigidbody>();

        // Move forward
        rb.DOMove(forwardPosition, maxDistance / projectileSpeed).SetEase(Ease.OutSine).OnComplete
        (() =>
        {
            // Start coroutine to move back to player position.
            CoroutineHelper.StartCoroutine(ReturnToPlayer(rb, projectile));
        });

        projectile.Initialize(damage);
    }

    IEnumerator ReturnToPlayer(Rigidbody rb, Projectile projectile)
    {
        yield return new WaitForSeconds(returnDelay);
        
        while (Vector3.Distance(rb.position, Player.transform.position) > 0.1f)
        {
            Vector3 playerPosition = Player.transform.position;
            rb.position = Vector3.MoveTowards(rb.position, playerPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(projectile.gameObject);
    }
}