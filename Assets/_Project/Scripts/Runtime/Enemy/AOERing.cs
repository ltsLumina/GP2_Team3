using UnityEngine;

public class AOERing : MonoBehaviour
{
    public float spawnTime { get; private set; }
    
    public float timeToDamage { get; set; }
    
    public bool isDealingDamage { get; set; }

    public BossAOE.RingType ringType { get; set; }
    private void Awake()
    {
        spawnTime = Time.time;
    }
}
