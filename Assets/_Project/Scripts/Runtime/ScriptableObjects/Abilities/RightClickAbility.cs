#region
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Right Click", menuName = "Abilities/Right Click Ability", order = 105)]
public class RightClickAbility : Ability
{
    [Header("Settings")]
    [SerializeField] int damage = 10;
    [SerializeField] GameObject effect;
    [Space(5)]
    [Header("Burst Settings")]
    [SerializeField] float burstDelay = 0.25f;
    [SerializeField] int burstDamage = 50;
    [Space(5)]
    [Header("Cone Settings")]
    [SerializeField] float range = 5f;
    [SerializeField] float angle = 90f;

    [Header("Audio")]
    [SerializeField] EventReference slashSFX;
    [SerializeField] EventReference bombSFX;

    [SerializeField] GameObject pulseWave;
    private Material pulseMaterial;

    public override void Use()
    {
        Debug.Log($"{Name} used!");
        Player.Mana.Adjust(ManaCost);

        Vector3 transformPosition = Player.transform.position + new Vector3(0, 0.25f, 0);
        GameObject obj = Instantiate(pulseWave, transformPosition, Player.transform.rotation);
        Vector3 rot = obj.transform.rotation.eulerAngles;
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + 0.75f, obj.transform.position.z);
        obj.transform.rotation = Quaternion.Euler(90, rot.y + 180, rot.z);
        pulseMaterial = obj.GetComponent<MeshRenderer>().material;

        CoroutineHelper.StartCoroutine(SpawnVisual(pulseMaterial, obj));

        Player.Animator.SetTrigger("slash");
        Action();
    }

    void Action()
    {
        // TODO: Play animation
        
        // strike any enemy within range, and within a 180 degree cone in front of the player
        var cone = Physics.OverlapSphere(Player.transform.position, range, LayerMask.GetMask("Enemy"));

        List<Enemy> enemiesInRange = new ();

        FMODUnity.RuntimeManager.PlayOneShotAttached(slashSFX, Player.gameObject);

        foreach (var enemy in cone)
        {
            // check if the enemy is in the cone
            if (Vector3.Angle(Player.transform.forward, enemy.transform.position - Player.transform.position) > angle / 2f) continue;
            
            if (enemy.TryGetComponent(out Enemy e))
            {
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(Player.transform.position + new Vector3(0, 0.75f, 0), enemy.transform.position - Player.transform.position + new Vector3(0, 0.75f, 0), out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.ToLower().Contains("wall") || (hit.transform.TryGetComponent(out Door door) && !door.IsOpen))
                    {
                        Debug.DrawRay(Player.transform.position, (enemy.transform.position - Player.transform.position) + new Vector3(0, 0.75f, 0), Color.red, 2);
                        continue;
                    }
                    else
                    {
                        Debug.DrawRay(Player.transform.position, (enemy.transform.position - Player.transform.position) + new Vector3(0, 0.75f, 0), Color.green, 2);
                    }

                }

                e.TakeDamage(damage);
                enemiesInRange.Add(e);
            }



            if (enemiesInRange.Count > 0)
            {
                Debug.Assert(enemiesInRange.Count < 10, "Bursting more than 10 enemies is not recommended, and will cause performance issues. " +
                                                        $"\nThere were {enemiesInRange.Count} enemies in the burst.");
                CoroutineHelper.StartCoroutine(Burst(enemiesInRange));
            }
        }

#if UNITY_EDITOR
        DrawCone(Player.transform.position, Player.transform.forward, Player.transform.up, range, angle, 16, enemiesInRange.Count > 0 ? Color.green : Color.red);
#endif
    }

    IEnumerator Burst(List<Enemy> enemies)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(bombSFX, enemies[0].gameObject);
        
        var effectInstance = Instantiate(effect, enemies[0].transform.position, Quaternion.identity);
        effectInstance.transform.localScale = Vector3.one * 2f;
        Destroy(effectInstance, 3f);

        yield return new WaitForSeconds(burstDelay);

        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(burstDamage);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator SpawnVisual(Material mat, GameObject pulse)
    {
        float vfxAmount = 0f;

        while (vfxAmount < 1)
        {
            vfxAmount += Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);

            yield return null;
        }

        Destroy(pulse);
    }

#if UNITY_EDITOR
    void DrawCone // praise ChatGPT
    (
        Vector3 position, Vector3 forward, Vector3 up, float length, float angle, int segmentCount,
        Color color
    )
    {
        float halfAngle = angle     * 0.5f;
        float step = halfAngle * 2f / segmentCount;

        // Draw main forward line
        Debug.DrawLine(position, position + forward * length, color, 2.5f);

        // Draw cone boundary lines
        Vector3 leftBoundary = Quaternion.AngleAxis(-halfAngle, up) * forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(halfAngle, up) * forward;
        Debug.DrawLine(position, position + leftBoundary  * length, color, 2.5f);
        Debug.DrawLine(position, position + rightBoundary * length, color, 2.5f);

        // Draw arc using multiple line segments
        for (int i = 0; i < segmentCount; i++)
        {
            float angle1 = -halfAngle + step * i;
            float angle2 = -halfAngle + step * (i + 1);

            Vector3 dir1 = Quaternion.AngleAxis(angle1, up) * forward;
            Vector3 dir2 = Quaternion.AngleAxis(angle2, up) * forward;

            Debug.DrawLine(position + dir1 * length, position + dir2 * length, color, 2.5f);
        }
    }
#endif
}