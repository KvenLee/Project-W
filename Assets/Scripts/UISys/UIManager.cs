using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : TUSingleton<UIManager>
{
    Dictionary<string, GameObject> m_UiDic = new Dictionary<string, GameObject>();
    
    public T GetUI<T>(string uiprefab) where T : BaseUICtrl
    {
        GameObject ui = null;
        if (m_UiDic.TryGetValue(uiprefab, out ui))
        {
            if (ui == null)
            {
                ui = LoadUI(uiprefab);
            }
        }
        else
        {
           ui = LoadUI(uiprefab);
        }
        if (ui != null)
        {
            ui.SetActive(true);
            return ui.GetComponent<T>();
        }
        return null;
    }

    GameObject LoadUI(string uiprefab)
    {
        GameObject ui =  ObjectsManager.CreateGameObject(BundleBelong.ui, uiprefab, false);
        if (ui != null)
        {
            RectTransform rt = ui.GetComponent<RectTransform>();
            if(rt !=null)
            {
                rt.localScale = Vector3.one;
                rt.localPosition = Vector3.zero;
                rt.localEulerAngles = Vector3.zero;
            }
            else
            {
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
            }
            m_UiDic[uiprefab] = ui;
        }
        return ui;
    }

    public void ReleaseUI(string uiprefab)
    {
        GameObject ui = null;
        if (m_UiDic.TryGetValue(uiprefab, out ui))
        {
            if (ui != null)
            {
                Object.Destroy(ui);
            }
            m_UiDic[uiprefab] = null;
            m_UiDic.Remove(uiprefab);
            ObjectsManager.UnloadManual(BundleBelong.ui, uiprefab);
        }
    }

    [ContextMenu("Release")]
    void Release()
    {
        CUility.GlobalRelease();
    }
}
