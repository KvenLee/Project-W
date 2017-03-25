using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutHelper : MonoBehaviour
{
    public GameObject itemPrefab;

    float x,y;
    GridLayoutGroup group;
    Action<Transform, int> updateItemCallback = null;

    private bool m_IsInit;
    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if(m_IsInit)
            return;
        if(group == null)
        {
            group = GetComponent<GridLayoutGroup>();
        }
        if(itemPrefab != null)
        {
            RectTransform[] rts = itemPrefab.GetComponentsInChildren<RectTransform>();
            foreach(RectTransform rt in rts)
            {
                if(rt != null)
                {
                    if(rt.rect.width > x)
                    {
                        x = rt.rect.width;
                    }
                    if(rt.rect.height > y)
                    {
                        y = rt.rect.height;
                    }
                }
            }
            if(x != 0 && y != 0)
                group.cellSize = new Vector2(x, y);
        }
        m_IsInit = true;
    }

    public void BindDataSource(int count, Action<Transform, int> callback)
    {
        Init();
        updateItemCallback = callback;
        Bind(count);
    }

    private void Bind(int count)
    {
        int index = 0;
        foreach (Transform child in transform)
        {
            if (index >= count)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
                if (updateItemCallback != null)
                {
                    updateItemCallback(child, index);
                    //updateItemCallback = null;
                }
            }
            index++;
        }
        //创建
        for (; index < count; index++)
        {
            GameObject item = Instantiate(itemPrefab) as GameObject;
            item.transform.SetParent(transform);
            item.name = "item_" + index;
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            if (updateItemCallback != null)
            {
                updateItemCallback(item.transform, index);
                //updateItemCallback = null;
            }
        }
    }
}
