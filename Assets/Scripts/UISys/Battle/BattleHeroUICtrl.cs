using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleHeroUICtrl : BaseUICtrl
{
    public GameObject heroUIRoot;
    public Image icon;
    public Image mask;
    public GridLayoutHelper gridHelper;

    public int actor_id;

    bool m_IsCanUse = true;
    public bool IsCanUse
    {
        get
        {
            return m_IsCanUse;
        }
        set
        {
            if(m_IsCanUse != value)
            {
                m_IsCanUse = value;
                mask.gameObject.SetActive(m_IsCanUse);
            }
        }
    }

    public Action<int> pressAction;

    void Awake()
    {
        UIEventListener.Get(heroUIRoot).onClick = delegate(GameObject go)
        {
            if(IsCanUse && pressAction != null)
            {
                pressAction(actor_id);
            }
        };
    }
}
