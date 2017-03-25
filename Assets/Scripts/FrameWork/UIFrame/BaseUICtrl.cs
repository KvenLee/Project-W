using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaseUICtrl : MonoBehaviour
{
    public Image[] m_UnloadTex;
    public Button m_BtnClose;

    UITweenHandler[] handler;

    public virtual void OnEnable()
    {
        if (handler == null)
        {
            handler = transform.GetComponentsInChildren<UITweenHandler>(true);
        }
    }

    public virtual void OnDestroy()
    {
        foreach(Image t in m_UnloadTex)
        {
            if(t != null)
            {
                ObjectsManager.UnLoadDirectly(t.sprite);
            }
        }
    }

    public virtual void UIEnter()
    {
        if(handler == null)
        {
            gameObject.SetActive(true);
            return;
        }
        foreach (UITweenHandler tw in handler)
        {
            tw.Play(true);
        }
    }
    public virtual void UILeave()
    {
        if(handler.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        foreach (UITweenHandler tw in handler)
        {
            tw.Play(false);
        }
    }
}
