using UnityEngine;
using System.Collections;

public class EconomicSys : GameSys<EconomicSys>
{
    private string prefab = "UIEconomic";
    public EconomicUICtrl CtrlUI { get; private set; }

    public override void SysInit()
    {
        CGameRoot.stateLeaveCallback += StateLeave;
        CGameRoot.stateEnterCallback += StateEnter;
        base.SysInit();
    }

    void InitCtrl()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<EconomicUICtrl>(prefab);
        }
        CtrlUI.UIEnter();
    }

    void StateEnter()
    {
        switch (CGameRoot.m_CurState)
        {
            case SystemEnum.Main:
            case SystemEnum.Enroll:
                InitCtrl();
                break;
            default: break;
        }
    }

    void StateLeave()
    {
        if (CtrlUI != null)
        {
            CtrlUI.UILeave();
            CtrlUI = null;
        }
    }
}
