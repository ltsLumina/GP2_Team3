using System;
using UnityEngine;

namespace NoSlimes.ObjectPools
{
    public static class ObjectPoolUtilities
    {
        public static T SpawnPoolable<T>(string poolName, Vector3 position = default, Quaternion rotation = default) where T : IPoolable
        {
            GameObject obj = ObjectPoolManager.GetPooledObject(poolName);

            if (obj != null)
            {
                if (obj.TryGetComponent(out T spawnedPoolable))
                {
                    spawnedPoolable.GameObject.transform.SetPositionAndRotation(position, rotation);

                    spawnedPoolable.OnSpawn();

                    return spawnedPoolable;
                }

                return default;
            }

            return default;
        }

        public static T SpawnPoolable<T>(GameObject prefab, Vector3 position = default, Quaternion rotation = default) where T : IPoolable
        {
            string poolName = GetPoolNameFromPrefab(prefab);
            return SpawnPoolable<T>(poolName, position, rotation);
        }

        public static bool TrySpawnPoolable<T>(string poolName, out T poolable, Vector3 position = default, Quaternion rotation = default) where T : IPoolable
        {
            poolable = SpawnPoolable<T>(poolName, position, rotation);
            return poolable != null;
        }

        public static bool TrySpawnPoolable<T>(GameObject prefab, out T poolable, Vector3 position = default, Quaternion rotation = default) where T : IPoolable
        {
            if (prefab == null)
            {
                poolable = default;
                return false;
            }

            string poolName = GetPoolNameFromPrefab(prefab);
            poolable = SpawnPoolable<T>(poolName, position, rotation);
            return poolable != null;
        }

        public static void RespawnPoolable<T>(T poolable, Vector3 position = default, Quaternion rotation = default) where T : IPoolable
        {
            if(poolable == null)
                return;

            poolable.ReturnToPool();
            
            poolable.GameObject.transform.SetPositionAndRotation(position, rotation);
            poolable.OnSpawn();
        }

        public static string GetPoolNameFromPrefab(GameObject prefab)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, GameObject> kvp in ObjectPoolManager.PoolPrefabs)
            {
                if (kvp.Value == prefab)
                {
                    return kvp.Key;
                }
            }

            return string.Empty;
        }

        public static Transform GetPoolParentFromPrefab(GameObject prefab)
        {
            string poolName = GetPoolNameFromPrefab(prefab);

            foreach (System.Collections.Generic.KeyValuePair<string, Transform> kvp in ObjectPoolManager.PoolParents)
            {
                if (kvp.Key == poolName)
                {
                    return kvp.Value;
                }
            }

            return null;
        }

    }

    [System.Serializable]
    public class PoolSettings
    {
        [SerializeField] private GameObject poolPrefab;
        [SerializeField] private int poolSize;

        public string PoolName
        {
            get
            {
                string poolName = "Invalid Pool";

                if (poolPrefab)
                    poolName = poolPrefab.name;

                return poolName;
            }
        }

        public GameObject PoolPrefab => poolPrefab;
        public int PoolSize => poolSize;

        public PoolSettings(GameObject poolPrefab, int poolSize)
        {
            this.poolPrefab = poolPrefab;
            this.poolSize = poolSize;
        }

        public void SetPrefab(GameObject newPrefab) => poolPrefab = newPrefab;
        public void SetSize(int newSize) => poolSize = newSize;

        public static bool operator ==(PoolSettings x, PoolSettings y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return x.Equals(y);
        }

        public static bool operator !=(PoolSettings x, PoolSettings y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (obj is PoolSettings other)
            {
                return Equals(poolPrefab, other.poolPrefab) && poolSize == other.poolSize;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (poolPrefab != null ? poolPrefab.GetHashCode() : 0);
                hash = hash * 31 + poolSize.GetHashCode();
                return hash;
            }
        }
    }

}