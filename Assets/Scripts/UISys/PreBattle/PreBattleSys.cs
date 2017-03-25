using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PreBattleSys : GameSys<PreBattleSys>
{
    private string prefab = "UIPreBattle";
    public PreBattleUICtrl CtrlUI { get; private set; }

    public override void SysEnter()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<PreBattleUICtrl>(prefab);
        }

        if (CtrlUI != null)
        {
            AddListener(CtrlUI.m_BtnClose, BtnClose);
            AddListener(CtrlUI.m_BtnGo, BtnGoBattle);

            CtrlUI.m_PopulatorArms.BindDataSource(null);
            CtrlUI.m_PopulatorHeros.BindDataSource(null);
            CtrlUI.m_PreHeroScrollView.BindDataSource(null);
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

    private void BtnClose()
    {
        CGameRoot.SwitchState(CGameRoot.m_PreState);
    }
    public void BtnGoBattle()
    {
        MaskTransfer.Instance.OnShow(LoadScene);
        //         LoadingSys.Instance.Goto("Battle", delegate()
        //         {
        //             CGameRoot.SwitchState(SystemEnum.Battle);
        //         });
    }
    IEnumerator LoadScene()
    {
        yield return SceneManager.LoadSceneAsync("_empty");
        CGameRoot.SwitchState(SystemEnum.Battle);
        //ObjectsManager.CreateGameObject(BundleBelong.map, "GridMap", false);
    }
    public void BattleOver()
    {
        MaskTransfer.Instance.OnShow(Go);
    }
    IEnumerator Go()
    {
        yield return SceneManager.LoadSceneAsync("_empty");
        MainGamePageSys.Instance.EnterMain();
    }
}
