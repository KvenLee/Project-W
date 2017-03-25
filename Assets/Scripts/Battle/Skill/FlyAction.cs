using System.Collections.Generic;
using UnityEngine;

public delegate void CallBack ();
public delegate bool CallBackBool();

public class FlyAction : TimeBase
{
    private float begin_time = float.Epsilon;

    Dictionary<int, bool > m_IdInitDic = new Dictionary<int, bool>();

    /// <summary>
    /// 模拟重力的自由落体运动（speed>0向上运动，speed<0自由落体）
    /// </summary>
    /// <param name="time"></param>
    /// <param name="deltaTime"></param>
    /// <param name="targetY"></param>
    /// <param name="gravity"></param>
    /// <param name="speed">speed>0向上运动，speed<0自由落体</param>
    /// <param name="callback"></param>
    public void FlyFreeForce(float time, float deltaTime, float targetY, float force, float speed, CallBack callback = null, int id = -1)
    {
        bool init = false;
        if(m_IdInitDic.TryGetValue(id, out init))
        {
            if(!init)
            {
                m_IdInitDic[id] = true;
                begin_time = time;
            }
        }
        else
        {
            begin_time = time;
            m_IdInitDic.Add(id, true);
        }
        
        Vector3 pos = transform.position + Vector3.up * force * (time - begin_time) * speed * deltaTime;
        bool back = force > 0 ? pos.y < targetY : pos.y > targetY;
        if(back)
        {
            //减速直线
            transform.position = pos;
        }
        else
        {
            if(m_IdInitDic.ContainsKey(id))
            {
                m_IdInitDic[id] = false;
            }

            if(callback != null)
            {
                callback();
                callback = null;
            }
        }
    }

    private Vector3 cache;
    /// <summary>
    /// 跟随一个点的移动方式
    /// </summary>
    /// <param name="deltaScaleTime"></param>
    /// <param name="begin_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="speed"></param>
    /// <param name="callback"></param>
    public void FlyTo(float deltaScaleTime, Vector3 begin_pos, Vector3 end_pos, float speed, CallBack callback = null, int id = -1)
    {
        Vector3 dir = end_pos - begin_pos;

        bool init = false;
        if(m_IdInitDic.TryGetValue(id, out init))
        {
            if(!init)
            {
                m_IdInitDic[id] = true;
                cache = dir;
            }
        }
        else
        {
            cache = dir;
            m_IdInitDic.Add(id, true);
        }
        
        Vector3 next_pos = transform.position + dir.normalized * deltaScaleTime * speed * 10;
        if(Vector3.Angle(cache, dir) < 90 && (next_pos - end_pos).magnitude > 0.1f * 0.1f)
        {
            transform.position = next_pos;
            cache = dir;
        }
        else
        {
            if(m_IdInitDic.ContainsKey(id))
            {
                m_IdInitDic[id] = false;
            }
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
    }

    /// <summary>
    /// 非匀速直线Lerp插值
    /// </summary>
    /// <param name="deltaScaleTime"></param>
    /// <param name="begin_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="speed"></param>
    /// <param name="callback"></param>
    public void FlyLerp(float deltaScaleTime, Vector3 begin_pos, Vector3 end_pos, float speed, CallBack callback = null, int id = -1)
    {
        bool init = false;
        if(m_IdInitDic.TryGetValue(id, out init))
        {
            if(!init)
            {
                m_IdInitDic[id] = true;
            }
        }
        else
        {
            m_IdInitDic.Add(id, true);
        }

        float sqr = (end_pos - begin_pos).magnitude;
        if (sqr > 0.1f * 0.1f)
        {
            //减速直线
            transform.position = Vector3.Lerp(begin_pos, end_pos, deltaScaleTime * speed);
        }
        else
        {
            if(m_IdInitDic.ContainsKey(id))
            {
                m_IdInitDic[id] = false;
            }
            if(callback != null)
            {
                callback();
                callback = null;
            }
        }
    }

    /// <summary>
    /// 非匀速直线Lerp插值(具有最小插值距离)
    /// </summary>
    /// <param name="deltaScaleTime"></param>
    /// <param name="begin_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="speed"></param>
    /// <param name="callback"></param>
    public void FlyLerpMin(float deltaScaleTime, Vector3 begin_pos, Vector3 end_pos, float speed, CallBack callback = null, int id = -1)
    {
        bool init = false;
        if(m_IdInitDic.TryGetValue(id, out init))
        {
            if(!init)
            {
                m_IdInitDic[id] = true;
            }
        }
        else
        {
            m_IdInitDic.Add(id, true);
        }

        float sqr = (end_pos - begin_pos).magnitude;
        if (sqr >= 0.1f * 0.1f)
        {
            //减速直线
            if (sqr < 1f * 1f)
            {
                speed *= 2f;
            }
            transform.position = Vector3.Lerp(begin_pos, end_pos, deltaScaleTime * speed);;
        }
        else
        {
            if(m_IdInitDic.ContainsKey(id))
            {
                m_IdInitDic[id] = false;
            }
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
    }


    /// <summary>
    /// 匀速直线运动-Lerp插值
    /// </summary>
    /// <param name="time"></param>
    /// <param name="begin_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="speed"></param>
    /// <param name="callback"></param>
    public void FlyDirect(float time, Vector3 begin_pos, Vector3 end_pos, float speed, CallBack callback = null, int id = -1)
    {
        bool init = false;
        if(m_IdInitDic.TryGetValue(id, out init))
        {
            if(!init)
            {
                m_IdInitDic[id] = true;
                begin_time = time;
                riseCenter = begin_pos;
            }
        }
        else
        {
            m_IdInitDic.Add(id, true);
            begin_time = time;
            riseCenter = begin_pos;
        }

        if ((end_pos - transform.position).magnitude > .1f * .1f)
        {
            //匀速直线
            transform.position = Vector3.Lerp(riseCenter, end_pos, (time - begin_time) * speed);
        }
        else
        {
            if(m_IdInitDic.ContainsKey(id))
            {
                m_IdInitDic[id] = false;
            }
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
    }

    private Vector3 center = Vector3.zero;
    private Vector3 riseCenter, setCenter;
    /// <summary>
    /// 球形匀速-Slerp插值
    /// </summary>
    /// <param name="time"></param>
    /// <param name="begin_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="speed"></param>
    /// <param name="jumpY"></param>
    /// <param name="callback"></param>
    public void FlySphere(float time, Vector3 begin_pos, Vector3 end_pos, float speed, float jumpY, CallBack callback = null, int id = -1)
    {
        bool init = false;
        if(m_IdInitDic.TryGetValue(id, out init))
        {
            if(!init)
            {
                m_IdInitDic[id] = true;
                begin_time = time;
                center = (begin_pos + end_pos) * 0.5f; center.y -= Vector3.Distance(end_pos, begin_pos) * jumpY;
                riseCenter = begin_pos - center;
                setCenter = end_pos - center;
            }
        }
        else
        {
            m_IdInitDic.Add(id, true);
            begin_time = time;
            center = (begin_pos + end_pos) * 0.5f; center.y -= Vector3.Distance(end_pos, begin_pos) * jumpY;
            riseCenter = begin_pos - center;
            setCenter = end_pos - center;
        }

        if((end_pos - transform.position).magnitude > .1f * .1f)
        {
            //匀速球形曲线
            transform.position = Vector3.Slerp(riseCenter, setCenter, (time - begin_time) * speed) + center;
        }
        else
        {
            if(m_IdInitDic.ContainsKey(id))
            {
                m_IdInitDic[id] = false;
            }
            center = Vector3.zero;
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
    }
}
