using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public interface IPoolObject
    {
        /// <summary>
        /// Obje ilk defa create olduğunda çalışır
        /// </summary>
        void OnObjectSpawn();
        /// <summary>
        /// Obje pool'dan çekildiğinde çalışır
        /// </summary>
        void OnObjectGet();

        /// <summary>
        /// Obje pool'a geri döndüğünde çalışır
        /// </summary>
        void OnObjectDeactive();
    }

    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject ObjectPrefab;
        public int ObjectCount;
    }

    public class PoolManager : Manager<PoolManager>
    {
        [Header("Pools")]
        [SerializeField] private List<Pool> Pools;
        private Dictionary<string, Queue<GameObject>> PoolDictionary;

        private void Start()
        {
            CreatePools();
        }

        public void CreatePools()
        {
            StartCoroutine(CretaePoolEnum());
        }

        private IEnumerator CretaePoolEnum()
        {
            PoolDictionary = new Dictionary<string, Queue<GameObject>>();
            foreach (var pool in Pools)
            {
                Queue<GameObject> ObjectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.ObjectCount; i++)
                {
                    GameObject obj = Instantiate(pool.ObjectPrefab);
                    obj.transform.SetParent(transform);
                    obj.SetActive(false);

                    if (obj.TryGetComponent<IPoolObject>(out var iPoolObject)) iPoolObject.OnObjectSpawn();

                    ObjectPool.Enqueue(obj);

                    if (i % 20 == 0) yield return null;
                }

                PoolDictionary.Add(pool.Tag, ObjectPool);
            }
        }

        public T GetObjectFromPool<T>(string tag, Vector3? position, Quaternion? rotation, float lifeTime = 2)
        {
            if (!PoolDictionary.ContainsKey(tag)) return default;

            GameObject obj = PoolDictionary[tag].Dequeue();
            if (position != null) obj.transform.position = position.Value;
            if (rotation != null) obj.transform.rotation = rotation.Value;


            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolObject>(out var iPoolObject)) iPoolObject.OnObjectGet();

            PoolDictionary[tag].Enqueue(obj);

            StartCoroutine(DeactiveObject(lifeTime, obj));

            if (obj.TryGetComponent<T>(out var tObj))
                return tObj;
            else
                return default;
        }


        private IEnumerator DeactiveObject(float lifeTime, GameObject obj)
        {
            yield return new WaitForSeconds(lifeTime);
            if (obj.TryGetComponent<IPoolObject>(out var iPoolObject)) iPoolObject.OnObjectDeactive();
            obj.SetActive(false);
        }

    }
}