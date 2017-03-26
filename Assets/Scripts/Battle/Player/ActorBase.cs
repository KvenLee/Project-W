using UnityEngine;
using System.Collections.Generic;

public class ActorBase : FlyAction
{
    public int m_Hp = 100;
    public int HP
    {
        get
        {
            return m_Hp;
        }
        set
        {
            m_Hp = value;
            if(bloodBar != null)
            {
                bloodBar.SetBloodBar(m_Hp / 100f);
            }
        }
    }
    public bool IsDead
    {
        get
        {
            return HP <= 0;
        }
    }

    public int actorID;
    public int station;
    public Pb.HeroAttrCS m_HeroAttr;
    public List<uint> own_skill_ids = new List<uint>();
    public Pb.TeamType battleTeam  = Pb.TeamType.CS_TEAM_BLUE;

    /// <summary>
    /// 位移异常状态(浮空等)
    /// </summary>
    public RoleOneState stateOne = RoleOneState.Normal;
    /// <summary>
    /// 异常状态（定身/控制等）
    /// </summary>
    public NegStateEnum stateNeg = NegStateEnum.Normal;

    private float m_PrePauseTimeScale = 1f;

    private List<SkillBase> playerSkills = new List<SkillBase>();
    private SkillBase currentSkill;
    private SkillBase nextSkill;
    private BaseSkillComplier skillComplier;

    /// <summary>
    /// 获得下一个普攻
    /// </summary>
    /// <returns></returns>
    public SkillBase GetNextSkill()
    {
        SkillBase sk = null;
        switch(RoundMgr.Instance.roundType)
        {
            case 0:
                sk = playerSkills.Find(delegate(SkillBase temp)
                {
                    return temp.skillCfg.skillType == SkillType.NORMAL && temp.player_skill_roundcd == 0;
                });
                break;
            case 1:
                sk = playerSkills.Find(delegate(SkillBase temp)
                {
                    return temp.skillCfg.skillType == SkillType.PASSIVE && temp.player_skill_roundcd == 0;
                });
                break;
            case 2:
                sk = playerSkills.Find(delegate(SkillBase temp)
                {
                    return temp.skillCfg.skillType == SkillType.ESOTERIC && temp.is_EsoSkill_Ready;
                });
                break;
        }
        return sk;
    }

    public bool IsResetPosition { get { return m_Ori_Pos == position; } }
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    private SpriteRenderer spriteRender;

    public bool IsActorReset
    {
        get
        {
            bool skillReset = true;
            for(int i = 0; i < playerSkills.Count; i++)
            {
                if(playerSkills[i].is_Using_Skill)
                    skillReset = false;
            }
            return IsDead || IsResetPosition && skillComplier.IsAnimClip(SkillManager.idle) && skillReset;
        }
    }

    private int m_SortingOrder;
    private Vector3 m_Ori_Pos;
    private Vector3 m_Fly_Pos;

    public Transform shadow;
    public Transform model;
    public Transform bloodTrans;
    public BloodBar bloodBar;

    protected void Start()
    {
        playerSkills.Clear();
        for(int i = 0; i < own_skill_ids.Count; i++)
        {
            string skill = "skill-" + own_skill_ids[i];
            GameObject go = ObjectsManager.CreateGameObject(BundleBelong.prefab, skill, false);
            if (go != null)
            {
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;
                SkillBase ps = new SkillBase();
                ps.player_skill_id = own_skill_ids[i];
                ps.skillCfg = go.GetComponent<SkillConfig>();
                ps.Sender = this;
                ps.player_skill_roundcd = 0;
                ps.is_Using_Skill = false;
                playerSkills.Add(ps);
            }
        }

        m_Ori_Pos = position;
        if (skillComplier == null) skillComplier = GetComponent<BaseSkillComplier>();
        if (spriteRender == null) spriteRender = GetComponentInChildren<SpriteRenderer>();
        m_SortingOrder = spriteRender.sortingOrder;

        stateOne = RoleOneState.Normal;

        Notify.Event.register((int)BattleCmd.MSGID_BATTLEROUND, this, EvtBattleRound);
    }

    private void EvtBattleRound(Notify.Args e)
    {
        for (int i = 0; i < playerSkills.Count; i++)
        {
            playerSkills[i].SkillRound();
        }
        nextSkill = GetNextSkill();
    }

    public void Skill()
    {
        if(nextSkill == null)
        {
            //Debug.Log("释放被动技能:" + name);
            return;
        }
        //Debug.Log("释放技能:" + nextSkill.skillCfg.skillType + " " + name);

        currentSkill = nextSkill;
        skillComplier.onSkill(currentSkill);
        currentSkill.player_skill_roundcd = currentSkill.skillCfg.skillCD;
    }

    public void SkillComplete()
    {
        Reset();
        if (currentSkill.skillCfg.skillType == SkillType.CHASE ||
            currentSkill.skillCfg.skillType == SkillType.ESOTERIC)
        {
            RoundMgr.Instance.ShowMask(false);
        }
    }

    /// <summary>
    /// 追打技能判定
    /// </summary>
    public bool ChaseSkill()
    {
        if(IsDead || RoundMgr.Instance.m_IsGameOver) 
            return false;

        for (int i = 0, count = playerSkills.Count; i < count; i++)
        {
            if (playerSkills[i].skillCfg.skillType != SkillType.CHASE)
                continue;

            //追打条件
            if (playerSkills[i].IsCanUseChaseSkill())
            {
                currentSkill = playerSkills[i];

                RoundMgr.Instance.BattlePlayersPuase(true);
                PlayerTimePause(false);
                PlayerHighOutLine(true);

                skillComplier.onSkill(currentSkill);
                currentSkill.player_skill_roundcd = currentSkill.skillCfg.skillCD;
                return true;
            }
        }
        return false;
    }

    public bool DetectEstSkillCD(SkillType type)
    {
        foreach(SkillBase sb in playerSkills)
        {
            if(sb.skillCfg.skillType == type)
            {
                return !sb.IsSkillInCD;
            }
        }
        return false;
    }

    /// <summary>
    /// 使用奥义大招
    /// </summary>
    public bool UseEstoericSkill()
    {
        for(int i = 0, count = playerSkills.Count; i < count; i++)
        {
            //追打条件
            if(playerSkills[i].skillCfg.skillType == SkillType.ESOTERIC)
            {
                if(playerSkills[i].player_skill_roundcd == 0)
                {
                    playerSkills[i].is_EsoSkill_Ready = true;
                    return true;
                }
            }
        }
        return false;
    }

    private float flow_time;
    protected override void Update()
    {
        base.Update();

        //追打技能判定放在这里
        ChaseSkill();

        //更新角色状态
        switch (stateOne)
        {
            case RoleOneState.Normal: flow_time = m_Time; break;
            case RoleOneState.NoNormal:
                if (IsDead)
                {
                    stateOne = RoleOneState.Normal;
                    break;
                }
                FlyDirect(m_Time, transform.position, m_Ori_Pos, BattleCfg.SPEED_BACK_RESET, delegate()
                {
                    Reset();
                    flow_time = m_Time;
                });
                break;
            case RoleOneState.FloatingUp:
                FlyLerpMin(m_DeltaScaleTime, transform.position, m_Fly_Pos, BattleCfg.SPEED_UP_HEAVY, delegate()
                {
                    transform.position = m_Fly_Pos;
                    stateOne = RoleOneState.FloatingTop;
                    //PlayAnimation(SkillManager.floatingTop);
                    flow_time = m_Time;
                });
                break;
            case RoleOneState.FloatingTop:
                if(m_Time - flow_time > BattleCfg.TIME_FLOATING)
                {
                    stateOne = RoleOneState.FlyDownFree;
                    PlayAnimation(SkillManager.floatingDown);
                    m_Fly_Pos = transform.position; m_Fly_Pos.y = m_Ori_Pos.y;
                    flow_time = m_Time;
                }
                break;
            case RoleOneState.Fly:
                FlyDirect(m_Time, transform.position, m_Fly_Pos, BattleCfg.SPEED_UP_HEAVY, delegate()
                {
                    transform.position = m_Fly_Pos;
                    //enabled = false;
                    stateOne = RoleOneState.FlyDownFree;
                    PlayAnimation(SkillManager.floatingDown);
                    flow_time = m_Time;
                });
                break;
            case RoleOneState.FlyDownFree:
                FlyFreeForce(m_Time, m_DeltaTime, m_Fly_Pos.y, -force, BattleCfg.SPEED_DOWN_LIGHT, delegate()
                {
                    transform.position = m_Fly_Pos;
                    //enabled = false;
                    stateOne = RoleOneState.FallGround;
                    m_Fly_Pos = transform.position; m_Fly_Pos.y += BattleCfg.HEIGHT_BOUNCE_LIGHT;
                    PlayAnimation(SkillManager.falling);
                    flow_time = m_Time;
                });
                break;
            case RoleOneState.FlyDownForce:
                FlyDirect(m_Time, transform.position, m_Fly_Pos, BattleCfg.SPEED_DOWN_HEAVY, delegate()
                {
                    transform.position = m_Fly_Pos;
                    //enabled = false;
                    stateOne = RoleOneState.FallGround;
                    m_Fly_Pos = transform.position; m_Fly_Pos.y += BattleCfg.HEIGHT_BOUNCE_HEAVY;
                    PlayAnimation(SkillManager.falling);
                    flow_time = m_Time;
                });
                break;
            case RoleOneState.FallGround:
                if(IsDead)
                {
                    stateOne = RoleOneState.Normal;
                    break;
                }
                //落地弹两下
                FlyLerpMin(m_DeltaScaleTime, transform.position, m_Fly_Pos, BattleCfg.SPEED_DOWN_HEAVY, delegate()
                {
                    m_Fly_Pos = transform.position; m_Fly_Pos.y = m_Ori_Pos.y;
                    FlyFreeForce(m_Time, m_DeltaTime, m_Fly_Pos.y, -force, BattleCfg.SPEED_DOWN_HEAVY, delegate()
                    {
                        transform.position = m_Fly_Pos;
                        stateOne = RoleOneState.GetUp;
                        PlayAnimation(SkillManager.getup);
                        flow_time = m_Time;
                    },1);
                });
                break;
            case RoleOneState.GetUp:
                if(m_Time - flow_time > 1f)
                {
                    if(IsResetPosition)
                    {
                        Reset();
                    }
                    else
                    {
                        stateOne = RoleOneState.NoNormal;
                        PlayAnimation(SkillManager.run);
                    }
                    flow_time = m_Time;
                }
                break;
            case RoleOneState.GoBack:
                FlyLerp(m_DeltaScaleTime, transform.position, m_Fly_Pos, BattleCfg.SPEED_BACK, delegate()
                {
                    transform.position = m_Fly_Pos;
                    //enabled = false;
                    flow_time = m_Time;
                    stateOne = RoleOneState.GoBackOver;
                });
                
                break;
            case RoleOneState.GoBackOver:
                if(m_Time - flow_time > 0.5f)
                {
                    stateOne = RoleOneState.NoNormal;
                    PlayAnimation(SkillManager.run);
                    flow_time = m_Time;
                }
                break;
        }
    }

    /// <summary>
    /// 造成伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public int HurtDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            RoundMgr.Instance.ActorDie(this);
            PlayerTimePause(false);
            PlayAnimation(SkillManager.dead);
            PlayerHighOutLine(false);
        }
        GameObject go = ObjectsManager.CreateGameObject(BundleBelong.prefab, "bloodtext", true);
        if(go !=null)
        {
            go.GetComponent<BloodText>().SetBloodText(damage, transform);
        }
        return damage;
    }
    /// <summary>
    /// 造成buff或debuff
    /// </summary>
    /// <param name="buffID"></param>
    public void HurtBuffOrDeBuff(string buffID)
    {
        GameObject buff = ObjectsManager.CreateGameObject(BundleBelong.buff, buffID, true);
        if (buff == null)
        {
            Debug.LogError(string.Format("buff: buffID-({0}) == null", buffID));
            return;
        }
        BaseBuff bb = buff.GetComponent<BaseBuff>();
        if (bb == null)
        {
            Debug.LogError(string.Format("script: BaseBuff-({0}) == null", buffID));
            return;
        }
        bb.BuffBegin(this);
    }
    /// <summary>
    /// 造成一次位移（浮空 击退 击倒 击落 击飞）
    /// </summary>
    /// <param name="hitState"></param>
    public void HurtAttachOne(AttackAttachOneState hitState)
    {
        switch (hitState)
        {
            case AttackAttachOneState.Normal:
                //m_BeHitState = RoleState.Normal;
                PlayAnimation(SkillManager.hurt);
                break;
            case AttackAttachOneState.HitFloating:
                m_Fly_Pos = transform.position; m_Fly_Pos.y = m_Ori_Pos.y + BattleCfg.HEIGHT_FLOATING_HEAVY;
                stateOne = RoleOneState.FloatingUp;
                PlayAnimation(SkillManager.floatingUp);
                break;
            case AttackAttachOneState.HitFly:
                m_Fly_Pos = transform.position + new Vector3(battleTeam == Pb.TeamType.CS_TEAM_BLUE ? -15 : 15, BattleCfg.HEIGHT_FLOATING_LIGHT, 0);
                stateOne = RoleOneState.Fly;
                PlayAnimation(SkillManager.floatingUp);
                break;
            case AttackAttachOneState.HitBack:
                stateOne = RoleOneState.GoBack;
                m_Fly_Pos = transform.position; m_Fly_Pos.x += battleTeam == Pb.TeamType.CS_TEAM_BLUE ? -BattleCfg.DISTANCE_BACK_HEAVY : BattleCfg.DISTANCE_BACK_HEAVY;
                PlayAnimation(SkillManager.hurt);
                break;
            case AttackAttachOneState.HitFall:
                m_Fly_Pos = transform.position + new Vector3(battleTeam == Pb.TeamType.CS_TEAM_BLUE ? -5f : 5f, 0, 0);
                m_Fly_Pos.y = m_Ori_Pos.y;
                stateOne = RoleOneState.FlyDownForce;
                PlayAnimation(SkillManager.hitFall);
                BattleCamera.Instance.Shake();
                break;
            case AttackAttachOneState.HitDownGround:
                //todo...加入判断
                stateOne = RoleOneState.FallGround;
                PlayAnimation(SkillManager.hitFall);
                break;
            default:
                PlayAnimation(SkillManager.hurt);
                break;
        }
        RoundMgr.Instance.BattlePlayersPuase(false);
    }
    /// <summary>
    /// 造成异常状态(定身 点穴 等)
    /// </summary>
    /// <param name="negState"></param>
    public void HurtAddNeg(NegStateEnum negState)
    {
        if (HP <= 0) return;

        if (!FNegEnum.IsContain(stateNeg, negState))
        {
            FNegEnum.Add(ref stateNeg, negState);
        }
    }
    /// <summary>
    /// 移除异常状态(定身 点穴 等)
    /// </summary>
    /// <param name="negState"></param>
    public void HurtSubNeg(NegStateEnum negState)
    {
        if (FNegEnum.IsContain(stateNeg, negState))
        {
            FNegEnum.Remove(ref stateNeg, negState);
        }
    }
    /// <summary>
    /// 角色是否暂停
    /// </summary>
    /// <param name="pause"></param>
    public void PlayerTimePause(bool pause)
    {
        if (pause)
        {
            m_PrePauseTimeScale = timeScale;
        }
        TimeBase[] tb = transform.GetComponents<TimeBase>();
        for (int i = 0; i < tb.Length; i++)
        {
            tb[i].SetTimeScale(pause ? 0 : m_PrePauseTimeScale);
        }
    }
    /// <summary>
    /// 角色是否高亮显示
    /// </summary>
    /// <param name="highOutLine"></param>
    public void PlayerHighOutLine(bool highOutLine)
    {
        spriteRender.sortingOrder = highOutLine ? SkillManager.outSortingOrder : m_SortingOrder;
    }
    public void PlayAnimation(string clipname)
    {
        if (!IsDead || clipname == SkillManager.dead)
        {
            skillComplier.PlayAnimation(clipname);
        }

    }
    /// <summary>
    /// 状态重置
    /// </summary>
    public void Reset()
    {
        PlayAnimation(SkillManager.idle);
        stateOne = RoleOneState.Normal;
        position = m_Ori_Pos;
        PlayerTimePause(false);
        PlayerHighOutLine(false);
    }
}
