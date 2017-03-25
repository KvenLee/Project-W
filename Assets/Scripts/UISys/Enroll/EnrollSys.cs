using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnrollCoinType
{
    Nil = 0,
    Coin,
    Gold,
}
public enum EnrollTimeType
{
    Nil = 0,
    One,
    Ten,
}
public class EnrollSys : GameSys<EnrollSys>
{
    private string prefab = "UIEnroll";
    public EnrollUICtrl CtrlUI
    {
        get;
        private set;
    }

    /// <summary>
    /// 抽卡消耗类型
    /// -1默认
    /// 1金币
    /// 2钻石
    /// </summary>
    public EnrollCoinType enrollCoinType = EnrollCoinType.Nil;

    /// <summary>
    /// 抽卡次数类型
    /// 1:一次
    /// 2：十次
    /// </summary>
    public EnrollTimeType enrollTimeType = EnrollTimeType.Nil;

    public override void SysEnter()
    {
        if (CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<EnrollUICtrl>(prefab);
            AddListener(CtrlUI.m_BtnClose, BtnBack);
            AddListener(CtrlUI.m_BtnCoinOne, delegate()
            {
                enrollCoinType = EnrollCoinType.Coin;
                enrollTimeType = EnrollTimeType.One;
                Enroll();
            });
            AddListener(CtrlUI.m_BtnCoinTen, delegate()
            {
                enrollCoinType = EnrollCoinType.Coin;
                enrollTimeType = EnrollTimeType.Ten;
                Enroll();
            });
            AddListener(CtrlUI.m_BtnGoldOne, delegate()
            {
                enrollCoinType = EnrollCoinType.Gold;
                enrollTimeType = EnrollTimeType.One;
                Enroll();
            });
            AddListener(CtrlUI.m_BtnGoldTen, delegate()
            {
                enrollCoinType = EnrollCoinType.Gold;
                enrollTimeType = EnrollTimeType.Ten;
                Enroll();
            });
            CtrlUI.m_Objmain.SetActive(true);
            CtrlUI.m_Objoneten.SetActive(false);
            CtrlUI.UIEnter();
        }
        base.SysEnter();
    }

    public override void SysLeave()
    {
        enrollTimeType = EnrollTimeType.Nil;
        enrollCoinType = EnrollCoinType.Nil;
        if (CtrlUI != null)
        {
            CtrlUI.UILeave();
            //CtrlUI.gameObject.SetActive(false);
            CtrlUI = null;
        }
        base.SysLeave();
    }

    private void BtnBack()
    {
        if (enrollCoinType != EnrollCoinType.Nil)
        {
            CtrlUI.m_Objmain.SetActive(true);
            CtrlUI.m_Objoneten.SetActive(false);
            enrollCoinType = EnrollCoinType.Nil;
        }
        else
        {
            CGameRoot.SwitchState(SystemEnum.Main);
        }
    }
    private void Enroll()
    {
        switch (enrollTimeType)
        {
            case EnrollTimeType.One:
                CtrlUI.m_Objmain.SetActive(false);
                CtrlUI.m_Objoneten.SetActive(true);
                CtrlUI.grid.BindDataSource(1, null);
                break;
            case EnrollTimeType.Ten:
                CtrlUI.m_Objmain.SetActive(false);
                CtrlUI.m_Objoneten.SetActive(true);
                CtrlUI.grid.BindDataSource(10, null);
                break;
        }
    }
}
