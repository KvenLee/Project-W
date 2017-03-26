using Res_Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnrollCell : BaseUICtrl
{
    public GameObject consume;
    public Image icon;
    public Text m_TxtName1;
    public Text m_TxtLevel1;

    public GameObject hero;
    public Text m_TxtName2;
    public Text m_TxtLevel2;

    public  void SetData(uint itemid,int count)
    {
        ResItemInfo itemInfo = TableMgr.Instance.GetRecord<ResItemInfo>(TableMgr.cIdxItem, itemid);
        if(itemInfo !=null)
        {
            icon.sprite = ObjectsManager.LoadObject<Sprite>(CUility.PbBytes2String(itemInfo.icon_bundle), CUility.PbBytes2String(itemInfo.icon));
            if( (ItemType ) itemInfo.type  == ItemType.E_Hero)
            {
                hero.SetActive(true);
                consume.SetActive(false);
                m_TxtName2.text = CUility.PbBytes2String(itemInfo.name);
                m_TxtLevel2.text = "1";
            }
            else
            {
                hero.SetActive(false);
                consume.SetActive(true);
                m_TxtName1.text = CUility.PbBytes2String(itemInfo.name);
                m_TxtLevel1.text = "1";
            }
        }
    }
}
