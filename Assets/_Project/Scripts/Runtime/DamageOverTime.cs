using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    private int damage;

    public void Initialize(int damage) => this.damage = damage;
    private float totalTimer = 0.3f;
    private float timer;

    private List<Enemy> enemyList = new List<Enemy>();

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer < 0)
        {
            timer = totalTimer;
            Collider[] others = Physics.OverlapSphere(transform.position, 3);

            foreach(Collider other in others)
            {
                if(other.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

}
