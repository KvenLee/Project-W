/*****************************************
 * A Star PathFinding
 * http://www.html-js.com/article/Random-A-routing-algorithm
 * 
 * ***************************************/

using System.Collections.Generic;
using UnityEngine;

public class PathItem
{
    public int x;
    public int y;

    public PathItem parent;
    public float F;
    public float G;
    public float H;
    public bool enable;
}

public class AStarPathFinding : MonoBehaviour
{
    public PathItem end_pos;
    public PathItem begin_pos;

    List<PathItem> gridMap = new List<PathItem>();

    List<PathItem> open_lst = new List<PathItem>();
    List<PathItem> close_lst = new List<PathItem>();


    void Find()
    {
        open_lst.Add(begin_pos);

    }

    void CalculateG(PathItem cur, ref PathItem next)
    {
        //左或右
        if(next.x == cur.x && next.y != cur.y)
        {
            next.G = Mathf.Abs(next.y - cur.y);
            return;
        }
        //上或下
        if(next.y == cur.y && next.x != cur.x)
        {
            next.G = Mathf.Abs(next.x - cur.x);
            return;
        }
        next.G = 1.4f * Mathf.Abs(next.x - cur.x);
    }
    void CalculateH(ref PathItem cur)
    {
        cur.H = Mathf.Abs(cur.x - end_pos.x) + Mathf.Abs(cur.y - end_pos.y);
    }

    void CalculateF(ref PathItem cur)
    {
        cur.F = cur.G + cur.H;
    }
}
