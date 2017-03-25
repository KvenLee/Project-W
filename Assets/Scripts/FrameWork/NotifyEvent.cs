using System.Collections.Generic;

/***********************
* 类功能说明: Notify消息底层结构
* 日期：
* 作者： 周 星
*
**********************/

namespace Notify
{
    public class Args : System.EventArgs { }
    public class Event
    {
        class Pair
        {
            public object obj;
            public StandardDelegate deleget;
            public Pair(object obj, StandardDelegate d)
            {
                this.obj = obj;
                this.deleget = d;
            }
        }

        static Dictionary<int, List<Pair>> eventsDic = new Dictionary<int, List<Pair>>();
        public delegate void StandardDelegate(Args e);
        /// <summary>
        /// 注册新的事件
        /// </summary>
        /// <param name="fun_id"></param>
        /// <param name="obj"></param>
        /// <param name="d"></param>
        public static void register(int fun_id, object obj, StandardDelegate d)
        {
            List<Pair> pairs = null;
            if (eventsDic.TryGetValue(fun_id, out pairs))
            {
                foreach (Pair p in pairs)
                {
                    if (p.obj.Equals(obj))
                    {
                        p.deleget += d;
                        return;
                    }
                }
                pairs.Add(new Pair(obj, d));
            }
            else
            {
                pairs = new List<Pair>();
                pairs.Add(new Pair(obj, d));
                eventsDic.Add(fun_id, pairs);
            }
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="fun_id"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool fire(int fun_id, Args e)
        {
            List<Pair> pairs = null;
            if (eventsDic.TryGetValue(fun_id, out pairs))
            {
                foreach (Pair p in pairs)
                    p.deleget(e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="obj"></param>
        public static void deregister(object obj)
        {
            List<int> funcnamelst = new List<int>();
            foreach (KeyValuePair<int, List<Pair>> kv in eventsDic)
            {
                for (int i = 0; i < kv.Value.Count; )
                {
                    if (kv.Value[i].obj.Equals(obj))
                    {
                        kv.Value.RemoveAt(i);
                        continue;
                    }
                    i++;
                }
                if (kv.Value.Count == 0)
                {
                    funcnamelst.Add(kv.Key);
                }
            }
            //从字典中移除List<Pair>为空的键值
            foreach (int s in funcnamelst)
            {
                if (eventsDic.ContainsKey(s))
                {
                    eventsDic.Remove(s);
                }
            }
        }
    }
}