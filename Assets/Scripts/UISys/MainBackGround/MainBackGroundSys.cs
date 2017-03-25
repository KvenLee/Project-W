using UnityEngine;
using System.Collections;

public class MainBackGroundSys : GameSys<MainBackGroundSys>
{
    private string prefab = "UIMainBg";
    public MainBackGroundUICtrl CtrlUI { get; private set; }

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
            CtrlUI = UIManager.Instance.GetUI<MainBackGroundUICtrl>(prefab);
        }
        CtrlUI.m_TexBG.enabled = true;
    }
    void StateLeave()
    {
        if (CtrlUI != null)
        {
            CUility.UnloadUITextrue(CtrlUI.m_TexBG);
            CtrlUI.m_TexBG.enabled = false;
        }
    }

    void StateEnter()
    {
        if (CGameRoot.m_CurState == SystemEnum.ResourceUpdate||
            CGameRoot.m_CurState == SystemEnum.Battle)
            return;

        string _name = string.Empty;
        switch (CGameRoot.m_CurState)
        {
            case SystemEnum.Enroll:
                _name = "dabeijing-1";
                break;
            default:
                //_name = "dabeijing-1";
                break;
        }
        if (!string.IsNullOrEmpty(_name))
        {
            InitCtrl();
            CUility.LoadUITextrue(CtrlUI.m_TexBG, _name);
        }
    }
}
