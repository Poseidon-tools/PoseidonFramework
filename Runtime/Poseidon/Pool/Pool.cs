namespace Core.Pool
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum PoolExpandMethods
    {
        Single,
        Half,
        Double,
    }

    public class Pool<T> where T : Component
    {
        
        private int poolSize;
        private readonly T poolObject;
        private readonly Transform parentTransform;
        private readonly Stack<T> poolStack;
        private readonly PoolExpandMethods expandMethod;

        public List<T> SpawnedObjects { get; }

        /// <summary>
        /// Pool system dedicated for Unity Component
        /// </summary>
        /// <param name="poolObject">Poolable object that will be spawned by the pool</param>
        /// <param name="parentTransform">Parent transform for poolObjects</param>
        /// <param name="initialSize">Initial max size of the pool</param>
        /// <param name="expandMethod">Possible methods how to handle pool expanding</param>
        public Pool(T poolObject, Transform parentTransform = null, int initialSize = 10, PoolExpandMethods expandMethod = PoolExpandMethods.Double)
        {
            poolSize = initialSize;
            poolStack = new Stack<T>(poolSize);
            SpawnedObjects = new List<T>(poolSize);
            this.parentTransform = parentTransform;
            this.poolObject = poolObject;
            this.expandMethod = expandMethod;

            for (int i = 0; i < poolSize; i++)
            {
                poolStack.Push(CreateObject());
            }
        }

        public T Spawn()
        {
            return Spawn(Vector3.zero);
        }

        public T Spawn(Vector3 position)
        {
            return Spawn(position, Quaternion.identity);
        }

        public T Spawn(Vector3 position, Quaternion rotation)
        {
            return Spawn(position, rotation, parentTransform);
        }

        public T Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            if (poolStack.Count == 0)
            {
                ExpandPool();
            }

            var item = poolStack.Pop();
            item.transform.SetParent(parent);
            item.gameObject.SetActive(true);
            var transform = item.transform;
            transform.position = position;
            transform.rotation = rotation;

            SpawnedObjects.Add(item);

            return item.GetComponent<T>();
        }

        public void ReturnToPool(T item)
        {
            //prevents from returning the same item twice
            if (poolStack.Contains(item))
                return;
            
            //remove item from the spawned objects list if possible
            bool doesPoolCointainTheItem = SpawnedObjects.Remove(item);

            //prevents from returning the item that wasn't spawned by this pool
            if (!doesPoolCointainTheItem)
                return;

            poolStack.Push(item);
            item.transform.SetParent(parentTransform);
            item.gameObject.SetActive(false);
        }

        public void ReturnAllToPool()
        {
            for (int i = SpawnedObjects.Count-1; i >= 0; i--)
            {
                ReturnToPool(SpawnedObjects[i]);
            }
        }

        private void ExpandPool()
        {
            switch (expandMethod)
            {
                case PoolExpandMethods.Half:
                    Resize(poolSize == 0 ? 1 : (poolSize / 2) + 1);
                    break;
                case PoolExpandMethods.Single:
                    Resize(poolSize + 1);
                    break;
                case PoolExpandMethods.Double:
                    Resize(poolSize == 0 ? 1 : poolSize * 2);
                    break;
                default:
                    break;
            }
            Debug.LogWarning($"{typeof(T)} pool expanded using {expandMethod} method. New size = {poolSize}");
        }

        private void Resize(int desiredPoolSize)
        {
            if (poolSize == desiredPoolSize) return;

            while (desiredPoolSize < poolStack.Count)
            {
                Object.Destroy(poolStack.Pop());
            }

            while (desiredPoolSize > poolStack.Count)
            {
                poolStack.Push(CreateObject());
            }

            poolSize = desiredPoolSize;
        }

        private T CreateObject()
        {
            var newItem = Object.Instantiate(poolObject, parentTransform);
            newItem.gameObject.SetActive(false);

            return newItem;
        }
    }
}
