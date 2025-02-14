#region
using System.Collections;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "DEV_Ability_1", menuName = "Abilities/DEV_Ability_1", order = 100)]
public class DEV_Ability_1 : Ability
{
    [SerializeField] int damage;
    [SerializeField] Projectile pulseWave;

    BoxCollider boxCollider;
    Material mat;
    Player player;

    public override void Use()
    {
        Debug.Log($"{Name} used!");
        Player.Mana.Adjust(ManaCost);

        player = GameManager.Instance.Player;
        Vector3 transformPosition = player.transform.position + new Vector3(0, 0.25f, 0);
        Projectile obj = Instantiate(pulseWave, transformPosition, player.transform.rotation);
        Vector3 rot = obj.transform.rotation.eulerAngles;
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + 0.75f, obj.transform.position.z);
        obj.transform.rotation = Quaternion.Euler(90, rot.y + 180, rot.z);
        mat = obj.GetComponent<MeshRenderer>().material;
        boxCollider = obj.GetComponent<BoxCollider>();

        obj.Initialize(damage);
        CoroutineHelper.StartCoroutine(SpawnVisual(mat, obj));
    }

    IEnumerator SpawnVisual(Material mat, Projectile pulse)
    {
        float vfxAmount = 0f;

        while (vfxAmount < 1)
        {
            vfxAmount += Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);

            if (vfxAmount >= 0.1f) boxCollider.enabled = true;

            yield return null;
        }

        Destroy(pulse);
    }
}
