using UnityEngine;
using System.Collections.Generic;

/*******************************
 *  Author: vekcon.zx.
 *  Time:   
 *  Function: GameObject poolmanager(对象池)
 * *****************************/

public class PoolManager : MonoBehaviour
{
    static PoolManager mInstance;
    public static PoolManager singleton
    {
        get
        {
            if (mInstance == null)
                mInstance = new GameObject("PoolManager").AddComponent<PoolManager>();
            return mInstance;
        }
        set
        {
            mInstance = value;
        }
    }

    [System.Serializable]
    public class PrefabLoader
    {
        public GameObject prefab;
        public int loadCount;
    }

    public class PoolPrefab
    {
        public string objname;
        public List<GameObject> instantiate;
        public int maxCount;            //总数量

        int m_AvailableCount;           //可用数量
        public int availableCount       
        {
            get
            {
                return m_AvailableCount;
            }
            set
            {
                m_AvailableCount = value;
                lastRecordTime = Time.realtimeSinceStartup;
            }
        }
        
        public float lastRecordTime;    //最后活跃时间

        public PoolPrefab()
        {
            instantiate = new List<GameObject>();
            maxCount = 0;
            availableCount = 0;
        }
    }
    public float checkTime = 301;

    Transform cache;

    /// <summary>
    /// 预加载预置
    /// </summary>
    public List<PrefabLoader> preLoadPoolPrefab = new List<PrefabLoader>();

    List<PoolPrefab> poolPrefabDic = new List<PoolPrefab>();
    bool TryGetValue(string _name , out PoolPrefab item)
    {
        for(int i=poolPrefabDic.Count -1;i>=0;i--)
        {
            if(poolPrefabDic[i].objname.Equals(_name))
            {
                item = poolPrefabDic[i];
                return true;
            }
        }
        item = null;
        return false;
    }

    /// <summary>
    /// 是否开启暂停继续物体功能
    /// </summary>
    public static bool pauseAndConsume;

    List<GameObject> activeObject = new List<GameObject>();
    List<MonoBehaviour> monos = new List<MonoBehaviour>();
    List<Animator> animators = new List<Animator>();
    List<Animation> animations = new List<Animation>();
    List<ParticleSystem> particles = new List<ParticleSystem>();

    public void Awake()
    {
        singleton = this;
        cache = transform;
        poolPrefabDic.Clear();

        activeObject.Clear();
        monos.Clear();
        animators.Clear();
        animations.Clear();
        particles.Clear();

        InitializePool();

        if (checkTime > 0)
        {
            InvokeRepeating("CheckTimer", checkTime, checkTime);
        }
    }

    void CheckTimer()
    {
        for (int i = poolPrefabDic.Count - 1; i >= 0; i--)
        {
            PoolPrefab pp = poolPrefabDic[i];
            //可用数量==最大数量说明这些物体已经没有被使用了
            if (Time.realtimeSinceStartup - pp.lastRecordTime > 5f * 60)
            {
                for (int j = pp.maxCount - 1; j >= 0 && pp.availableCount > 0; j--)
                {
                    if (!pp.instantiate[i].activeSelf || !pp.instantiate[i].activeInHierarchy)
                    {
                        DestroyImmediate(pp.instantiate[i]);
                        pp.instantiate.RemoveAt(i);
                        pp.maxCount--;
                        pp.availableCount--;
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        activeObject.Clear();
        monos.Clear();
        animators.Clear();
        animations.Clear();
        particles.Clear();
        ReleasePoolManager();
        singleton = null;
    }

    /// <summary>
    /// 彻底回收全部创建出来的物体
    /// </summary>
    void ReleasePoolManager()
    {
        for (int i = poolPrefabDic.Count - 1; i >= 0; i--)
        {
            PoolPrefab pp = poolPrefabDic[i];
            for (int j = pp.instantiate.Count - 1; j >= 0; j--)
            {
                Destroy(pp.instantiate[j]);
                pp.instantiate.RemoveAt(j);
            }
            pp.maxCount = 0;
            pp.availableCount = 0;
        }
    }

    void ReleaseObject()
    {

    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <returns></returns>
    public void InitializePool()
    {
        for (int i = preLoadPoolPrefab.Count - 1; i >= 0; i--)
        {
            int count = preLoadPoolPrefab[i].loadCount;
            if (count > 0)
            {
                InsertPrefabDic(count, preLoadPoolPrefab[i].prefab);
            }
        }
    }

    /// <summary>
    /// 动态初始化内存池
    /// </summary>
    /// <param name="count"></param>
    /// <param name="prefab"></param>
    public void InitializePool(int count, GameObject prefab)
    {
        if (count > 0)
        {
            InsertPrefabDic(count, prefab);
        }
    }

    /// <summary>
    /// 插入到预置字典
    /// </summary>
    void InsertPrefabDic(int count, GameObject p)
    {
        //如果该物体已经存在字典里，但是没有可用的则新建一个
        string prefabName = p.name + "(Clone)";

        GameObject go = null;
        PoolPrefab pp;
        if (TryGetValue(prefabName,out pp))
        {
            for (int k = 0; k < count; k++)
            {
                go = Instantiate(p) as GameObject;
                go.SetActive(false);
                go.transform.SetParent(cache);

                pp.instantiate.Add(go);
                pp.maxCount++;
                pp.availableCount++;
            }
            return;
        }

        //该物体不存在字典里面则直接创建
        
        pp = new PoolPrefab();
        for (int i = 0; i < count; i++)
        {
            go = Instantiate(p) as GameObject;
            go.SetActive(false);
            go.transform.SetParent(cache);

            pp.instantiate.Add(go);
            pp.maxCount++;
            pp.availableCount++;
        }
        pp.objname = go.name;
        poolPrefabDic.Add(pp);
    }

    /// <summary>
    /// 将一个已经创建好的物体加入到预置池以便管理.
    /// </summary>
    /// <param name="prefab"></param>
    /// 
    public void SampleInsertPoolDic(GameObject prefab)
    {
        PoolPrefab pp;
        if (TryGetValue(prefab.name, out pp))
        {
            pp.instantiate.Add(prefab);
            pp.maxCount++;

            if (!prefab.activeSelf )
                pp.availableCount++;
        }
        else
        {
            pp = new PoolPrefab();
            pp.objname = prefab.name;
            pp.instantiate.Add(prefab);
            pp.maxCount++;

            if (!prefab.activeSelf)
                pp.availableCount++;
            poolPrefabDic.Add(pp);
        }
    }

    /// <summary>
    /// 创建(获取)一个预置
    /// </summary>
    public GameObject Create(GameObject prefab)
    {
        string prefabName = prefab.name + "(Clone)";

        PoolPrefab pp;
        if (TryGetValue(prefabName, out pp))
        {
            for (int i = 0, maxCount = pp.maxCount; i < maxCount; i++)
            {
                GameObject temp = pp.instantiate[i];

                if (!temp.activeSelf || !temp.activeInHierarchy)
                {
                    temp.transform.SetParent(cache);
                    temp.transform.localEulerAngles = Vector3.zero;
                    temp.transform.localPosition = Vector3.zero;
                    temp.transform.localScale = Vector3.one;

                    temp.SetActive(true);
                    if (pauseAndConsume)
                        activeObject.Add(temp);


                    pp.availableCount--;

                    return temp;
                }
            }
        }

        //没有需要的物体
        InsertPrefabDic(1, prefab);
        return Create(prefab);
    }

    /// <summary>
    /// 创建（获取）物体
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="pos">世界坐标</param>
    /// <param name="parent">父节点</param>
    /// <returns></returns>
    public GameObject Create(GameObject prefab, Vector3 pos, Transform parent = null)
    {
        string prefabName = prefab.name + "(Clone)";

        PoolPrefab pp;
        if (TryGetValue(prefabName, out pp))
        {
            for (int i = 0, maxCount = pp.maxCount; i < maxCount; i++)
            {
                GameObject temp = pp.instantiate[i];

                if (!temp.activeSelf || !temp.activeInHierarchy)
                {
                    if (parent == null) parent = cache;

                    temp.transform.SetParent(parent);
                    temp.transform.position = pos;
                    temp.SetActive(true);

                    if (pauseAndConsume)
                        activeObject.Add(temp);

                    pp.availableCount--;
                    return temp;
                }
            }
        }

        //没有需要的物体
        InsertPrefabDic(1, prefab);
        return Create(prefab, pos, parent);
    }

    /// <summary>
    /// 回收预置
    /// </summary>
    public void DestroyEx(GameObject destroyGo)
    {
        PoolPrefab pp;
        if (TryGetValue(destroyGo.name, out pp))
        {
            destroyGo.SetActive(false);
            destroyGo.transform.SetParent(cache);
            pp.availableCount++;

            if (pauseAndConsume)
                activeObject.Remove(destroyGo);
        }
    }

    /// <summary>
    /// 暂停所有物体
    /// </summary>
    public void PauseGameObjects()
    {
        monos.Clear();
        particles.Clear();
        animators.Clear();
        animations.Clear();

        for (int i = 0; i < activeObject.Count; i++)
        {
            GameObject go = activeObject[i];
            if (go.activeSelf)
            {
                MonoBehaviour[] mono = go.GetComponentsInChildren<MonoBehaviour>();
                for (int j = mono.Length - 1; j >= 0; j--)
                {
                    mono[j].enabled = false;
                    monos.Add(mono[j]);
                }
                ParticleSystem[] particle = go.GetComponentsInChildren<ParticleSystem>();
                for (int j = particle.Length - 1; j >= 0; j--)
                {
                    particle[j].Pause(true);
                    particles.Add(particle[j]);
                }
                Animator[] ar = go.GetComponentsInChildren<Animator>();
                for (int j = ar.Length - 1; j >= 0; j--)
                {
                    ar[j].enabled = false;
                    animators.Add(ar[j]);
                }
                Animation[] an = go.GetComponentsInChildren<Animation>();
                for (int j = an.Length - 1; j >= 0; j--)
                {
                    an[j].enabled = false;
                    animations.Add(an[j]);
                }
            }
        }
    }

    /// <summary>
    /// 继续所有物体
    /// </summary>
    public void ContinueGameObjects()
    {
        for (int j = monos.Count - 1; j >= 0; j--)
        {
            monos[j].enabled = true;
        }
        for (int j = particles.Count - 1; j >= 0; j--)
        {
            particles[j].Play(true);
        }
        for (int j = animators.Count - 1; j >= 0; j--)
        {
            animators[j].enabled = true;
        }
        for (int j = animations.Count - 1; j >= 0; j--)
        {
            animations[j].enabled = true;
        }
    }

    /// <summary>
    /// 暂停一个指定的物体
    /// </summary>
    public void PauseGameObject(GameObject go)
    {
        if (go.activeSelf)
        {
            MonoBehaviour[] mono = go.GetComponentsInChildren<MonoBehaviour>();
            for (int j = mono.Length - 1; j >= 0; j--)
            {
                mono[j].enabled = false;
            }

            ParticleSystem[] particle = go.GetComponentsInChildren<ParticleSystem>();
            for (int j = particle.Length - 1; j >= 0; j--)
            {
                particle[j].Pause(true);
            }
            Animator[] ar = go.GetComponentsInChildren<Animator>();
            for (int j = ar.Length - 1; j >= 0; j--)
            {
                ar[j].enabled = false;
            }
            Animation[] an = go.GetComponentsInChildren<Animation>();
            for (int j = an.Length - 1; j >= 0; j--)
            {
                an[j].enabled = false;
            }
        }
    }

    /// <summary>
    /// 继续一个指定的物体
    /// </summary>
    public void ContinueGameObject(GameObject go)
    {
        MonoBehaviour[] mono = go.GetComponentsInChildren<MonoBehaviour>();
        for (int j = mono.Length - 1; j >= 0; j--)
        {
            mono[j].enabled = true;
        }
        ParticleSystem[] particle = go.GetComponentsInChildren<ParticleSystem>();
        for (int j = particle.Length - 1; j >= 0; j--)
        {
            particle[j].Pause(false);
        }
        Animator[] ar = go.GetComponentsInChildren<Animator>();
        for (int j = ar.Length - 1; j >= 0; j--)
        {
            ar[j].enabled = true;
        }
        Animation[] an = go.GetComponentsInChildren<Animation>();
        for (int j = an.Length - 1; j >= 0; j--)
        {
            an[j].enabled = true;
        }
    }
}