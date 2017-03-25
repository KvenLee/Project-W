using System.Text;
using System.IO;

public class CUility
{
    //====================Pb:Bytes<--->String
    public static string PbBytes2String(byte[] _bytes)
    {
        if(_bytes == null)
            return string.Empty;

        char[] array = Encoding.UTF8.GetChars(_bytes);
        int len = 0;
        for(int count = array.Length; len < count; len++)
        {
            if(array[len] == 0) break;
        }
        return new string(array, 0, len);
    }

    public static byte[] String2PbBytes(string pbStr)
    {
        if(string.IsNullOrEmpty(pbStr))
            return null;

        return Encoding.UTF8.GetBytes(pbStr);
    }

    /// <summary>
    /// 输入内容合法性检查
    /// </summary>
    /// <returns></returns>
    public bool InputValidityCheck(string content)
    {
        //非空 字符 中文 字母 数字
        string checker = "^[\xalpha-\xff5a\u4E00-\u9FA5A-Za-z0-9_]{1,14}$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(content, checker))
        {
            return false;
        }
        return true;
    }

    //=====================Serialize

    public static void Serialize<T>(T obj)
    {
        FileCustomHelper.CreateFolder(AssetPath.gameDataPath);
        using(Stream stream = new FileStream(string.Format(@"{0}\{1}.db", AssetPath.gameDataPath, typeof(T).Name), FileMode.OpenOrCreate))
        {
            ProtoBuf.Serializer.Serialize<T>(stream, obj);
        }
    }

    public static T DeSerialize<T>()
    {
        string filepath = string.Format(@"{0}\{1}.db", AssetPath.gameDataPath, typeof(T).Name);
        if(FileCustomHelper.CheckIfExist(filepath))
        {
            using(Stream stream = new FileStream(filepath, FileMode.Open))
            {
                T cs2 = ProtoBuf.Serializer.Deserialize<T>(stream);
                return cs2;
            }
        }
        else
        {
            UnityEngine.Debug.Log("文件不存在:" + filepath);
            return default(T);
        }
    }

    //=============UITextrue Load
    public static void LoadUITextrue(UnityEngine.UI.Image uiTex, string tex_name)
    {
        if (uiTex.sprite != null)
        {
            UnityEngine.Sprite tex = uiTex.sprite;
            ObjectsManager.RemoveUIObj(tex);
        }
        if (string.IsNullOrEmpty(tex_name))
        {
            uiTex.sprite = null;
        }
        else
        {
            UnityEngine.Sprite tex2 = ObjectsManager.LoadObject<UnityEngine.Sprite>(BundleBelong.ugui, tex_name);
            ObjectsManager.AddUIObj(tex2);
            uiTex.sprite = tex2;
        }
    }
    public static void UnloadUITextrue(UnityEngine.UI.Image uiTex)
    {
        if(uiTex.sprite != null)
        {
            UnityEngine.Sprite tex = uiTex.sprite;
            ObjectsManager.RemoveUIObj(tex);
            uiTex.sprite = null;
        }
    }

    public static void GlobalRelease()
    {
        UnityEngine.Resources.UnloadUnusedAssets();
        System.GC.Collect();
        UnityEngine.Resources.UnloadUnusedAssets();
    }
}
