using System.Collections;
using UnityEngine;

public class MessageTipSys : GameOverlapSys<MessageTipSys>
{
    private string prefab = "UIMessageTip";
    public MessageTipUICtrl CtrlUI { get; private set; }

    public override void SysEnter()
    {
        base.SysEnter();
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<MessageTipUICtrl>(prefab);
        }
    }

    public override void SysLeave()
    {
        if (CtrlUI != null)
        {
            CtrlUI.gameObject.SetActive(false);
            CtrlUI = null;
        }
        base.SysLeave();
    }

    public void ShowMessage(string content)
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<MessageTipUICtrl>(prefab);
        }
        CtrlUI.m_LabContent.text = content;
    }
}