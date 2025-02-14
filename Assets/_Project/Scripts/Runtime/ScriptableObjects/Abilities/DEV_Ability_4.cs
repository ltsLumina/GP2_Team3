using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DEV_Ability_4", menuName = "Abilities/DEV_Ability_4", order = 103)]
public class DEV_Ability_4 : Ability
{
    [SerializeField] private int damage;
    [SerializeField] private float duration;
    [SerializeField] private Projectile damageWall;
    private float timer;
    private Material mat;

    private void Awake()
    {
        // TODO: cannot get renderer on prefab

    }

    public override void Use()
    {
        Debug.Log($"{Name} used!");
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000); // Fallback to a point far away if no hit
        GameObject wall = Instantiate( damageWall.gameObject, targetPoint, Quaternion.identity);
        mat = wall.GetComponent<Renderer>().material;
        Projectile wallAbility = wall.GetComponent<Projectile>();
        wall.transform.forward = new Vector3(Player.transform.position.x, 0, Player.transform.position.z) - new Vector3(wall.transform.position.x, 0, wall.transform.position.z);
        wall.transform.position = new Vector3(wall.transform.position.x, Player.transform.position.y, wall.transform.position.z);

        wallAbility.Initialize(damage);
        timer = duration;
        CoroutineHelper.StartCoroutine(SpawnRingVisual(mat, wallAbility));

    }


    private IEnumerator SpawnRingVisual(Material mat, Projectile damageWall)
    {
        float vfxAmount = 1f;

        while (vfxAmount > 0)
        {
            vfxAmount -= Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);
            yield return null;
        }

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        CoroutineHelper.StartCoroutine(DeSpawnRingVisual(mat, damageWall));

    }

    private IEnumerator DeSpawnRingVisual(Material mat, Projectile damageWall)
    {
        float vfxAmount = 0f;

        while (vfxAmount < 1)
        {
            vfxAmount += Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);
            yield return null;
        }

        Destroy(damageWall.gameObject);
    }


}