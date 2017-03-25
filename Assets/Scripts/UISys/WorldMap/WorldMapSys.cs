using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// Author:Lee
/// Date:2017-3-8
/// Func:世界地图Control层

public class WorldMapSys : GameOverlapSys<WorldMapSys>
{
    private string prefab = "UIWorldMapPage";
    public WorldMapUICtrl CtrlUI { get; private set; }

    #region 初始化函数Func

    public override void SysEnter()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<WorldMapUICtrl>(prefab);
            CtrlUI.UIEnter();
        }
        base.SysEnter();
    }

    public override void SysLeave()
    {
        if (CtrlUI != null)
        {
            CtrlUI.UILeave();
            CtrlUI = null;
        }
        base.SysLeave();
    }

    public override void BindFunc()
    {
        if (CtrlUI != null)
        {
            UIEventListener.Get(CtrlUI.m_BtnClose.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.TianShanBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.YaoWangGuBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.LongMenHuangMoBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.JiEDaoBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.ChiLianTangBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.QingFengZhaoBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.MaiHuangJianZhongBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.HuWangShanBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.DaJueSiBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.BaiMaHuangChengBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.WanLiBoBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.WanDuXiaBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.XveDaoMenBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.FengHuoLianHuanWuBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.PengLaiGeBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.GuSuBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.WuYouShanBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.GuanHaiLanBtn.gameObject).onClick = OnButtonClick;
        }
    }
    #endregion

    #region 界面处理Func

    private void OnButtonClick(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        else if (go == CtrlUI.m_BtnClose.gameObject)
        {
            //关闭界面
            CGameRoot.CloseOverlapState(SystemEnum.WorldMap);
            return;
        }

        WorldArea area = go.GetComponent<WorldMapArea>().AreaMark;
        switch (area)
        {
            case WorldArea.TianShan:
            case WorldArea.YaoWangGu:
            case WorldArea.LongMenHuangMo:
            case WorldArea.JiEDao:
            case WorldArea.ChiLianTang:
            case WorldArea.QingFengZhao:
                PreBattleSys.Instance.BtnGoBattle();
                break;
            case WorldArea.MaiHuangJianZhongBtn:
            case WorldArea.HuWangShan:
            case WorldArea.DaJueSi:
            case WorldArea.BaiMaHuangCheng:
            case WorldArea.WanLiBo:
            case WorldArea.WanDuXia:
            case WorldArea.XveDaoMen:
            case WorldArea.FengHuoLianHuanWuBtn:
            case WorldArea.PengLaiGe:
            case WorldArea.GuSu:
            case WorldArea.WuYouShan:
            case WorldArea.GuanHaiLan:

                break;
        }
    }

    #endregion
}
