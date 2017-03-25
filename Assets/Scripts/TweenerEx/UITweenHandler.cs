using System;
using UnityEngine;
using DG.Tweening;

public enum TweenAfterForward
{
    DoNothing,
    Enable,
    Disable,
}
public enum TweenOnActive
{
    ContinueFromCurrent,
    RestartTween,
}
public class UITweenHandler : MonoBehaviour
{
    public GameObject attachRoot;
    public Ease defaultEaseType = Ease.Linear;
    public TweenAfterForward finishAfterForward = TweenAfterForward.Enable;
    public TweenAfterForward finishAfterReverse = TweenAfterForward.Disable;
    public TweenAfterForward ifTargetIsDisabled = TweenAfterForward.Enable;
    public TweenOnActive tweenOnActive = TweenOnActive.ContinueFromCurrent;

    public Action<bool> OnFinishCallBack = null;

    protected Transform trans
    {
        get
        {
            RectTransform rt = GetComponent<RectTransform>();
            if(rt != null)
            {
                return rt;
            }
            else
            {
                return transform;
            }
        }
    }

    public virtual void Play(bool isForward)
    {
        switch (ifTargetIsDisabled)
        {
            case TweenAfterForward.Disable:
                ActiveGameObject(false);
                break;
            case TweenAfterForward.Enable:
                ActiveGameObject(true);
                break;
            case TweenAfterForward.DoNothing:
                break;
        }
    }

    public virtual void Finish(bool isForward)
    {
        switch (isForward ? finishAfterForward : finishAfterReverse)
        {
            case TweenAfterForward.Disable:
                ActiveGameObject(false);
                break;
            case TweenAfterForward.Enable:
                ActiveGameObject(true);
                break;
            case TweenAfterForward.DoNothing:
                break;
        }
        if (OnFinishCallBack != null)
        {
            OnFinishCallBack(isForward);
            OnFinishCallBack = null;
        }
    }

    void ActiveGameObject(bool active)
    {
        gameObject.SetActive(active);
        if(attachRoot != null)
        {
            attachRoot.SetActive(active);
        }
    }
}