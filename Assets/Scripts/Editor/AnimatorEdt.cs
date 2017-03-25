#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

/// <summary>e
/// 序列帧转换为Animator动画
/// </summary>
/// 
public class AnimatorEdt : Editor
{
    public static string pathRoot = "Assets/Data/Role/2D";
    public static string mapRoot = "Assets/Data/Map";
    public static string fxRoot = "Assets/Data/Fx";

    public static string animatorControllerPath = "Assets/ResourcesEx/" + BundleBelong.animation;
    public static string prefabPath = "Assets/ResourcesEx/" + BundleBelong.prefab;
    public static string matPath = "Assets/ResourcesEx/" + BundleBelong.material;

    [MenuItem("Custom/Map/加载地图资源")]
    static void LoadMapSprites()
    {
        foreach(string directory in System.IO.Directory.GetDirectories(mapRoot))
        {
            string dir = directory.Replace("\\", "/");
            string[] _path = dir.Split('/');
            string prefabname = _path[_path.Length - 1];
            string tag = string.Empty;// prefabname + "@map";

            List<Sprite> mapLst = new List<Sprite>();
            foreach(string file in System.IO.Directory.GetFiles(dir))
            {
                string fileDir = file.Replace("\\", "/");
                if(fileDir.EndsWith(".png") || fileDir.EndsWith(".jpg"))
                {
                    mapLst.Add(AssetDatabase.LoadAssetAtPath<Sprite>(fileDir));
                    SpriteImporter(fileDir, tag, false);
                }
            }
            //创建地图预置 
            GameObject go = new GameObject(prefabname);
            string objPath ="Assets/ResourcesEx/" + BundleBelong.map + "/" + prefabname + ".prefab";
            GameObject prefab = PrefabUtility.CreatePrefab(objPath, go);
            MapCfg cfg = prefab.AddComponent<MapCfg>();
            cfg.containsSprites.Clear();
            cfg.containsSprites.AddRange(mapLst);
            SetAssetBundleName(objPath, BundleBelong.map);
            DestroyImmediate(go);
        }
    }
    [MenuItem("Custom/Fx/重新打包特效")]
    static void RePackFxSprites()
    {
        PackFxSprites(true);
    }
    [MenuItem("Custom/Fx/更新打包特效")]
    static void UpdatePackFxSprites()
    {
        PackFxSprites(true);
    }

    //打包特效图集
    static void PackFxSprites(bool reBuild)
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach (string directory in System.IO.Directory.GetDirectories(fxRoot))
        {
            string dir = directory.Replace("\\", "/");
            string[] _path = dir.Split('/');
            string filename = _path[_path.Length - 1];
            string animatorNormalPath = filename + "@fx";

            string controllerPath = animatorControllerPath + "/" + animatorNormalPath + ".controller";
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            SetAssetBundleName(controllerPath, BundleBelong.animation);

            AnimatorControllerLayer layer = controller.layers[0];

            foreach (string file in System.IO.Directory.GetFiles(dir))
            {
                string fileDir = file.Replace("\\", "/");
                if (fileDir.EndsWith(".png") || fileDir.EndsWith(".jpg"))
                {
                    sprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(fileDir));
                    SpriteImporter(fileDir, animatorNormalPath, true);
                }
            }
            string clipPath = dir + "/" + animatorNormalPath + ".anim";
            BuildAnimctionClip(sprites.ToArray(), clipPath);
            AddStateTransition(clipPath, layer, animatorNormalPath);
        }
    }

    //===============

    static Dictionary<string,string> path2ClipDic_Normal = new Dictionary<string, string>
    {
        {"站立/shang","idle_shang"},{"站立/xia","idle_xia"},{"站立/you","idle_you"},{"站立/youshang","idle_youshang"},
        {"站立/youxia","idle_youxia"},{"站立/zuo","idle_zuo"},{"站立/zuoshang","idle_zuoshang"},{"站立/zuoxia","idle_zuoxia"},
        {"行走/shang","run_shang"},{"行走/xia","run_xia"},{"行走/you","run_you"},{"行走/youshang","run_youshang"},
        {"行走/youxia","run_youxia"},{"行走/zuo","run_zuo"},{"行走/zuoshang","run_zuoshang"},{"行走/zuoxia","run_zuoxia"},

        {"采集/shang","collect_shang"},{"采集/xia","collect_xia"},{"采集/you","collect_you"},{"采集/youshang","collect_youshang"},
        {"采集/youxia","collect_youxia"},{"采集/zuo","collect_zuo"},{"采集/zuoshang","collect_zuoshang"},{"采集/zuoxia","collect_zuoxia"},
        {"采矿/shang","mine_shang"},{"采矿/xia","mine_xia"},{"采矿/you","mine_you"},{"采矿/youshang","mine_youshang"},
        {"采矿/youxia","mine_youxia"},{"采矿/zuo","mine_zuo"},{"采矿/zuoshang","mine_zuoshang"},{"采矿/zuoxia","mine_zuoxia"},
    };
    static Dictionary<string,string> path2ClipDic_Battle = new Dictionary<string, string>
    {
        {"战斗待机","idle"},{"奔跑","run"},{"受击","hurt"},{"闪避","miss"},{"死亡","dead"},
        {"被击浮空上升","floatingUp"},{"被击浮空下落","floatingDown"},{"被击浮空循环","floatingTop"},
        {"被击倒地中","hitFall"},{"被击倒地循环","falling"},{"倒地","fall"},{"起立","getup"},
        {"攻击/1","attack1"},{"攻击/2","attack2"},{"攻击/3","attack3"},
        {"技能/1","skill1"},{"技能/2","skill2"},{"技能/3","skill3"}
    };

    static Dictionary<string,string> path2ClipDic_Battle_3D = new Dictionary<string, string>
    {
        {"Idle","idle"},{"Run","run"},{"受击","hurt"},{"闪避","miss"},{"Dead","dead"},
        {"被击浮空上升","floatingUp"},{"被击浮空下落","floatingDown"},{"被击浮空循环","floatingTop"},
        {"被击倒地中","hitFall"},{"被击倒地循环","falling"},{"倒地","fall"},{"起立","getup"},
        {"AttackA","attack1"},{"AttackB","attack2"},{"攻击/3","attack3"},
        {"SkillA","skill1"},{"SkillB","skill2"},{"SkillC","skill3"},{"SplashAni","skill4"},{"SkillD","skill5"},
        {"SpecialIdle","specialIdle"},{"Win","win"}
    };

    static bool IsLoop(string path)
    {
        if(path.IndexOf("idle") >= 0 ||
            path.IndexOf("run") >= 0 ||
            path.IndexOf("fall") >= 0 ||
            path.IndexOf("falling") >= 0 ||
            path.IndexOf("floatingTop") >= 0)
        {
            return true;
        }
        return false;
    }

    [MenuItem("Custom/2DAnimator/设置图片Tag")]
    static void BuildAtlasTag()
    {
        foreach(string directory in System.IO.Directory.GetDirectories(pathRoot))
        {
            string dir = directory.Replace("\\", "/");
            string[] _path = dir.Split('/');
            //path = _path[_path.Length - 1];
            string animatorNormalPath = _path[_path.Length - 1] + "@normal";
            string animatorBattlePath = _path[_path.Length - 1] + "@battle";

            //Debug.Log(dir + " " + animatorNormalPath + " " + animatorBattlePath);
            foreach(KeyValuePair<string,string > kv in path2ClipDic_Normal)
            {
                string spritePath = dir + "/" + kv.Key;

                if(!System.IO.Directory.Exists(spritePath))
                    continue;

                ChangeSpritesTag(spritePath, animatorNormalPath);
            }
            foreach(KeyValuePair<string,string > kv in path2ClipDic_Battle)
            {
                string spritePath = dir + "/" + kv.Key;

                if(!System.IO.Directory.Exists(spritePath))
                    continue;

                ChangeSpritesTag(spritePath, animatorBattlePath);
            }
        }
    }

    [MenuItem("Custom/2DAnimator/重新打包动画")]
    static void ReBuild2DAnimator()
    {
        Build2DAnimator(true);
    }
    [MenuItem("Custom/2DAnimator/更新打包动画")]
    static void UpdateBuild2DAnimator()
    {
        Build2DAnimator(false);
    }

    static void Build2DAnimator(bool reBuild)
    {
        List<Sprite> sprites = new List<Sprite>();
        List<string> clips = new List<string>();
        foreach(string directory in System.IO.Directory.GetDirectories(pathRoot))
        {
            clips.Clear();
            string dir = directory.Replace("\\", "/");
            string[] _path = dir.Split('/');
            string filename = _path[_path.Length - 1];
            string animatorNormalPath = _path[_path.Length - 1] + "@normal";
            string animatorBattlePath = _path[_path.Length - 1] + "@battle";

            string controllerPath = animatorControllerPath + "/" + animatorNormalPath + ".controller";
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            SetAssetBundleName(controllerPath, BundleBelong.animation);

            AnimatorControllerLayer layer = controller.layers[0];
            foreach(KeyValuePair<string,string > kv in path2ClipDic_Normal)
            {
                string spritePath = dir + "/" + kv.Key;

                if(!System.IO.Directory.Exists(spritePath))
                    continue;

                string clipPath = spritePath + "/" + filename + "@" + kv.Value + ".anim";
                if(!reBuild && System.IO.File.Exists(clipPath))
                {
                    clips.Add(kv.Value);
                    continue;
                }
                GetAllSpritesByPath(ref sprites, spritePath);
                BuildAnimctionClip(sprites.ToArray(), clipPath);
                AddStateTransition(clipPath, layer, kv.Value);
                clips.Add(kv.Value);
            }

            controllerPath = animatorControllerPath + "/" + animatorBattlePath + ".controller";
            AnimatorController controller2 = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            SetAssetBundleName(controllerPath, BundleBelong.animation);

            AnimatorControllerLayer layer2 = controller2.layers[0];
            foreach(KeyValuePair<string,string > kv in path2ClipDic_Battle)
            {
                string spritePath = dir + "/" + kv.Key;

                if(!System.IO.Directory.Exists(spritePath))
                    continue;

                string clipPath = spritePath + "/" + filename + "@" + kv.Value + ".anim";
                if(!reBuild && System.IO.File.Exists(clipPath))
                {
                    clips.Add(kv.Value);
                    continue;
                }
                GetAllSpritesByPath(ref sprites, spritePath);
                BuildAnimctionClip(sprites.ToArray(), clipPath);
                AddStateTransition(clipPath, layer2, kv.Value);
                clips.Add(kv.Value);
            }
            CreatePrefab(prefabPath, filename, (sprites != null && sprites.Count > 0) ? sprites[0] : null, clips, controller);
        }
    }

    [MenuItem("Custom/3DAnimator/创建动画")]
    static void Build3DAniamtor()
    {
        List<string> clips = new List<string>();
        foreach(string directory in System.IO.Directory.GetDirectories(pathRoot))
        {
            clips.Clear();
            string dir = directory.Replace("\\", "/");

            string[] _path = dir.Split('/');
            string filename = _path[_path.Length - 1];
            string animatorBattlePath = _path[_path.Length - 1] + "@battle";

            string controllerPath = animatorControllerPath + "/" + animatorBattlePath + ".controller";
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            SetAssetBundleName(controllerPath, BundleBelong.animation);

            AnimatorControllerLayer layer = controller.layers[0];
            controller.AddParameter("animvalue", AnimatorControllerParameterType.Int);
            foreach(KeyValuePair<string,string > kv in path2ClipDic_Battle_3D)
            {
                string spritePath = dir + "/Dota_Role_" + filename + "_" + kv.Key + ".fbx";
                if(!System.IO.File.Exists(spritePath))
                    continue;
                AddStateTransition(spritePath, layer, kv.Value, "animvalue");
                clips.Add(kv.Value);
            }
            string animPath = dir + "/Dota_Role_" + filename + ".fbx";
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(animPath);
            CreatePrefab3D(prefabPath, filename, go, clips, controller);
        }
    }

    static void SetAssetBundleName(string filepath, string bundlename)
    {
        AssetImporter ai = AssetImporter.GetAtPath(filepath);
        ai.assetBundleName = bundlename;
    }

    static void AddStateTransition(string clipPath, AnimatorControllerLayer layer, string clipname, string param = "")
    {
        AnimatorStateMachine sm = layer.stateMachine;
        //根据动画文件读取它的AnimationClip对象
        AnimationClip newClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

        //取出动画名子 添加到state里面
        AnimatorState state = sm.AddState(string.IsNullOrEmpty(clipname) ? newClip.name.Split('@')[1] : clipname);
        state.motion = newClip;

        if (!string.IsNullOrEmpty(param))
        {
            AnimatorTransition trans = sm.AddEntryTransition(state);
            trans.AddCondition(AnimatorConditionMode.Equals, SkillManager.animTransIdDic[clipname], param);

            AnimatorStateTransition trans_exit = state.AddExitTransition(false);
            trans_exit.duration = 0.02f;
            trans_exit.AddCondition(AnimatorConditionMode.NotEqual, SkillManager.animTransIdDic[clipname], param);
        }
    }
    static void BuildAnimctionClip(Sprite[] sprites, string path)
    {
        AnimationClip clip = new AnimationClip();
        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        //改变游戏物体的路径（空字符串表示根部）
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Length];
        for(int i = 0; i < sprites.Length; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe();
            keyFrames[i].time = 0.1f * i;
            keyFrames[i].value = sprites[i];
        }

        //有些动画我希望天生它就动画循环
        if(IsLoop(path))
        {
            //设置动画文件为循环动画
            SerializedObject serializedClip = new SerializedObject(clip);
            AnimationClipSettings clipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
            clipSettings.loopTime = true;
            serializedClip.ApplyModifiedProperties();
        }

        clip.frameRate = 30;
        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
        AssetDatabase.CreateAsset(clip, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 从一张大图集里面获取切过之后小sprites
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    static void GetSpritesFromOne(ref List<Sprite> list, int index)
    {
        list.Clear();
        string _path = pathRoot + "/" + index + ".png";
        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(_path);

        for(int i = 1; i < objs.Length; i++)
        {
            list.Add((Sprite)objs[i]);
        }
    }

    /// <summary>
    /// 获取多个小的sprites
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    static void GetSprites(ref List<Sprite> list, int index)
    {
        list.Clear();
        int _index = index;
        Sprite spr = null;
        while(true)
        {
            string _path = pathRoot + "/" + _index++ + ".png";
            spr = AssetDatabase.LoadAssetAtPath<Sprite>(_path);
            if(spr != null)
            {
                list.Add(spr);
            }
            else
            {
                break;
            }
        }
    }
    static void ChangeSpritesTag(string path, string tag)
    {
        foreach (string directory in System.IO.Directory.GetFiles(path))
        {
            string fileDir = directory.Replace("\\", "/");
            if (fileDir.EndsWith(".png") || fileDir.EndsWith(".jpg"))
            {
                SpriteImporter(fileDir, tag);
            }
        }
    }

    static void SpriteImporter(string fileDir, string tag, bool splitAlpha = true)
    {
        TextureImporter importer = AssetImporter.GetAtPath(fileDir) as TextureImporter;
        if(!string.IsNullOrEmpty(tag) && importer.spritePackingTag == tag)
            return;

        importer.spritePackingTag = tag;

        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
        settings.maxTextureSize = 1024;
        settings.textureCompression = TextureImporterCompression.Compressed;
        settings.crunchedCompression = true;
        importer.SetPlatformTextureSettings(settings);

        settings.overridden = true;
        settings.name = "Android";
        settings.format = TextureImporterFormat.ETC_RGB4;
        settings.allowsAlphaSplitting = splitAlpha;
        importer.SetPlatformTextureSettings(settings);

        settings.overridden = true;
        settings.name = "iPhone";
        settings.format = TextureImporterFormat.PVRTC_RGBA4;
        importer.SetPlatformTextureSettings(settings);

        AssetDatabase.ImportAsset(fileDir, ImportAssetOptions.ForceUpdate);
    }

    static void GetAllSpritesByPath(ref List<Sprite> list, string path)
    {
        list.Clear();
        Sprite spr = null;
        foreach(string directory in System.IO.Directory.GetFiles(path))
        {
            string dir = directory.Replace("\\", "/");
            if (dir.EndsWith(".png") || dir.EndsWith(".jpg"))
            {
                spr = AssetDatabase.LoadAssetAtPath<Sprite>(dir);
                list.Add(spr);
            }
        }
    }
    static void CreatePrefab(string prefabPath, string prefabName, Sprite _sprite, List<string> clips, RuntimeAnimatorController animatorController)
    {
        GameObject go = new GameObject();
        go.name = prefabName;
        SpriteRenderer spriteRender = go.AddComponent<SpriteRenderer>();
        spriteRender.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath + "/mat_role.mat");
        //spriteRender.sprite = _sprite;
        Animator animator = go.GetComponent<Animator>();
        if(animator == null)
        {
            animator = go.AddComponent<Animator>();
        }
        //animator.runtimeAnimatorController = animatorController;
        string objpath = prefabPath + "/" + prefabName + ".prefab";
        GameObject prefab = PrefabUtility.CreatePrefab(objpath, go);
        AnimatorCfg cfg = prefab.AddComponent<AnimatorCfg>();
        cfg.containsClips.Clear();
        cfg.containsClips.AddRange(clips);

        SetAssetBundleName(objpath, BundleBelong.prefab);
        DestroyImmediate(go);
    }
    static void CreatePrefab3D(string prefabPath, string prefabName, GameObject go, List<string> clips, RuntimeAnimatorController animatorController)
    {
        if(go == null) return;

        go.name = prefabName;
        Animator animator = go.GetComponent<Animator>();
        if(animator == null)
        {
            animator = go.AddComponent<Animator>();
        }
        //animator.runtimeAnimatorController = animatorController;
        string objpath = prefabPath + "/" + prefabName + ".prefab";
        GameObject prefab = PrefabUtility.CreatePrefab(objpath, go);
        AnimatorCfg cfg = prefab.AddComponent<AnimatorCfg>();
        cfg.containsClips.Clear();
        cfg.containsClips.AddRange(clips);
        SetAssetBundleName(objpath, BundleBelong.prefab);
    }
}

class AnimationClipSettings
{
    SerializedProperty m_Property;

    private SerializedProperty Get(string property) { return m_Property.FindPropertyRelative(property); }

    public AnimationClipSettings(SerializedProperty prop) { m_Property = prop; }

    public float startTime { get { return Get("m_StartTime").floatValue; } set { Get("m_StartTime").floatValue = value; } }
    public float stopTime { get { return Get("m_StopTime").floatValue; } set { Get("m_StopTime").floatValue = value; } }
    public float orientationOffsetY { get { return Get("m_OrientationOffsetY").floatValue; } set { Get("m_OrientationOffsetY").floatValue = value; } }
    public float level { get { return Get("m_Level").floatValue; } set { Get("m_Level").floatValue = value; } }
    public float cycleOffset { get { return Get("m_CycleOffset").floatValue; } set { Get("m_CycleOffset").floatValue = value; } }

    public bool loopTime { get { return Get("m_LoopTime").boolValue; } set { Get("m_LoopTime").boolValue = value; } }
    public bool loopBlend { get { return Get("m_LoopBlend").boolValue; } set { Get("m_LoopBlend").boolValue = value; } }
    public bool loopBlendOrientation { get { return Get("m_LoopBlendOrientation").boolValue; } set { Get("m_LoopBlendOrientation").boolValue = value; } }
    public bool loopBlendPositionY { get { return Get("m_LoopBlendPositionY").boolValue; } set { Get("m_LoopBlendPositionY").boolValue = value; } }
    public bool loopBlendPositionXZ { get { return Get("m_LoopBlendPositionXZ").boolValue; } set { Get("m_LoopBlendPositionXZ").boolValue = value; } }
    public bool keepOriginalOrientation { get { return Get("m_KeepOriginalOrientation").boolValue; } set { Get("m_KeepOriginalOrientation").boolValue = value; } }
    public bool keepOriginalPositionY { get { return Get("m_KeepOriginalPositionY").boolValue; } set { Get("m_KeepOriginalPositionY").boolValue = value; } }
    public bool keepOriginalPositionXZ { get { return Get("m_KeepOriginalPositionXZ").boolValue; } set { Get("m_KeepOriginalPositionXZ").boolValue = value; } }
    public bool heightFromFeet { get { return Get("m_HeightFromFeet").boolValue; } set { Get("m_HeightFromFeet").boolValue = value; } }
    public bool mirror { get { return Get("m_Mirror").boolValue; } set { Get("m_Mirror").boolValue = value; } }
}
#endif