#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EditorHelper : Editor
{
    [MenuItem("Custom/UGUI/ArialFont2MSYH")]
    static void ArialFont2MSYH()
    {
        string path = "Assets/ResourcesEx/" + BundleBelong.ui;
        Font msyh = AssetDatabase.LoadAssetAtPath<Font>("Assets/ResourcesEx/fonts.assetbundle/msyh.ttf");
        foreach (string directory in System.IO.Directory.GetFiles(path))
        {
            string dir = directory.Replace("\\", "/");
            if (dir.EndsWith(".prefab"))
            {
                GameObject preafab = AssetDatabase.LoadAssetAtPath<GameObject>(dir);
                Text[] texts = preafab.GetComponentsInChildren<Text>(true);
                foreach (Text t in texts)
                {
                    if (t.font.name != "JDJXINGKAI")
                    {
                        Debug.Log(t.font + " " + preafab.name);
                        t.font = msyh;
                        t.fontStyle = FontStyle.Normal;
                    }
                }
                EditorUtility.SetDirty(preafab);
            }
        }
    }
    [MenuItem("Custom/UGUI/PackAtlas")]
    static void PackUGUIAtlas()
    {
        string path = "Assets/Data/UGUI/UI组件";
        string prefabPath = "Assets/ResourcesEx/" + BundleBelong.altas + "/altas.prefab";

        GameObject go = new GameObject("atlas");
        GameObject pre = PrefabUtility.CreatePrefab(prefabPath, go);
        UGUIAtlasCfg cfg = pre.AddComponent<UGUIAtlasCfg>();
        cfg.atlasCfg.Clear();

        foreach(string directory in System.IO.Directory.GetFiles(path))
        {
            string dir = directory.Replace("\\", "/");
            if(dir.EndsWith(".png"))
            {
                Sprite prefab = AssetDatabase.LoadAssetAtPath<Sprite>(dir);
                cfg.atlasCfg.Add(prefab.name, prefab);
            }
        }
        Debug.Log(cfg.atlasCfg.Count);
        AssetImporter ai = AssetImporter.GetAtPath(prefabPath);
        ai.assetBundleName = BundleBelong.altas;
        DestroyImmediate(go);
    }
}
#endif
