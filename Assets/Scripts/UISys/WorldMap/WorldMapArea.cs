using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author:Lee
/// Date:2017-3-8
/// Func:世界地图地点标记

public class WorldMapArea : MonoBehaviour
{
    public WorldArea AreaMark;

}

public enum WorldArea
{
    TianShan = 0,
    YaoWangGu = 1,
    LongMenHuangMo = 2,
    JiEDao = 3,
    ChiLianTang = 4,
    QingFengZhao = 5,
    MaiHuangJianZhongBtn = 6,
    HuWangShan = 7,
    DaJueSi = 8,
    BaiMaHuangCheng = 9,
    WanLiBo = 10,
    WanDuXia = 11,
    XveDaoMen = 12,
    FengHuoLianHuanWuBtn = 13,
    PengLaiGe = 14,
    GuSu = 15,
    WuYouShan = 16,
    GuanHaiLan = 17,
}