using UnityEngine;
using UnityEngine.Events;

namespace NoSlimes.ObjectPools
{
    public interface IPoolable
    {
        GameObject GameObject { get; }
        Transform PoolTransform { get; }

        event UnityAction<IPoolable> OnSpawned;
        event UnityAction<IPoolable> OnReturnedToPool;

        /// <summary>
        /// Method that gets called when spawnable object is added to pool
        /// </summary>
        void OnAddToPool(Transform poolParent);

        /// <summary>
        /// Method that handles base logic for returning a spawnable object to the pool
        /// </summary>
        void ReturnToPool();

        /// <summary>
        /// Method that gets called on poolable object when it is spawned
        /// </summary>
        void OnSpawn();
    }
}