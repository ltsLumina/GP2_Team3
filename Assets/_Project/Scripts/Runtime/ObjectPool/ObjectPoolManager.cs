using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace NoSlimes.ObjectPools
{
    [DefaultExecutionOrder(-1)]
    public class ObjectPoolManager : MonoBehaviour
    {
        private readonly static Dictionary<string, List<IPoolable>> pools = new();
        private readonly static Dictionary<string, GameObject> poolPrefabs = new();
        private readonly static Dictionary<string, Transform> poolParents = new();

        private static Transform poolBaseParent;

        public static ReadOnlyDictionary<string, List<IPoolable>> Pools => new(pools);
        public static ReadOnlyDictionary<string, GameObject> PoolPrefabs => new(poolPrefabs);
        public static ReadOnlyDictionary<string, Transform> PoolParents => new(poolParents);

        private void Awake()
        {
            pools.Clear();
            poolPrefabs.Clear();
            poolParents.Clear();
        }

        /// <summary>
        /// Adds a new pool of GameObjects, with desired name and pool size
        /// </summary>
        /// <param name="poolName">The name of the new pool</param>
        /// <param name="prefab">The prefab of the GameObject to make a pool of</param>
        /// <param name="poolSize">The size of the new pool</param>
        public static void AddPool(string poolName, GameObject prefab, int poolSize)
        {
            poolBaseParent = poolBaseParent != null ? poolBaseParent : new GameObject("ObjectPools").transform;

            if (!pools.ContainsKey(poolName))
            {
                pools[poolName] = new List<IPoolable>();
            }

            Transform poolParent = new GameObject(poolName).transform;
            poolParent.transform.SetParent(poolBaseParent);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject go = Object.Instantiate(prefab, poolParent);
                go.SetActive(false);

                IPoolable pooledSpawnable = go.GetComponent<IPoolable>();

                pools[poolName].Add(pooledSpawnable);

                pooledSpawnable.OnAddToPool(poolParent);
            }

            poolPrefabs[poolName] = prefab;
            poolParents[poolName] = poolParent;
        }

        /// <summary>
        /// Removes the given pool along with of all its objects
        /// </summary>
        /// <param name="poolName">The name of the pool to remove</param>
        public static void RemovePool(string poolName)
        {
            if (pools.ContainsKey(poolName))
            {
                Transform poolParent = poolBaseParent.Find(poolName);

                for (int i = 0; i < poolParent.childCount; i++)
                {
                    Object.Destroy(poolParent.GetChild(0).gameObject);
                }
                Object.Destroy(poolParent.gameObject);

                pools.Remove(poolName);
                poolPrefabs.Remove(poolName);
                poolParents.Remove(poolName);
            }
            else
            {
                Debug.LogWarning($"No object pool with collectionName \"{poolName}\" exists!");
            }
        }

        /// <summary>
        /// Gets an GameObject from the pool
        /// </summary>
        /// <param name="poolName">The name of the pool to get an GameObject from</param>
        /// <returns>Returns a GameObject from the pool, if there is one available</returns>
        public static GameObject GetPooledObject(string poolName)
        {
            if (pools.ContainsKey(poolName) && pools[poolName].Count > 0)
            {
                for (int i = 0; i < pools[poolName].Count; i++)
                {
                    if (!pools[poolName][i].GameObject.activeInHierarchy)
                    {
                        //Return the object
                        return pools[poolName][i].GameObject;
                    }
                }

                Debug.LogWarning($"All objects in pool \"{poolName}\" are active in the scene - <color=red>pool depleted</color>");
                return null;
            }
            else
            {
                Debug.LogWarning($"No object pool with collectionName \"{poolName}\" exists!");
            }
            return null;
        }

        public static bool HasPool(string poolName) => pools.ContainsKey(poolName);
        public static bool HasPool(GameObject poolObject) => PoolPrefabs.Values.Contains(poolObject);
    }
}