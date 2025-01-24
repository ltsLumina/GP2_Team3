using NoSlimes.ObjectPools;
using System;
using UnityEngine;

namespace NoSlimes.ObjectPools
{
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private PoolSettingsSO poolSettings;

        private void Awake()
        {
            foreach (string key in poolSettings.Pools.Keys)
            {
                foreach (PoolSettings pool in poolSettings.Pools[key])
                {
                    string poolName = $"{key}_{pool.PoolName}";
                    ObjectPoolManager.AddPool(poolName, pool.PoolPrefab, pool.PoolSize);
                }
            }
        }
    }
}
