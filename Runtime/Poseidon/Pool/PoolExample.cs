namespace Core.Pool
{
    using UnityEngine;
    using DG.Tweening;

    public class PoolExample : MonoBehaviour
    {
        public GameObject prefabToSpawn;
        private Pool<Transform> pool;
        void Start()
        {
            int iterations = 12;
            float distanceToSpawn = 10;
            
            pool = new Pool<Transform>(prefabToSpawn.transform, transform, 10, PoolExpandMethods.Half);
            for(int i = 0; i < iterations; i++)
            {
                float lerp = i / (float) iterations;
                
                pool.Spawn(new Vector3(Mathf.Lerp(0, distanceToSpawn, lerp), 0, 0),
                           Quaternion.AngleAxis(Mathf.Lerp(0, 360f, lerp), Vector3.up));
            }
            DOVirtual.DelayedCall(3f, () => { pool.ReturnAllToPool(); });
        }
    }
}
