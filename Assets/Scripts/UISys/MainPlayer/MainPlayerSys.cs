using UnityEngine;
using System.Collections;

/// Author:Lee
/// Date:2017-3-14
/// Func:主角Control层

public class MainPlayerSys : GameOverlapSys<MainPlayerSys>
{
    private string prefab = "UIPlayerPage";
    public MainPlayerUICtrl CtrlUI { get; private set; }

    #region 初始化函数Func

    public override void SysEnter()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<MainPlayerUICtrl>(prefab);
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
            UIEventListener.Get(CtrlUI.HeroInfoBtn.gameObject).onClick = OnButtonClick;
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
            CGameRoot.CloseOverlapState(SystemEnum.Heros);
            return;
        }
        else if (go == CtrlUI.HeroInfoBtn.gameObject)
        {
            //打开英雄详细信息界面
            CGameRoot.SwitchOverLapState(SystemEnum.HeroDetail);
        }
    }

    #endregion
}
