using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI系统中的Button事件系统
/// </summary>
public class UIEventSys : MonoBehaviour
{
    private List<Button> m_Buttons;

    public void AddListener(Button btn, UnityAction callback)
    {
        if(m_Buttons ==null)
        {
            m_Buttons = new List<Button>();
        }
        if(callback != null)
        {
            btn.onClick.AddListener(callback);
            m_Buttons.Add(btn);
        }
    }
    public void ClearListener()
    {
        if(m_Buttons == null)
            return;
        foreach(Button btn in m_Buttons)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}
