/*---------------------------------------------------------------------------------------------------------------------
*--  文件名：   
*--  创建人:    vekcon.zx
*--    日期:    2016-6-1
*--    描述：   
*--------------------------------------------------------------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SkillBase
{
    /// <summary>
    /// 技能id
    /// </summary>
    public uint player_skill_id;
    public uint player_skill_lv;

    public uint player_skill_roundcd;
    public ActorBase Sender;
    public SkillConfig skillCfg;
    public bool is_Using_Skill;
    public bool is_EsoSkill_Ready;

    public Vector3 hitPos
    {
        get
        {
            Vector3 pos = Vector3.zero;
            switch(skillCfg.hitPos)
            {
                case HitPosEnum.Target:
                    if (m_Targets.Count > 0)
                    {
                        pos = m_Targets[0].battleTeam == Pb.TeamType.CS_TEAM_BLUE ? 
                            m_Targets[0].position + Vector3.right * BattleCfg.HIT_DISTANCE :
                            m_Targets[0].position + Vector3.left * BattleCfg.HIT_DISTANCE;
                    }
                    if (skillCfg.skillType == SkillType.CHASE)
                    {
                        pos.y += 0.5f;
                    }
                    break;
                case HitPosEnum.Center:
                    pos = Vector3.zero;
                    break;
            }
            return pos;
        }
    }

    List<ActorBase> m_Targets = new List<ActorBase>();
    public virtual List<ActorBase> SkillTargets
    {
        get
        {
            if(!is_Using_Skill && m_Targets.Count == 0)
            {
                m_Targets.Clear();
                Pb.TeamType team = (Pb.TeamType)(Pb.TeamType.CS_TEAM_RED - Sender.battleTeam + 1);
                List<ActorBase> actors = RoundMgr.Instance.GetActorByTeam(team);
                if(actors == null || actors.Count == 0)
                    return m_Targets;

                switch(skillCfg.targetGetter)
                {
                    case TargetGetterType.Single:
                        m_Targets.Add(actors[0]);
                        break;
                    case TargetGetterType.Floating:
                        foreach(ActorBase p in actors)
                        {
                            if(p.stateOne == RoleOneState.FloatingTop)
                            {
                                m_Targets.Add(p);
                                break;
                            }
                        }
                        break;
                }
            }
            return m_Targets;
        }
    }
    
    /// <summary>
    /// 是否可以使用技能
    /// </summary>
    /// <returns></returns>
    public bool CanUseSkill(SkillType skillType)
    {
        if(skillCfg.skillType == SkillType.ESOTERIC)
        {
            return !is_Using_Skill && player_skill_roundcd == 0 && SkillTargets != null && SkillTargets.Count > 0 && is_EsoSkill_Ready;
        }
        else
        {
            return !is_Using_Skill && player_skill_roundcd == 0 && SkillTargets != null && SkillTargets.Count > 0;
        }
    }
    /// <summary>
    /// 是否可以使用追打技能
    /// </summary>
    /// <returns></returns>
    public bool IsCanUseChaseSkill()
    {
        return skillCfg.skillType == SkillType.CHASE 
            && !is_Using_Skill 
            && player_skill_roundcd == 0 
            && SkillTargets != null 
            && SkillTargets.Count > 0;
    }

    public void SkillRound()
    {
        if(player_skill_roundcd > 0)
            player_skill_roundcd--;
    }
    public void SkillBegin()
    {
        is_Using_Skill = true;
    }
    public void SkillEnd()
    {
        is_Using_Skill = false;
        is_EsoSkill_Ready = false;
        m_Targets.Clear();
    }
}

/// <summary>
/// 技能配置
/// </summary>
public class SkillConfig : MonoBehaviour
{
    public int skillID;
    public string skillName;
    public string skillDes;
    public uint skillCD;                 //技能CD（冷却回合）
    /// <summary>
    /// 技能类型
    /// </summary>
    public SkillType skillType = SkillType.NORMAL;

    //动画配置
    public string preAttack;            //打击起手动作
    public string attack;               //打击动作
    public float persistTime;           //持续时间
    public float aniSpeed;              //播放速度 
    //动画特效
    public string animFx;
    /// <summary>
    /// 攻击距离(0:近程 1：远程)
    /// </summary>
    public float attackRange;
    public float moveSpeed;             //移动速度

    public HitPosEnum hitPos;
    public TargetGetterType targetGetter;           //目标选取方式
    /// <summary>
    /// 技能被执行次数(比如打击多次)
    /// </summary>
    public int excuteCount = 1;
    /// <summary>
    /// 动画事件
    /// </summary>
    public AnimEvent[] animEvts;
}

[System.Serializable]
public class AnimEvent
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public string eventName;
    //打击时刻(0-1)
    public float hitTime;

    //===重置动画设置
    public bool resetAnim;
    //...

    //===打击动画设置
    
    public string audioClip;        //击中音效
    public int damage;              //伤害
    //特效
    public string[] hitFx;
    //buff
    public string[] buffs;
    /// <summary>
    /// 攻击附加状态
    /// </summary>
    public AttackAttachOneState attachState;
}

