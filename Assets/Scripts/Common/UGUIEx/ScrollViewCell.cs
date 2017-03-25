using UnityEngine;
using System.Collections;

/// <summary>
/// UGUI循环ScrollView的item
/// </summary>

public class ScrollViewCell : MonoBehaviour
{
    protected ScrollViewLooper controller = null;
    protected System.Object dataObject = null;
    private int dataIndex;
    protected float cellHeight;
    protected float cellWidth;
    protected bool deactivateIfNull = true;
    protected ScrollViewCell parentCell;

    public GameObject item
    {
        get
        {
            return gameObject;
        }
    }
    public Transform transform
    {
        get
        {
            return item.transform;
        }
    }

    public System.Object DataObject
    {
        get { return dataObject; }
        set
        {
            dataObject = value;
            ConfigureCellData();
        }
    }

    public int DataIndex
    {
        get { return dataIndex; }
    }

    public virtual void Init(ScrollViewLooper controller, System.Object data, int index, float cellHeight = 0.0f, float cellWidth = 0.0f, ScrollViewCell parentCell = null)
    {
        this.controller = controller;
        this.dataObject = data;
        this.dataIndex = index;
        this.cellHeight = cellHeight;
        this.cellWidth = cellWidth;
        this.parentCell = parentCell;

        if (deactivateIfNull)
        {
            if (data == null)
                this.item.SetActive(false);
            else
                this.item.SetActive(true);
        }
    }

    public void ConfigureCell()
    {
        this.ConfigureCellData();
    }

    protected virtual void ConfigureCellData()
    {
    }
}
