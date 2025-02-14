using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AOERing : MonoBehaviour
{
    public float spawnTime { get; private set; }
    public bool isDealingDamage { get; set; }

    public GameObject ringIndicator;

    public int ringDamage;

    new public bool didStart = false;
    
    private float dotTimer;

    private Transform player;
    private Vector3 ringSize;
    public Material aoeRingMat { get; private set; }
    public MeshRenderer meshRenderer;

    public BossAOE.AttackType ringType { get; set; }

    public float bloodRingScale;

    //Changed to start because of a race condition in Awake
    private void Awake()
    {
        spawnTime = Time.time;

        // save a reference to the player transform for slight performance uplift since calling .transform is making an extern call
        player = GameManager.Instance.Player.transform;
        
        // cache the meshrenderer
        meshRenderer = GetComponent<MeshRenderer>();

        aoeRingMat = meshRenderer.material;
        ringSize = meshRenderer.bounds.size / 2; // we half it since we only need value from middle point to end point and not end to end
    }

    private void OnDestroy() => EnemyManager.Instance.GetBoss()?.ResetSpikeCooldown();

    // this function is only for setting the spike. The distance to any side is always the same so we only have to check x
    public void SetRingSize(Vector3 s, float offset)
    {
        ringSize.x = s.x + offset;
        ringSize.y = s.x + offset;
        ringSize.z = s.x + offset;
    }

    private bool CheckDistanceAgainstPlayer()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        return (distance <= ringSize.x && distance <= ringSize.z);
    }

    public void DoInstantDamage()
    {
        if (CheckDistanceAgainstPlayer())
        {
            player.TryGetComponent(out IDamageable comp);
            comp.TakeDamage(ringDamage);
        }
    }

    public void UpdateRing()
    {

        if (ringType == BossAOE.AttackType.BloodRing)
        {
            ringSize = meshRenderer.bounds.size / 2;
            transform.position = transform.parent.position;
            Vector3 scaleDummy = transform.localScale;
            if (scaleDummy.x < bloodRingScale)
            {
                spawnTime = Time.time;
                scaleDummy += new Vector3(4f, 4f, 4f) * Time.deltaTime;
                transform.localScale = scaleDummy;
            }
            else
                EnemyManager.Instance.GetBoss()?.SliceSpikeCooldown();

            if (CheckDistanceAgainstPlayer() && isDealingDamage)
            {
                dotTimer += Time.deltaTime;

                if (dotTimer >= 1f) // contiuous damage would be better than once a second damage but that would require some weird rounding rules unless we make health a float
                {
                    player.TryGetComponent(out IDamageable comp);
                    comp.TakeDamage(ringDamage);
                    dotTimer = 0f;
                }
            }
        }
    }
}
