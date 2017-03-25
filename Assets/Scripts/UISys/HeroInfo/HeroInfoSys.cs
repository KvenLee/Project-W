using UnityEngine;
using System.Collections;

/// Author:Lee
/// Date:2017-3-20
/// Func:英雄详细信息Control层

public class HeroInfoSys : GameOverlapSys<HeroInfoSys>
{
    private string prefab = "UIHeroInfoPage";
    public HeroInfoUICtrl CtrlUI { get; private set; }

    #region 初始化函数Func

    public override void SysEnter()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<HeroInfoUICtrl>(prefab);
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
            UIEventListener.Get(CtrlUI.TrainHeroBtn.gameObject).onClick = OnButtonClick;      
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
            CGameRoot.SwitchOverLapState(SystemEnum.Heros);
            return;
        }
        else if (go == CtrlUI.TrainHeroBtn.gameObject)
        {
            //打开培养面板
            CGameRoot.SwitchOverLapState(SystemEnum.HeroUp);
        }
    }

    #endregion
}
