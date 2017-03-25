using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITweenAlpha : UITweenHandler
{
	public float from;
    public float to;
    public float duration = 1f;

    Graphic[] graphics;
    public override void Play(bool isForward)
    {
        base.Play(isForward);

        if (graphics == null)
        {
            graphics = transform.GetComponentsInChildren<Graphic>(true);
        }
        switch (tweenOnActive)
        {
            case TweenOnActive.ContinueFromCurrent:
                break;
            case TweenOnActive.RestartTween:
                Color color;
                foreach (Graphic g in graphics)
                {
                    color = g.color; color.a = isForward ? from : to;
                    g.color = color;
                }
                break;
        }
        
        foreach (Graphic g in graphics)
        {
            DOTween.ToAlpha(delegate() { return g.color; }, delegate(Color x) { g.color = x; }, isForward ? to : from, duration);
        }
    }

    [ContextMenu("Convert from<--->to values")]
    void Convert()
    {
        float temp = from;
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
