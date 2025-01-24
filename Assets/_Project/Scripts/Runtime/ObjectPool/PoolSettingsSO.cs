using NoSlimes.ObjectPools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace NoSlimes
{
    [CreateAssetMenu(menuName = "PoolSettings", fileName = "new PoolSettings")]
    public class PoolSettingsSO : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<string, List<PoolSettings>> pools = new();
        public Dictionary<string, List<PoolSettings>> Pools => new(pools);

        public event UnityAction OnPoolsChanged;

        public void AddPool(PoolSettings poolSettings, string poolCollection = "default")
        {
            if (pools[poolCollection].FirstOrDefault(p1 => p1 == poolSettings) != null)
                return;

            if (!pools.ContainsKey(poolCollection))
                pools.Add(poolCollection, new List<PoolSettings>());

            pools[poolCollection].Add(poolSettings);

            OnPoolsChanged?.Invoke();
        }

        public void RemovePool(PoolSettings poolSettings, string poolCollection = "default")
        {
            if (!pools[poolCollection].Contains(poolSettings))
                return;

            pools[poolCollection].Remove(poolSettings);
            OnPoolsChanged?.Invoke();
        }

        public bool HasPool(GameObject poolPrefab, string poolCollection = "default")
        {
            return pools[poolCollection].FirstOrDefault(p => p.PoolPrefab == poolPrefab) != null;
        }
    }
}
