using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapDir
{
    top = 3,
    down = 6,
    left = 1,
    right = 2,
    lefttop = 4,
    leftdown = 7,
    righttop = 5,
    rightdown = 8,
    center = 0,
}

/// <summary>
/// 九宫格地图
/// </summary>
public class GridMapMgr : MonoBehaviour
{
    public Transform cam;
    public Transform mapParent;
    public string map_id;
    public int colume = 4;

    private MapCfg mapCfg;
    private List<GameObject> renders;
    private Transform mainActor;

    IEnumerator Start()
    {
        mapCfg = ObjectsManager.LoadObject<GameObject>(BundleBelong.map, map_id).GetComponent<MapCfg>();
        mainActor = BuildActor(110, new Vector2(30, -10), true);
        yield return null;
        SpreadMaps();
    }

    [ContextMenu("Get")]
    void Get()
    {
        Debug.Log(GetMapIndex(mainActor.position));
    }

    /// <summary>
    /// 获得目标点所在的地图索引（资源）
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    int GetMapIndex(Vector2 pos)
    {
        for(int i=0; i < mapCfg.Count; i++)
        {
            float left_x = MapCfg.mapDistance / 2 * -1 + MapCfg.mapDistance * (i % colume);
            float top_y = MapCfg.mapDistance / 2 - MapCfg.mapDistance * (i / colume);
            if(pos.x >= left_x && pos.x < left_x + MapCfg.mapDistance && pos.y >= top_y - MapCfg.mapDistance && pos.y < top_y)
            {
                Debug.Log(i % colume + "," + i / colume);
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 获取目标点所在的地图二维索引（x,y）
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool GetMapXY(Vector2 pos, out int x, out int y)
    {
        x = y = 0;
        int index = GetMapIndex(pos);
        if(index == -1)
            return false;
        x = index % colume;
        y = index / colume;
        return true;
    }
    int ConvertXY2Index(int x, int y)
    {
        return y * colume + x;
    }

    int[] GetNextMaps(Vector2 point, int length = 1)
    {
        int x, y = 0;
        List<int> index = new List<int>();
        if(!GetMapXY(point, out x, out y))
        {
            return null;
        }
        MapDir dir = GetMapDir(point);
        switch(dir)
        {
            //计算地图编号
            case MapDir.top: y -= length; break;
            case MapDir.down: y += length; break;
            case MapDir.left: x -= length; break;
            case MapDir.right: x += length; break;
            case MapDir.lefttop: x -= length; y -= length; break;
            case MapDir.righttop: x += length; y -= length; break;
            case MapDir.leftdown: x -= length; y += length; break;
            case MapDir.rightdown: x += length; y += length; break;
        }

        return new int[2];
    }


    public MapDir GetMapDir(Vector2 point)
    {
        int mapIndex = GetMapIndex(point);
        return GetMapDir(mapIndex, point);
    }

    /// <summary>
    /// 获取目标点所在地图的方位
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    MapDir GetMapDir(int mapIndex, Vector2 point)
    {
        int dir = 0;
        if(mapIndex != -1)
        {
            float left_x = MapCfg.mapDistance / 2 * -1 + MapCfg.mapDistance * (mapIndex % colume);
            float right_x = left_x + MapCfg.mapDistance;
            float center_left = left_x + MapCfg.mapDistance / 4;
            float center_right = right_x - MapCfg.mapDistance / 4;
            //判断x方位
            if(point.x > left_x && point.x < center_left)
            {
                //左
                dir = (int)MapDir.left;
            }
            else if(point.x > center_right && point.x < right_x)
            {
                //右
                dir = (int)MapDir.right;
            }
            else
            {
                //中
                dir = (int)MapDir.center;
            }

            float top_y = MapCfg.mapDistance / 2 - MapCfg.mapDistance * (mapIndex / colume);
            float down_y = top_y - MapCfg.mapDistance;
            float center_top = top_y - MapCfg.mapDistance / 2;
            float center_down = down_y + MapCfg.mapDistance / 2;
            if(top_y > point.y && point.y > center_top)
            {
                //上
                dir += (int)MapDir.top;
            }
            else if(center_down > point.y && point.y < down_y)
            {
                //下
                dir += (int)MapDir.down;
            }
            else
            {
                //中
                dir += (int)MapDir.center;
            }
        }
        return (MapDir)dir;
    }




    [ContextMenu("SpreadMaps")]
    void SpreadMaps()
    {
        if(renders == null)
        {
            renders = new List<GameObject>();
        }
        Vector2 map_pos = Vector2.zero;
        for(int i=0; i < mapCfg.Count; i++)
        {
            if(i != 0)
            {
                if(i % colume == 0)
                {
                    map_pos.y -= MapCfg.mapDistance;
                    map_pos.x = 0;
                }
                else
                {
                    map_pos.x += MapCfg.mapDistance;
                }
            }

            if(renders.Count > i)
            {
                renders[i].GetComponent<SpriteRenderer>().sprite = mapCfg[i];
                renders[i].transform.position = map_pos;
                renders[i].transform.SetParent(mapParent);
                renders[i].name = "map" + i % colume + "," + i / colume;
            }
            else
            {
                GameObject map = new GameObject("map" + i % colume + "," + i / colume);
                map.AddComponent<SpriteRenderer>().sprite = mapCfg[i];
                map.transform.SetParent(mapParent);
                map.transform.position = map_pos;
                map.transform.localEulerAngles = Vector3.zero;
                map.transform.localScale = Vector3.one;
                renders.Add(map);
            }
        }
    }

    //========

    Transform BuildActor(int source_id, Vector2 actor_pos, bool isMainActor = false)
    {
        GameObject actor = ObjectsManager.CreateGameObject(BundleBelong.prefab, "ActorRoot", false);
        if(actor != null)
        {
            actor.name = "Actor_" + source_id;
            actor.transform.SetParent(transform);
            actor.transform.localPosition = actor_pos;
            actor.transform.localEulerAngles = Vector3.zero;
            actor.transform.localScale = Vector3.one;

            GameObject model =ObjectsManager.CreateGameObject(BundleBelong.prefab, "actor001", false);
            if(model != null)
            {
                model.transform.SetParent(actor.transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localScale = Vector3.one;
                model.transform.localEulerAngles = Vector3.one;
                model.GetComponent<SpriteRenderer>().sortingOrder = 10;
                Animator am = model.GetComponent<Animator>();
                am.runtimeAnimatorController = ObjectsManager.LoadObject<RuntimeAnimatorController>(BundleBelong.animation, "actor001@normal");
                actor.GetComponent<PolyNavAgent>().m_Animator = am;
                if(isMainActor)
                {
                    actor.AddComponent<ClickToMove>();
                    if(cam != null)
                        cam.GetComponent<CameraFollow>().follower = actor.transform;
                }
            }
        }
        return actor.transform;
    }
}