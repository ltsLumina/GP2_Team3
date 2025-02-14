using System;
using System.Collections;
using UnityEngine;

public class EnemyDeathVFX : MonoBehaviour
{
    private Enemy enemy;
    private Material mat;
    private Action enemyDeath;
    [SerializeField] private ParticleSystem bloodParticles;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        mat = GetComponentInChildren<Transform>().GetComponentInChildren<Renderer>().material;
        enemy.OnDeath += StartVFX;
    }

    private void StartVFX()
    {
        StartCoroutine(DeathVFX());
        Instantiate(bloodParticles.gameObject, gameObject.transform.position, Quaternion.identity);
    }


    private IEnumerator DeathVFX()
    {
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime;
            yield return null;
        }


        float vfxAmount = 0f;

        while (vfxAmount < 1)
        {
            vfxAmount += Time.deltaTime / 3;
            mat.SetFloat("_Dissolve", vfxAmount);
            yield return null;
        }

        Destroy(gameObject);
    }
}
