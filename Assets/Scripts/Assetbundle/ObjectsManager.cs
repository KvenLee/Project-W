using UnityEngine;
using System.Collections.Generic;

/*******************************
 *  Author: Andrea Chow.
 *  Time:   2015.07.28
 *  Function: 物体保存(bundle 加载)
 * *****************************/

/// <summary>
/// bundle 所属类型
/// </summary>
public struct BundleBelong
{
    public static string animation = "animations.assetbundle";
    public static string data = "datas.assetbundle";
    public static string prefab = "prefabs.assetbundle";
    public static string map = "maps.assetbundle";
    public static string material = "materials.assetbundle";
    public static string texture = "textures.assetbundle";
    public static string sprite = "sprites.assetbundle";
    public static string ui = "uis.assetbundle";
    public static string skin = "skins.assetbundle";
    public static string tables = "tableres.assetbundle";
    public static string model = "models.assetbundle";
    public static string audio = "audios.assetbundle";
    public static string effect = "effects.assetbundle";
    public static string buff = "buffs.assetbundle";
    public static string scene = "scenes.assetbundle";
    public static string ugui = "ugui.assetbundle";
    public static string altas = "atlas.assetbundle";
    public static string loading = "loadingatlas.assetbundle";
    public static string chipicon = "chipiconatlas.assetbundle";
    public static string halficon = "halficonatlas.assetbundle";
    public static string headicon = "headiconatlas.assetbundle";
    public static string itemicon = "itemiconatlas.assetbundle";
}

/// <summary>
/// 资源目录
/// </summary>
public class AssetPath
{
    public static string bundleConfig = "gameassetbundle";

    public static string packagePath = Application.streamingAssetsPath + "/" + bundleConfig;
    public static string downloadPath =

#if UNITY_EDITOR
    "file://" + Application.streamingAssetsPath + "/" + bundleConfig + "/";
#elif UNITY_ANDROID
      "jar:file://" + Application.dataPath + "!/assets/" + bundleConfig + "/";
#elif UNITY_IPHONE
      Application.dataPath + "/Raw/" +bundleConfig+ "/";
#else
    "file://" + Application.streamingAssetsPath + "/" + bundleConfig + "/";
#endif

    public static string cachedPath = Application.persistentDataPath + "/" + bundleConfig;

    public static string gameDataPath = Application.persistentDataPath + "/gameData";
}

public class ObjectsManager
{
    static bool loadFromResources = false;
    static AssetBundle manifestBundle = null;
    static AssetBundleManifest abm = null;

    static Dictionary<string, Object> LoadedObjects = new Dictionary<string, Object>();
    static Dictionary<string, AssetBundle> LoadedAB = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// Load Object from assetbundle use unity 5.0 version function;
    /// </summary>
    /// <param name="bundleName">assetbundle name (like:bundle.assetbundle)</param>
    /// <param name="gameName">name of needed gameObject (like:player)</param>
    /// <returns></returns>
    static T Load<T>(string bundleName, string gameName) where T : Object
    {
        string dicname = bundleName + "_" + gameName;

        Object needGo = null;
        if(LoadedObjects.TryGetValue(dicname, out needGo))
        {
            return needGo as T;
        }
        else
        {
            //T needGo = null;
            if(loadFromResources)
            {
                needGo = Resources.Load<T>(bundleName + "/" + gameName);
            }
            else
            {
                AssetBundle needBundle = null;
                if(!LoadedAB.TryGetValue(bundleName, out needBundle))
                {
                    try
                    {
                        //load object from assetbundle!
                        if(manifestBundle == null && FileCustomHelper.CheckIfExist(AssetPath.cachedPath + "/" + AssetPath.bundleConfig))
                            manifestBundle = AssetBundle.LoadFromFile(AssetPath.cachedPath + "/" + AssetPath.bundleConfig);

                        if(manifestBundle)
                        {
                            if(abm == null)
                                abm = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                            //获取依赖文件列表
                            string[] dependInfos = abm.GetAllDependencies(bundleName);
                            for(int i = 0; i < dependInfos.Length; i++)
                            {
                                //加载所有依赖bundle
                                if(!LoadedAB.TryGetValue(dependInfos[i], out needBundle))
                                {
                                    needBundle = AssetBundle.LoadFromFile(AssetPath.cachedPath + "/" + dependInfos[i]);
                                    LoadedAB.Add(dependInfos[i], needBundle);
                                }
                            }

                            //加载所需文件
                            needBundle = AssetBundle.LoadFromFile(AssetPath.cachedPath + "/" + bundleName);
                            LoadedAB.Add(bundleName, needBundle);

                            // needGo = needBundle.LoadAsset<T>(gameName);

                            //卸载加载的 assetbundle
                            //manifestBundle.Unload(false);
                            //                         for (int i = 0; i < dependAssetbundles.Length; i++)
                            //                             dependAssetbundles[i].Unload(false);
                            //                         needBundle.Unload(false);
                        }
                        else
                        {
                            UnityEngine.Debug.Log("manifestBundle == null");
                        }
                    }
                    catch(System.Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
                if(needBundle != null)
                {
                    needGo = needBundle.LoadAsset<T>(gameName);
                }
                if(needGo == null)
                {
                    //load from resources
                    needGo = Resources.Load<T>(bundleName + "/" + gameName);
                }
            }
            if(needGo != null)
            {
                LoadedObjects.Add(dicname, needGo);
            }
            return needGo as T;
        }
    }

    /// <summary>
    /// 加载资源并创建一个GameObject
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="gameName"></param>
    /// <returns></returns>
    public static GameObject CreateGameObject(string bundleName, string gameName, bool usepoolmanager)
    {
        GameObject go = Load<GameObject>(bundleName, gameName);
        if(go == null)
            return null;

        if(usepoolmanager)
            return PoolManager.singleton.Create(go);
        else
            return Object.Instantiate(go) as GameObject;
    }

    /// <summary>
    /// 加载资源 资源类型为 T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bundleName"></param>
    /// <param name="gameName"></param>
    /// <returns></returns>
    public static T LoadObject<T>(string bundleName, string gameName) where T : Object
    {
        if(string.IsNullOrEmpty(gameName) || string.IsNullOrEmpty(bundleName))
            return null;
        return Load<T>(bundleName, gameName);
    }

    public static void UnloadAll()
    {
        //unload assetbundle
        foreach(KeyValuePair<string, AssetBundle> kv in LoadedAB)
        {
            if(kv.Value != null)
            {
                kv.Value.Unload(true);
            }
        }
        LoadedAB.Clear();
        LoadedObjects.Clear();
        Resources.UnloadUnusedAssets();
    }

    public static void UnloadManual(string bundleName, string gameName)
    {
        string dicname = bundleName + "_" + gameName;
        if(LoadedObjects.ContainsKey(dicname))
        {
            LoadedObjects[dicname] = null;
            LoadedObjects.Remove(dicname);
        }
    }
    public static void UnloadLoaded(Object instance)
    {
        if(instance == null)
            return;
        foreach(KeyValuePair<string, Object> kv in LoadedObjects)
        {
            if(kv.Value == instance)
            {
                Resources.UnloadAsset(kv.Value);
                LoadedObjects[kv.Key] = null;
                LoadedObjects.Remove(kv.Key);
                break;
            }
        }
    }

    /// <summary>
    /// 直接卸载资源
    /// </summary>
    /// <param name="instance"></param>
    public static void UnLoadDirectly(Object instance)
    {
        if(instance != null)
        {
            Resources.UnloadAsset(instance);
        }
    }

    //NGUI材质球和贴图内存优化===Resources加载模式下

    static Dictionary<Object, int> m_AtlasDic = new Dictionary<Object, int>();
    public static void AddUIObj(Object atlas)
    {
        if(atlas == null)
            return;
        if(m_AtlasDic.ContainsKey(atlas))
        {
            m_AtlasDic[atlas]++;
        }
        else
        {
            m_AtlasDic.Add(atlas, 1);
        }
    }
    public static void RemoveUIObj(Object atlas)
    {
        if(atlas == null)
            return;
        if(m_AtlasDic.ContainsKey(atlas))
        {
            m_AtlasDic[atlas]--;
            if(m_AtlasDic[atlas] <= 0)
            {
                UnloadLoaded(atlas);
            }
        }
    }

    //=============
    public Sprite LoadHalfIcon(string icon_name)
    {
        string bundle_name = "halficon" + icon_name;
        return Load<Sprite>(bundle_name, icon_name);
    }

    public Sprite LoadHeadIcon(string icon_name)
    {
        string bundle_name = "headicon" + icon_name;
        return Load<Sprite>(bundle_name, icon_name);
    }
    public Sprite LoadChipIcon(string icon_name)
    {
        string bundle_name = "chipicon" + icon_name;
        return Load<Sprite>(bundle_name, icon_name);
    }
}
