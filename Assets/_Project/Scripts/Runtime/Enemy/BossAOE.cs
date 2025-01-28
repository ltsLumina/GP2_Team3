using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossAOE : MonoBehaviour
{
    [SerializeField]
    private AOERing aoeRingPrefab;
    private List<AOERing> rings = new();

    private float lastAttackTime = 0;
    [SerializeField]
    private float attackCooldown = 5f;

    public enum AttackType
    {
        None = 0,
        BloodRing,
        SpikeRing
    }
    
    private void SpawnDOTRing()
    {
        Vector3 position = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z);
        AOERing aoeRing = Instantiate(aoeRingPrefab, position, aoeRingPrefab.transform.rotation, transform);
        aoeRing.ringType = AttackType.BloodRing;
        aoeRing.aoeRingMat.SetFloat("_Amount", 0.4f);
        rings.Add(aoeRing);
        
        StartCoroutine(SpawnRingVisual(aoeRing.aoeRingMat, aoeRing));
    }

    private void SpawnSpikeRing()
    {
        Vector3 position = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z);
        AOERing aoeRing = Instantiate(aoeRingPrefab, position, aoeRingPrefab.transform.rotation, transform);
        aoeRing.ringType = AttackType.SpikeRing;
        aoeRing.aoeRingMat.SetFloat("_Amount", 1f);
        aoeRing.ringDamage = 250;
        rings.Add(aoeRing);
    }

    private AttackType RollNextAttack()
    {
        return (AttackType)Random.Range(1, Enum.GetNames(typeof(AttackType)).Length); // include last item with +1 since Random.Range on int is not max inclusive
    }

    private void Update()
    {
        //if (Time.time - lastAttackTime >= attackCooldown)
        //{
            AttackType nextAttack = RollNextAttack();
            Debug.Log(nextAttack);
            switch (nextAttack)
            {
                case AttackType.BloodRing:
                {
                    if (Input.GetKeyDown(KeyCode.R))
                        SpawnDOTRing();
                    break;
                }
                case AttackType.SpikeRing:
                    if (Input.GetKeyDown(KeyCode.F))
                        SpawnSpikeRing();
                    break;
            }

       //    lastAttackTime = Time.time;
        //}

        if (rings.Count > 0)
        {
            for (int i = rings.Count - 1; i >= 0; i--)
            {
                // we have to call this before we potentially remove the ring at the end of this scope
                rings[i].UpdateRing();

                if (rings[i].ringType == AttackType.BloodRing)
                {
                    if (Time.time - rings[i].spawnTime > 5f && rings[i].isDealingDamage)
                        StartCoroutine(DeSpawnRingVisual(rings[i].aoeRingMat, rings[i]));

                    continue;
                }

                if (Time.time - rings[i].spawnTime > 2f)
                {
                    rings[i].DoInstantDamage();
                    Destroy(rings[i].gameObject);
                    rings.RemoveAt(i);
                }
            } 
        }
    }

    private IEnumerator SpawnRingVisual(Material mat, AOERing aoeRing)
    {
        float vfxAmount = 0.4f;

        while(vfxAmount < 1)
        {
            vfxAmount += Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);
            yield return null;
        }

        aoeRing.isDealingDamage = true;
        aoeRing.ringDamage = Random.Range(10, 25);  // not 
    }

    private IEnumerator DeSpawnRingVisual(Material mat, AOERing aoeRing)
    {
        float vfxAmount = 1f;
        aoeRing.isDealingDamage = false;

        while (vfxAmount > 0.4)
        {
            vfxAmount -= Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);
            yield return null;
        }

        Destroy(aoeRing.gameObject);
        rings.RemoveAt(rings.IndexOf(aoeRing));
    }
}
