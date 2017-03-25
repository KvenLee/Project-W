using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敏感字
/// </summary>
/// 
public class SensitiveWordHelper
{
    private SensitiveWordHelper() { }

    private static SensitiveWordHelper m_Instance;
    public static SensitiveWordHelper Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SensitiveWordHelper();

            }
            return m_Instance;
        }
    }

    /// <summary>
    /// 敏感字库加载初始化
    /// </summary>
    public void SensitiveWordsLoad()
    {
        if (m_Instance == null)
        {
            m_Instance = new SensitiveWordHelper();
        }
        TextAsset ta = ObjectsManager.LoadObject<TextAsset>(BundleBelong.data, "SensitiveWord");
        if (ta != null)
        {
            string[] lst = ta.text.Split('\n');
            for (int i = 0, count = lst.Length; i < count; i++)
            {
                m_SensitiveWordsLst.Add(lst[0].Substring(0, lst[0].Length - 1));
            }
        }
    }

    //==========敏感字=============
    List<string> m_SensitiveWordsLst = new List<string>();
    /// <summary>
    /// 敏感字检查
    /// </summary>
    /// <returns></returns>
    public bool SensitiveWordsCheck(string content)
    {
        if (string.IsNullOrEmpty(content))
            return false;

        string lower_content = content.ToLower();
        int index = m_SensitiveWordsLst.FindIndex(delegate(string temp)
        {
            return lower_content.IndexOf(temp) >= 0;
        });
        return index >= 0;
    }
    public string SensitiveWordsPross(string content)
    {
        if (string.IsNullOrEmpty(content))
            return content;

        string lower_content = content.ToLower();
        for (int i = 0, count = m_SensitiveWordsLst.Count; i < count; i++)
        {
            if (string.IsNullOrEmpty(m_SensitiveWordsLst[i]))
                continue;

            if (lower_content.IndexOf(m_SensitiveWordsLst[i]) >= 0)
            {
                lower_content = lower_content.Replace(m_SensitiveWordsLst[i], ConvertStarWords(m_SensitiveWordsLst[i].Length));
                //break;
            }
        }
        return lower_content;
    }
    string ConvertStarWords(int count)
    {
        string ret = "";
        for (int i = 0; i < count; i++)
        {
            ret += "*";
        }
        return ret;
    }
}
