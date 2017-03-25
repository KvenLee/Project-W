using DG.Tweening;
using UnityEngine;

public class UITweenPosition : UITweenHandler
{
    public Vector3 from;
    public Vector3 to;
    public float duration = 1f;

    public override void Play(bool forward)
    {
        base.Play(forward);
        switch (tweenOnActive)
        {
            case TweenOnActive.ContinueFromCurrent:
                break;
            case TweenOnActive.RestartTween:
                trans.position = forward ? from : to;
                break;
        }
        trans.DOMove(forward ? to : from, duration).SetEase<Tweener>(defaultEaseType).OnComplete<Tweener>(delegate()
        {
            Finish(forward);
        });
    }

    [ContextMenu("Convert from<--->to values")]
    void Convert()
    {
        Vector3 temp = from;
        from = to;
        to = temp;
    }
    [ContextMenu("PlayForward")]
    void PlayTest1()
    {
        Play(true);
    }
    [ContextMenu("PlayReverse")]
    void PlayTest2()
    {
        Play(false);
    }
}