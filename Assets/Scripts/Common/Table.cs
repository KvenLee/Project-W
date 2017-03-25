using UnityEngine;
using System.Collections;

public class Table : MonoBehaviour
{
    Transform[] childs;
    public int width  = 1;
    public float offset = 0.1f;

    // Use this for initialization
    void Start()
    {
        childs = new Transform[transform.childCount];
        int i = 0;
        foreach(Transform t in transform)
        {
            childs[i++] = t;
        }
    }

    [ContextMenu("Set")]
    void Set()
    {
        int count = 0;
        int colume = 1;
        foreach(Transform t in transform)
        {
            t.localPosition += new Vector3(offset * colume, 0, 0);
            count++;
            if(count >= width)
            {
                count = 0;
                colume++;
            }
        }
    }
}
