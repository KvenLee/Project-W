using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*******************************
 *  Author: Andrea Chow.
 *  Time:   2015.08.15
 *  Function: 场景过载效果
 * *****************************/

public class MaskTransfer : MonoBehaviour {

    public float durtion = 1f;

    float currAlpha;        //当前的透明值
    float timer;            //计时器
    bool istransfer;

    static Func<IEnumerator> delegateFun = null;
    static Action middleCallback = null;
    static Action finishCallback = null;
    static Texture maskTexture;

    static MaskTransfer mInstance;
    public static MaskTransfer Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new GameObject("MaskTransfer").AddComponent<MaskTransfer>();
                maskTexture = Resources.Load<Texture>("maskTexture");
                DontDestroyOnLoad(mInstance);
            }
            return mInstance;
        }
    }

    /// <summary>
    /// release resource!
    /// </summary>
    void OnDestroy()
    {
        delegateFun = null;
        middleCallback = null;
        finishCallback = null;
        ObjectsManager.UnLoadDirectly(maskTexture);
        mInstance = null;
    }

    /// <summary>
    /// 1个参数的调用（1）
    /// </summary>
    /// <param name="func"></param>
    public void OnShow(Func<IEnumerator> func)
    {
        OnShow3(func, null, null);
    }
    /// <summary>
    /// 2个参数的调用(1和2)
    /// </summary>
    /// <param name="func"></param>
    /// <param name="middle"></param>
    public void OnShow2(Func<IEnumerator> func, Action middle)
    {
        OnShow3(func, middle, null);
    }

    /// <summary>
    /// 3个参数的调用（1、2、3）
    /// </summary>
    /// <param name="func"></param>
    /// <param name="middle"></param>
    /// <param name="complete"></param>
    public void OnShow3(Func<IEnumerator> func, Action middle, Action complete)
    {
        if (istransfer)
        {
            StopAllCoroutines();
            delegateFun = null;
            middleCallback = null;
            finishCallback = null;
            istransfer = false;
        }

        delegateFun = func;
        middleCallback = middle;
        finishCallback = complete;

        gameObject.SetActive(true);
        StartCoroutine("DoTransfer");
    }

    List<string> showLst = new List<string>()
    {
        "战斗","迷惑","连击","郭靖","黄蓉","杨康","完颜康","穆念慈","一灯","暴力","情感","羁绊","融合","随机","你","我","他","它","她","糟糕","你输了","你赢了","住手","哈",
    };
    int index = 0;
    float _timer;
    float x,y;

    void OnGUI()
    {
        GUI.depth = 0;
        GUI.color = new Color(1, 1, 1, currAlpha / (durtion / 2));
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), maskTexture);
        //draw mask text .
        if(currAlpha / (durtion / 2) > 0.6f)
        {
            GUI.Label(new Rect(x, y, 500, 300), showLst[index % showLst.Count]);

            //遮罩过程
            if(Time.time - _timer > 0.1f)
            {
                x = UnityEngine.Random.Range(Screen.width / -2, Screen.width / 2);
                y = UnityEngine.Random.Range(Screen.height / -2, Screen.height / 2);
                index++;
                _timer = Time.time;
            }
        }
    }

    IEnumerator DoTransfer()
    {
        istransfer = true;
        timer = currAlpha = 0;

        while (0 <= timer && timer < durtion / 2)
        {
            timer += Time.deltaTime;
            currAlpha += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ObjectsManager.UnloadAll();
        yield return null;

        if (delegateFun != null)
        {
            yield return StartCoroutine(delegateFun());
            delegateFun = null;
        }

        if (middleCallback != null)
        {
            middleCallback();
            middleCallback = null;
            yield return null;
        }

        while (timer > durtion / 2 && timer < durtion)
        {
            timer += Time.deltaTime;
            currAlpha -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
        istransfer = false;
        gameObject.SetActive(false);

        if (finishCallback != null)
        {
            finishCallback();
            finishCallback = null;
        }
    }
}
