using System.Drawing;
using UnityEngine;

public class AOERing : MonoBehaviour
{
    public float spawnTime { get; private set; }
    public bool isDealingDamage { get; set; }

    public int dotDamage;
    private float dotTimer;

    private Player player;
    private MeshRenderer meshRenderer;
    private Vector3 ringSize;
    public Material aoeRingMat { get; private set; }

    private void Awake()
    {
        spawnTime = Time.time;
        // save a reference to the player for code readability
        player = GameManager.Instance.Player;
        // cache the meshrenderer
        meshRenderer = GetComponent<MeshRenderer>();
        
        aoeRingMat = meshRenderer.material;
        ringSize = meshRenderer.bounds.size / 2; // we half it since we only need value from middle point to end point and not end to end
    }

    private bool CheckDistanceAgainstPlayer()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        return (distance <= ringSize.x && distance <= ringSize.z);
    }

    public void UpdateRing()
    {
        ringSize = meshRenderer.bounds.size / 2;
        
        if (CheckDistanceAgainstPlayer() && isDealingDamage)
        {
            dotTimer += Time.deltaTime;
            if (dotTimer >= 1f) // contiuous damage would be better than once a second damage but that would require some weird rounding rules unless we make health a float
            {
                player.TryGetComponent(out IDamageable comp);
                comp.TakeDamage(Mathf.CeilToInt(dotDamage));
                dotTimer = 0f;
            }
        }
    }
}
