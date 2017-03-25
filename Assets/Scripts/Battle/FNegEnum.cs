using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 异常状态--角色异常状态(多选)
/// </summary>
/// 
//[System.Flags]
public enum NegStateEnum
{
    /// <summary>
    /// 正常状态
    /// </summary>
    Normal = 0,
    /// <summary>
    /// 定身（被定身的目标禁止移动)
    /// </summary>
    Immobilized = 1,
    /// <summary>
    /// 点穴(被点穴的目标禁止使用能量)
    /// </summary>
    Acupuncture = 2,
}

public class FNegEnum
{
    /// <summary>
    /// 添加枚举元素
    /// </summary>
    /// <param name="w"> 原始变量 </param>
    /// <param name="_w"> 要增加的类型</param>
    public static void Add(ref NegStateEnum w, NegStateEnum _w)
    {
        w = w | _w;
    }

    /// <summary>
    /// 添加枚举元素
    /// </summary>
    /// <param name="w"> 原始变量 </param>
    /// <param name="_wArray"> 要增加的类型</param>
    public static void Add(ref NegStateEnum w, NegStateEnum[] _wArray)
    {
        foreach (NegStateEnum _w in _wArray)
        {
            w = w | _w;
        }
    }

    /// <summary>
    /// 移除枚举元素
    /// </summary>
    /// <param name="w"> 原始变量 </param>
    /// <param name="_wArray"> 要增加的类型</param>
    public static void Remove(ref NegStateEnum w, NegStateEnum _w)
    {
        w = w & ~_w;
    }
    /// <summary>
    /// 移除枚举元素
    /// </summary>
    /// <param name="w"> 原始变量 </param>
    /// <param name="_wArray"> 要增加的类型</param>
    public static void Remove(ref NegStateEnum w, NegStateEnum[] _wArray)
    {
        foreach (NegStateEnum _w in _wArray)
        {
            w = w & ~_w;
        }
    }

    /// <summary>
    /// 是否包含
    /// </summary>
    /// <param name="w"> 原始变量 </param>
    /// <param name="_w"> 要增加的类型</param>
    public static bool IsContain(NegStateEnum w, NegStateEnum _w)
    {
        return 0 != (w & _w);
    }

    /// <summary>
    /// 是否为None
    /// </summary>
    public static bool IsNone(NegStateEnum w)
    {
        return w != NegStateEnum.Normal;
    }    
}

// 异常状态enum测试
//     void NegStateTest()
//     {
//         RoleNegState weekDay = RoleNegState.Normal;
//         FNegEnum.Add(ref weekDay, new RoleNegState[] { RoleNegState.Immobilized, RoleNegState.Acupuncture });
//         FNegEnum.Remove(ref weekDay, new RoleNegState[] { RoleNegState.Acupuncture });
//         bool hasMonday = FNegEnum.IsContain(weekDay, RoleNegState.Immobilized);
//     }
