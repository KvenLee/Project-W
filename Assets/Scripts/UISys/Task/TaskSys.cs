using UnityEngine;
using System.Collections;

/// Author:Lee
/// Date:2017-3-11
/// Func:任务界面Control层

public class TaskSys : GameOverlapSys<TaskSys>
{
    private string prefab = "UITaskPage";
    public TaskUICtrl CtrlUI { get; private set; }

    #region 初始化函数Func

    public override void SysEnter()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<TaskUICtrl>(prefab);
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
            CGameRoot.CloseOverlapState(SystemEnum.Task);
            return;
        }
    }

    #endregion
}
