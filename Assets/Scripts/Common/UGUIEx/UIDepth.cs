using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDepth : MonoBehaviour
{
    public int order;
    public bool isUI = false;

    // Use this for initialization
    void Start()
    {
        if (isUI)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renders)
            {
                r.sortingOrder = order;
            }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (isUI)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renders)
            {
                r.sortingOrder = order;
            }
        }
    }
#endif
}
