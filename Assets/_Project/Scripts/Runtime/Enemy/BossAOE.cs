using System.Collections.Generic;
using UnityEngine;

public class BossAOE : MonoBehaviour
{
    [SerializeField]
    private AOERing aoeRingPrefab;
    private List<AOERing> rings = new();

    public enum RingType
    {
        Dot = 0,
        Instant
    }
    
    public void SpawnDOTRing()
    {
        AOERing aoeRing = (AOERing)Instantiate(aoeRingPrefab, transform);
        
        // 0 - 49 Ring Instant dmg  : 50 - 99 Ring Dot dmg
        aoeRing.ringType = Random.Range(0, 100) > 49 ? RingType.Instant : RingType.Dot;
        aoeRing.timeToDamage = aoeRing.ringType == RingType.Instant ? 3 : 1;
        
        rings.Add(aoeRing);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SpawnDOTRing();
        
        for (int i = rings.Count - 1; i >= 0; i--)
        {
            if (Time.time - rings[i].spawnTime > rings[i].timeToDamage && !rings[i].isDealingDamage)
            {
                if (rings[i].ringType == RingType.Instant)
                {
                    Debug.Log("Instant damage from ring, POOOOOOF");
                    Destroy(rings[i].gameObject);
                    rings.RemoveAt(i);
                    continue;
                }
                Debug.Log("Start doing damage");
                rings[i].isDealingDamage = true;
            }

            if (Time.time - rings[i].spawnTime > 5f && rings[i].ringType == RingType.Dot)
            {
                Debug.Log("Removing DOT ring");
                Destroy(rings[i].gameObject);
                rings.RemoveAt(i);
            }
        }
    }
}
