using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSys : GameSys<LoadingSys>
{
    private string prefab = "UILoading";
    public LoadingUICtrl CtrlUI { get; private set; }

    public override void SysEnter()
    {
        if(CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<LoadingUICtrl>(prefab);
        }
        if(CtrlUI != null)
        {
            DontDestroyOnLoad(CtrlUI.gameObject);
            m_progress = 0;
            StartCoroutine("Go");
        }
        base.SysEnter();
    }

    public override void SysUpdate()
    {
        if(CtrlUI != null )
        {
            external_progress = asyncOperator != null ? asyncOperator.progress : external_progress;
            CtrlUI.m_SprCircle.transform.Rotate(Vector3.forward, -500 * CGameRoot.update_frame);

            m_progress += CGameRoot.update_frame;
            if(m_progress > external_progress)
                m_progress = external_progress;

            if(m_progress >= 1)
            {
                if(CompleteCallback != null)
                {
                    CompleteCallback();
                    CompleteCallback = null;
                }
                SysLeave();
            }
            else
            {
                int pos = Mathf.RoundToInt(m_progress * 1334);
                //CtrlUI.m_SprSlider.SetNativeSize(pos, 20);
                CtrlUI.m_SprStar.transform.localPosition = new Vector3(pos-1334, 112, 0);
                CtrlUI.m_LabProgress.text = string.Format("{0:f2}%", m_progress * 100);
            }
        }
        base.SysUpdate();
    }

    public override void SysLeave()
    {
        if(CtrlUI != null)
        {
            //CtrlUI.m_SprSlider.SetDimensions(0, 20);
            CtrlUI.m_SprStar.transform.localPosition = new Vector3(-1334, 112, 0);
            CtrlUI.m_LabProgress.text = string.Format("{0:f2}%", 0);

            CtrlUI.gameObject.SetActive(false);
            CtrlUI = null;
            asyncOperator = null;
            UIManager.Instance.ReleaseUI(prefab);
            CUility.GlobalRelease();
        }
        base.SysLeave();
    }
    public float external_progress;
    AsyncOperation asyncOperator = null;

    Func<IEnumerator> func = null;
    Action CompleteCallback = null;

    string nextScene =string.Empty;
    float m_progress = 0;

    public void SetExternalProgress(float progress)
    {
        if(progress <= 1 && progress >= 0)
        {
            external_progress = progress;
        }
    }

    public void OnShow(Func<IEnumerator> func, Action complete)
    {
        this.func = func;
        CompleteCallback = complete;

        ObjectsManager.UnloadAll();
        CGameRoot.SwitchState(SystemEnum.Loading);
    }

    public void Goto(string _nextScene, Action _callback)
    {
        nextScene = _nextScene;
        CompleteCallback = _callback;
        
        if(string.IsNullOrEmpty(nextScene))
        {
            if(CompleteCallback != null)
            {
                CompleteCallback();
                CompleteCallback = null;
            }
        }
        else
        {
            ObjectsManager.UnloadAll();
            CGameRoot.SwitchState(SystemEnum.Loading);
        }
    }
    IEnumerator Go()
    {
//         if (func != null)
//         {
//             yield return StartCoroutine(func());
//             func = null;
//         }
//         yield return null;
//         if (CompleteCallback != null)
//         {
//             CompleteCallback();
//             CompleteCallback = null;
//         }

        SceneManager.LoadScene(1);
        yield return null;
        
        asyncOperator = SceneManager.LoadSceneAsync(nextScene);
        yield return asyncOperator;

        if(asyncOperator.isDone)
        {
            yield break;
        }
    }
}
