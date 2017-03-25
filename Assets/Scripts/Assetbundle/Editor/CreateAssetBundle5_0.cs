using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System.Text;

/*******************************
 *  Author: Andrea Chow.
 *  Time:   
 *  Function: 创建assetbundle 5.0版本使用
 * *****************************/

public class CreateAssetBundle5_0
{
    //<string,Hash128> === <bundlename,hash>
    static Dictionary<string, Hash128> HashDic = new Dictionary<string, Hash128>();

    [MenuItem("Custom/AssetBundles/Clear Assetbundle")]
    static void ClearAssetbundle()
    {
        FileCustomHelper.DeleteFolderContent(AssetPath.packagePath);
        FileCustomHelper.DeleteFolderContent(AssetPath.cachedPath);

        //刷新资源编辑器
        AssetDatabase.Refresh();
    }


    [MenuItem("Custom/AssetBundles/Clear All Files")]
    static void ClearAllFiles()
    {
        FileCustomHelper.DeleteFolderContent(Application.persistentDataPath);

        //删除bundle，并刷新资源编辑器
        FileCustomHelper.DeleteFolderContent(AssetPath.packagePath);
        AssetDatabase.Refresh();
    }

    [MenuItem("Custom/AssetBundles/ReName Assetbundle(测试中，不用！！)")]
    static void ReNameAssets()
    {
        Object[] objs = Selection.objects;
        foreach (Object o in objs)
        {
            AssetDatabase.ClearLabels(o);
            AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(o));
            ai.assetBundleName = o.name;
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    [MenuItem("Custom/AssetBundles/Build Asset 5.x")]
    static void BuildAndroid()
    {
        BuildAssets(BuildTarget.Android);
    }

    [MenuItem("Custom/AssetBundles/Build Asset 5.x_PC64bit")]
    static void BuildPC64bit()
    {
        BuildAssets(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Custom/AssetBundles/Build Asset 5.x_PC32bit")]
    static void BuildPC32bit()
    {
        BuildAssets(BuildTarget.StandaloneWindows);
    }

    static void BuildAssets(BuildTarget target)
    {
        //put the bundles in a folder called "StreamingAssets" within the Assets folder
        string folderpath = AssetPath.packagePath;

        FileCustomHelper.CreateFolder(folderpath);

        //1.加载本地配置文件,设置HashDic
        string configFilePath = folderpath + "/" + "assetbundleConfig.assetbundle";

        HashDic.Clear();
        if (File.Exists(configFilePath))
        {
            using (StreamReader fs = new StreamReader(configFilePath, Encoding.Default))
            {
                string line = string.Empty;
                while (!string.IsNullOrEmpty(line = fs.ReadLine()))
                {
                    string[] str = line.Split(',');
                    HashDic.Add(str[0], Hash128.Parse(str[1]));
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("文件 " + configFilePath + "不存在！");
        }

        FileCustomHelper.DeleteFile(folderpath + "/" + AssetPath.bundleConfig);

        //2.比对更新HashDic并获得需要压缩的资源
        AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(folderpath, BuildAssetBundleOptions.UncompressedAssetBundle, target);
        if (abm)
        {
            UnityEngine.Debug.Log("AssetBundle打包完毕！");
        }

        string[] bundles = AssetDatabase.GetAllAssetBundleNames();
        UnityEngine.Debug.Log("所有资源个数：" + bundles.Length);

        Dictionary<string, Hash128> assetlist = new Dictionary<string, Hash128>();

        for (int i = 0; i < bundles.Length; i++)
        {
            assetlist.Add(bundles[i], abm.GetAssetBundleHash(bundles[i]));
        }

        List<string> updateList = new List<string>();

        foreach (KeyValuePair<string, Hash128> kv in assetlist)
        {
            //是否是新增资源
            if (HashDic.ContainsKey(kv.Key))
            {
                //更新资源
                if (!HashDic[kv.Key].Equals(kv.Value))
                {
                    HashDic[kv.Key] = kv.Value;
                }
                else continue;
            }
            else
            {
                //新增资源
                HashDic.Add(kv.Key, kv.Value);
            }
            updateList.Add(kv.Key);
        }

        //将assetbundlemanifest加入到更新字典中
        UnityEngine.Debug.Log("修改更新资源个数：" + updateList.Count);
        if (updateList.Count > 0)
        {
            Hash128 hash = Hash128.Parse(UniqueIDHelper.GetMD5HashFromFile(folderpath + "/" + AssetPath.bundleConfig));
            //Hash128 hash = Hash128.Parse(abm.GetInstanceID().ToString());
            
            //是否是新增资源
            if (HashDic.ContainsKey(AssetPath.bundleConfig))
            {
                //更新资源
                while (HashDic[AssetPath.bundleConfig].Equals(hash))
                {
                    hash = Hash128.Parse(abm.GetInstanceID().ToString());
                }
                UnityEngine.Debug.Log("hash128= " + hash);
                HashDic[AssetPath.bundleConfig] = hash;
            }
            else
            {
                //新增资源
                HashDic.Add(AssetPath.bundleConfig, hash);
            }
            updateList.Add(AssetPath.bundleConfig);
        }

        //3.压缩修改的或新增的资源
        for (int i = 0; i < updateList.Count; i++)
        {
            string bundleFilePath = folderpath + "/" + updateList[i];

            byte[] bytes = File.ReadAllBytes(bundleFilePath);

            //采用Gzip进行压缩
            bytes = GzipHelper.GzipCompress(bytes);

            using (FileStream fs = new FileStream(bundleFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
        }

        //4.更新本地配置文件
        using (FileStream fs = new FileStream(configFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            string content = string.Empty;
            foreach (KeyValuePair<string, Hash128> kv in HashDic)
            {
                content += kv.Key + "," + kv.Value.ToString() + "\n";
            }
            byte[] hashBytes = System.Text.Encoding.Default.GetBytes(content);
            fs.Write(hashBytes, 0, hashBytes.Length);
            fs.Flush();
        }

        //5.刷新资源编辑器
        AssetDatabase.Refresh();
    }
}
