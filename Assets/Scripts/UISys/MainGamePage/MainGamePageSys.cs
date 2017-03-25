using UnityEngine;
using System.Collections;

/// Author:Lee
/// Date:2017-3-6
/// Func:主城界面Control层

public class MainGamePageSys : GameSys<MainGamePageSys>
{
    private string prefab = "UIMainGamePage";

    public MainGamePageUICtrl CtrlUI { get; private set; }

    #region 初始化函数Func

    public override void SysEnter()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<MainGamePageUICtrl>(prefab);
        }
        base.SysEnter();
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

    public override void BindFunc()
    {
        if (CtrlUI != null)
        {
            UIEventListener.Get(CtrlUI.ImproveVipBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.PlayerModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.TeamModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.BagModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.TaskModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.RecruitModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.WorldModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.CalendarModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.ChestModuleBtn.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(CtrlUI.MapModuleBtn.gameObject).onClick = OnButtonClick;
        }
    }
    #endregion

    #region 界面处理Func

    private void OnButtonClick(GameObject go)
    {
        if (go == CtrlUI.ImproveVipBtn.gameObject)
        {
            //提升VIP等级
        }
        else if (go == CtrlUI.PlayerModuleBtn.gameObject)
        {
            //主角模块
            CGameRoot.OverlapState(SystemEnum.Heros);
        }
        else if (go == CtrlUI.TeamModuleBtn.gameObject)
        {
            //阵型模块
        }
        else if (go == CtrlUI.BagModuleBtn.gameObject)
        {
            //背包模块
            CGameRoot.OverlapState(SystemEnum.Bag);
        }
        else if (go == CtrlUI.TaskModuleBtn.gameObject)
        {
            //任务模块
            CGameRoot.OverlapState(SystemEnum.Task);
        }
        else if (go == CtrlUI.RecruitModuleBtn.gameObject)
        {
            //招募模块
            CGameRoot.SwitchState(SystemEnum.Enroll);
        }
        else if (go == CtrlUI.WorldModuleBtn.gameObject)
        {
            //世界地图模块           
            CGameRoot.OverlapState(SystemEnum.WorldMap);
        }
        else if (go == CtrlUI.CalendarModuleBtn.gameObject)
        {
            //日历签到模块
        }
        else if (go == CtrlUI.ChestModuleBtn.gameObject)
        {
            //宝箱模块
        }
        else if (go == CtrlUI.MapModuleBtn.gameObject)
        {
            //地图模块
        }
    }

    #endregion

    public void EnterMain()
    {
        ObjectsManager.CreateGameObject(BundleBelong.map, "GridMap", false);
        CGameRoot.SwitchState(SystemEnum.Main);
    }
}
