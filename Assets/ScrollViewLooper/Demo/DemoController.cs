using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data
{
    public int index;
    public string _name;
}

public class DemoController : MonoBehaviour
{
    public ScrollViewLooper scrollController;
    // Use this for initialization
    void Start()
    {
        List<Data> list = new List<Data>();
        for (int i = 0; i < 20; i++)
        {
            Data d = new Data();
            d.index = i;
            d._name = "item" + i;
            list.Add(d);
        }

        scrollController.BindDataSource(list);

        // scrollController.setCellsData(20,2 );
    }
}
