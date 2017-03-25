using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色动画配置参考表
/// </summary>
/// 
public class AnimatorCfg : MonoBehaviour
{
    public List<string> containsClips = new List<string>();

    public bool IsContainsClip(string clipname)
    {
        if(string.IsNullOrEmpty(clipname))
            return false;
        return containsClips.Contains(clipname);
    }
}
