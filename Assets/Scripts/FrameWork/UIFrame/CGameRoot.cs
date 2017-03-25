using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum SystemEnum
{
    None = 0,       //空状态
    ResourceUpdate,
    Login,
    Loading,
    Setting,

    Depart,
    Pub,
    Shop,
    PreBattle,
    Barrack,
    Field,
    Market,
    Map,

    //当前游戏模块
    Main,           //主游戏UI
    Bag,            //背包
    Heros,          //主角（影星英雄）
    HeroDetail,     //英雄详情
    HeroUp,         //英雄培养
    Task,           //任务
    Enroll,         //招募
    WorldMap,       //世界地图

    //通用界面 特殊处理
    MainGround,     //世界背景 
    Economic,       //经济面板

    //战斗
    Battle,

    Common,
}

public delegate void StateChange();
public class CGameRoot : TUSingleton<CGameRoot>
{
    public EventSystem uiEvtSys;
    public SystemEnum firstSystem;

    public bool IsPointOnUI
    {
        get
        {
            if(EventSystem.current == null)
            {
                EventSystem.current = uiEvtSys;
            }
#if  UNITY_EDITOR || UNITY_STANDALONE
            return EventSystem.current.IsPointerOverGameObject();
#else
            if(Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            else
                return false;
#endif
        }
    }

    /// <summary>
    /// UI的更新帧数30
    /// </summary>
    public int frame_count_ui = 30;
    public bool updateCurrentSys;

    static Dictionary<SystemEnum, CGameSystem> sysDic;

    public static CGameSystem currentSystem;

    public static StateChange stateLeaveCallback;
    public static StateChange stateEnterCallback;

    public static float update_frame;
    float deltaTime;

    [ContextMenu("Test")]
    void Test()
    {
        MessageTipSys.Instance.ShowMessage("一只活泼可爱的小青蛙!!");
    }

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        Shader.WarmupAllShaders();

        sysDic = new Dictionary<SystemEnum, CGameSystem>
        {
            {SystemEnum.ResourceUpdate, AddComponentEx<ResourcesUpdate>()},
            {SystemEnum.Loading, AddComponentEx<LoadingSys>()},
            
            {SystemEnum.MainGround, AddComponentEx<MainBackGroundSys>()},
            {SystemEnum.Economic, AddComponentEx<EconomicSys>()},

            {SystemEnum.Main, AddComponentEx<MainGamePageSys>()},

            {SystemEnum.Bag, AddComponentEx<BagSys>()},
            {SystemEnum.Heros, AddComponentEx<MainPlayerSys>()},
            {SystemEnum.HeroDetail, AddComponentEx<HeroInfoSys>()},
            {SystemEnum.HeroUp, AddComponentEx<HeroGrowSys>()},
            {SystemEnum.Task, AddComponentEx<TaskSys>()},
            {SystemEnum.Enroll, AddComponentEx<EnrollSys>()},
            {SystemEnum.WorldMap, AddComponentEx<WorldMapSys>()},
                    
            {SystemEnum.PreBattle, AddComponentEx<PreBattleSys>()},
            //battle
            {SystemEnum.Battle, AddComponentEx<BattleSys>()},
            
            //common
            {SystemEnum.Common, AddComponentEx<MessageTipSys>()},
        };
    }

    T AddComponentEx<T>() where T : CGameSystem
    {
        GameObject go = new GameObject(typeof(T).Name);
        go.transform.SetParent(transform);
        T sys = go.AddComponent<T>();
        sys._SysInit();
        return sys;
    }

    public void Start()
    {
        update_frame = 1F / frame_count_ui;
        deltaTime = Time.deltaTime;
        //InvokeRepeating("SysUpdate", deltaTime, update_frame);

        if (firstSystem != SystemEnum.None)
        {
            SwitchState(firstSystem);
        }
    }

    void /*SysUpdate*/Update()
    {
        if (updateCurrentSys)
        {
            if (currentSystem != null)
                currentSystem._SysUpdate();
            return;
        }
        foreach (KeyValuePair<SystemEnum, CGameSystem> kv in sysDic)
        {
            if (kv.Value.IsSysEnter)
            {
                kv.Value._SysUpdate();
            }
        }
    }

    #region Frame

    public static SystemEnum m_PreState;
    public static SystemEnum m_CurState;

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="state"></param>
    public static void SwitchState(SystemEnum state)
    {
        //切换状态时，先关闭所有叠加状态，在切换主状态
        foreach (SystemEnum se in m_OverlapState)
        {
            Close(se);
        }
        m_OverlapState.Clear();

        CGameSystem next = null;
        if (sysDic.TryGetValue(state, out next))
        {
            if (currentSystem != null)
            {
                currentSystem._SysLeave();
                m_PreState = m_CurState;
                if (stateLeaveCallback != null)
                {
                    stateLeaveCallback();
                }
            }
            if (next != null)
            {
                m_CurState = state;
                currentSystem = next;
                currentSystem._SysEnter();
                if (stateEnterCallback != null)
                {
                    stateEnterCallback();
                }
            }
            else
            {
                m_CurState = SystemEnum.None;
            }
        }
    }

    static List<SystemEnum> m_OverlapState = new List<SystemEnum>();

    public static void SwitchOverLapState(SystemEnum state)
    {
        //切换状态时，先关闭所有叠加状态，在切换主状态
        foreach (SystemEnum se in m_OverlapState)
        {
            Close(se);
        }
        m_OverlapState.Clear();

        if (m_OverlapState.FindIndex(delegate(SystemEnum temp)
        {
            return temp == state;
        }) != -1)
        {
            Debug.LogError("该系统已经打开，或者未正常关闭!!" + state);
        }
        else
        {
            try
            {
                sysDic[state]._SysEnter();
            }
            catch (System.Exception e)
            {
                Debug.LogError("当前状态不存在!!" + state);
            }
            m_OverlapState.Add(state);
        }
    }

    /// <summary>
    /// 叠加状态
    /// </summary>
    /// <param name="state"></param>
    public static void OverlapState(SystemEnum state)
    {
        if (m_OverlapState.FindIndex(delegate(SystemEnum temp)
        {
            return temp == state;
        }) != -1)
        {
            Debug.LogError("该系统已经打开，或者未正常关闭!!" + state);
        }
        else
        {
            CGameSystem next = null;
            if (sysDic.TryGetValue(state, out next))
            {
                next._SysEnter();
            }
            m_OverlapState.Add(state);
        }
    }
    public static void CloseOverlapState(SystemEnum state)
    {
        if (m_OverlapState.FindIndex(delegate(SystemEnum temp)
        {
            return temp == state;
        }) != -1)
        {
            Close(state);
            m_OverlapState.Remove(state);
        }
    }
    static void Close(SystemEnum state)
    {
        CGameSystem next = null;
        if (sysDic.TryGetValue(state, out next))
        {
            next._SysLeave();
        }
    }

    #endregion
}

/// <summary>
/// 非叠加切换系统
/// </summary>
/// <typeparam name="T"></typeparam>
public class GameSys<T> : CGameSystem where T : CGameSystem
{
    public static T Instance;

    public override void SysInit()
    {
        base.SysInit();
        if (Instance == null)
        {
            Instance = this as T;
        }
    }

    public override void SysEnter()
    {
        base.SysEnter();
        BindFunc();
        AddListener();
    }

    public override void SysLeave()
    {
        base.SysLeave();
    }

    public virtual void BindFunc() { }
    public virtual void AddListener() { }
}

/// <summary>
/// 叠加系统
/// </summary>
/// <typeparam name="T"></typeparam>
public class GameOverlapSys<T> : CGameSystem where T : CGameSystem
{
    public static T Instance;
    public override void SysInit()
    {
        base.SysInit();
        if (Instance == null)
            Instance = this as T;
    }

    public override void SysEnter()
    {
        BindFunc();
    }

    public override void SysLeave()
    {
        base.SysLeave();
    }

    public virtual void BindFunc() { }
}

public class CGameSystem : UIEventSys
{
    public bool IsSysEnter
    {
        get;
        private set;
    }

    public bool IsSysInit
    {
        get;
        private set;
    }

    #region 框架调用

    /// <summary>
    /// 作用相当于Awake
    /// </summary>
    public void _SysInit()
    {
        if (!IsSysInit)
        {
            SysInit();
            IsSysInit = true;
        }
    }
    public void _SysEnter()
    {
        if (!IsSysEnter)
        {
            SysEnter();
            IsSysEnter = true;
        }
    }
    public void _SysUpdate()
    {
        if (IsSysEnter)
        {
            SysUpdate();
        }
    }
    public void _SysLeave()
    {
        if (IsSysEnter)
        {
            ClearListener();
            SysLeave();
            IsSysEnter = false;
        }
    }

    #endregion

    //外部调用
    public virtual void SysInit() { }
    public virtual void SysEnter() { }
    public virtual void SysUpdate() { }
    public virtual void SysLeave() { }
    public virtual void OnDestroy()
    {
        Notify.Event.deregister(this);
    }
}
