using UnityEngine;
using System.Collections;

/// <summary>
/// buff生命周期
/// 回合开始的时候触发效果 
/// 回合中的时候buff不消失 
/// 回合结束的时候结束消失
/// </summary>

public class BaseBuff : FlyAction
{
    public int buffID;
    public string buffname;
    public NegStateEnum buffState;
    public ActorBase m_Target;

    protected override void Awake()
    {
        base.Awake();
        Notify.Event.register((int)BattleCmd.MSGID_BATTLEROUND, this, BuffEffect);
    }

    public virtual void BuffEffect(Notify.Args e)
    {
        if (m_Target != null)
        {

        }
    }

    protected override void Update()
    {
        base.Update();
        BuffUpdate();
    }

    public virtual void BuffUpdate()
    {
        if (m_Time - _timer > 2)
        {
            BuffOver(null);
        }
    }

    public virtual void BuffBegin(ActorBase target)
    {
        m_Target = target;
        if (m_Target != null)
        {
            transform.SetParent(m_Target.transform);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            m_Target.HurtAddNeg(buffState);
            _timer = m_Time;
            //Debug.Log("<BUFF>BuffType:" + buffState + " TargetName:" + m_Target.name);
        }
    }

    public virtual void BuffOver(Notify.Args e)
    {
        if (m_Target != null)
        {
            m_Target.HurtSubNeg(buffState);
            m_Target = null;
            gameObject.SetActive(false);
        }
    }
}
