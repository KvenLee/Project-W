/*---------------------------------------------------------------------------------------------------------------------
*--  文件名：   
*--  创建人:    vekcon.zx
*--    日期:    2016-6-1
*--    描述：   
*--------------------------------------------------------------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseSkillComplier : FlyAction
{
    #region cached Skill Data
    private Animation m_Animation;
    private Animator m_Animator;

    private int m_HitEvtIndex = 0;          //当前打击事件
    private float m_PreNormTime;        //上一帧动画时间

    public bool is_Using_Skill;

    private ActorBase m_Sender;
    private SkillBase m_CurSkill;
    private SkillConfig m_CurSkillCfg;
    private List<ActorBase> m_Targets = new List<ActorBase>();

    private AnimationState state;
    private Vector3 m_IdlePos;
    private Vector3 m_HitPos;
    private int m_ExcuteCount;              //该技能执行次数
    private float m_Ori_Anim_Speed;
    private float m_Cur_Anim_Speed;

    public AnimatorStateInfo stateinfo { get; set; }

    #endregion

    void Start()
    {
        if (m_Animation == null)
            m_Animation = GetComponentInChildren<Animation>();
        if (m_Animator == null)
            m_Animator = GetComponentInChildren<Animator>();

        if (m_Sender == null)
            m_Sender = transform.GetComponent<ActorBase>();
        m_IdlePos = m_Sender.position;
        m_Ori_Anim_Speed = m_Animator.speed;
        m_Cur_Anim_Speed = m_Animator.speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (m_Animator != null)
        {
            stateinfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_Animator.speed = m_Cur_Anim_Speed * timeScale;
        }
    }

    public void onSkill(SkillBase _skill)
    {
        if (m_CurSkill != null && m_CurSkill != _skill)
        {
            m_CurSkill.SkillEnd();
        }

        m_CurSkill = _skill;
        m_CurSkillCfg = _skill.skillCfg;
        m_Targets = m_CurSkill.SkillTargets;
        is_Using_Skill = true;
        m_CurSkill.SkillBegin();

        //Debug.Log(string.Format("<SKILL>SkillName:{0} sender:{1} targets(count):{2}", m_CurSkillCfg, this, m_Targets.Count));

        //追击技能和奥义大招目标高亮显示
        if (m_CurSkillCfg.skillType == SkillType.CHASE || m_CurSkillCfg.skillType == SkillType.ESOTERIC)
        {
            //开启黑色遮罩
            RoundMgr.Instance.ShowMask(true);

            for (int i = 0; i < m_Targets.Count; i++)
            {
                m_Targets[i].PlayerHighOutLine(true);
            }
        }

        m_ExcuteCount = m_CurSkillCfg.excuteCount;
        m_HitPos = m_CurSkill.hitPos;

        if (m_Animator != null)
        {
            m_Animator.speed = m_CurSkillCfg.aniSpeed;
        }
        if (m_Animation != null)
        {
            state = m_Animation[m_CurSkillCfg.attack];
            state.speed = m_CurSkillCfg.aniSpeed;
        }
        m_Ori_Anim_Speed = m_CurSkillCfg.aniSpeed;

        StopCoroutine("onSkillExcute");
        StartCoroutine("onSkillExcute");
    }

    private IEnumerator onSkillExcute()
    {
        m_ExcuteCount--;

        if (!string.IsNullOrEmpty(m_CurSkillCfg.preAttack))
        {
            PlayAnimation(m_CurSkillCfg.preAttack);
            _timer = m_Time;
            while (m_Time - _timer < 1f)     //注意：这里的等待时间需要大于preAttack动画时间
            {
                yield return null;
            }
        }
        if (m_CurSkillCfg.attackRange <= 0)
        {
            PlayAnimation(SkillManager.run);
            while (updateMoveTo/*updateJumpTo*/(transform.position, m_HitPos) == false)
            {
                yield return null;
            }
        }
        else
        {
            _timer = m_Time;
            while (m_Time - _timer < 0.5f)  //远程攻击技能起手0.5s延迟
            {
                yield return null;
            }
        }
        yield return null;

        m_PreNormTime = 0;
        if (!string.IsNullOrEmpty(m_CurSkillCfg.attack))
        {
            PlayAnimation(m_CurSkillCfg.attack);
        }
        InitAnimEvent();
        while (updateAni() == false)
        {
            yield return null;
        }
        m_CurSkill.SkillEnd();

        //角色静止
        _timer = m_Time;
        while (m_Time - _timer < 0.3f)
        {
            yield return null;
        }

        if (m_ExcuteCount > 0)
        {
            yield return null;
            StopCoroutine("onSkillExcute");
            yield return StartCoroutine("onSkillExcute");
        }
        else
        {
//             _timer = m_Time;
//             while (m_Time - _timer < 0.1f)
//             {
//                 yield return null;
//             }

            if (m_CurSkillCfg.attackRange <= 0)
            {
                PlayAnimation(SkillManager.run);
                while (/*updateMoveTo*/updateJumpTo(transform.position, m_IdlePos) == false)
                {
                    yield return null;
                }
            }
            onSkillComplete();
            is_Using_Skill = false;
        }
    }
    private void onSkillComplete()
    {
        m_Sender.SkillComplete();
    }

    bool updateMoveTo(Vector3 begin, Vector3 end)
    {
        //近战才需要跑到敌人面前！！
        if (m_CurSkillCfg.attackRange <= 0)
        {
            bool init = false;
            FlyDirect(m_Time, begin, end, m_CurSkillCfg.moveSpeed, delegate()
            {
                transform.position = end;
                //enabled = false;
                init = true;
            });
            return init;
        }
        return true;
    }

    protected float jumpY = 0.5f;
    bool updateJumpTo(Vector3 begin, Vector3 end)
    {
        //近战才需要跑到敌人面前！！
        if (m_CurSkillCfg.attackRange <= 0)
        {
            bool init = false;
            FlySphere(m_Time, begin, end, m_CurSkillCfg.moveSpeed, jumpY, delegate()
            {
                transform.position = end;
                //enabled = false;
                init = true;
            });
            return init;
        }
        return true;
    }
    void InitAnimEvent()
    {
        if(!string.IsNullOrEmpty(m_CurSkillCfg.animFx))
        {
            GameObject fx = ObjectsManager.CreateGameObject(BundleBelong.effect, m_CurSkillCfg.animFx, true);
            if(fx)
            {
                fx.transform.position = m_Sender.position;
            }
        }
        m_HitEvtIndex = 0;
    }

    bool updateAni()
    {
        if (m_CurSkillCfg == null)
            return true;

        if (m_HitEvtIndex < m_CurSkillCfg.animEvts.Length)
        {
            float normTime = 0;
            if (m_Animation != null)
            {
                normTime = state.normalizedTime;
            }
            if (m_Animator != null)
            {
                if (!stateinfo.IsName(m_CurSkillCfg.attack))
                {
                    //                     while(m_Time - _timer > 0.5f)
                    //                     {
                    //                         _timer = m_Time;
                    //                         PlayAnimation(m_CurSkillCfg.attack);
                    //                     }
                    return false;
                }
                normTime = stateinfo.normalizedTime - (int)stateinfo.normalizedTime;      //取小数
            }
            float currTime = m_CurSkillCfg.animEvts[m_HitEvtIndex].hitTime;
            if (normTime < m_PreNormTime || (m_PreNormTime < currTime && currTime <= normTime))
            {
                if (m_CurSkillCfg.animEvts[m_HitEvtIndex].resetAnim)
                {
                    PlayAnimation(SkillManager.idle);
                    m_PreNormTime = normTime;
                    return true;
                }
                else
                {
                    OnHit(m_Targets,
                        m_CurSkillCfg.animEvts[m_HitEvtIndex].damage,
                        m_CurSkillCfg.animEvts[m_HitEvtIndex].attachState
                        , m_CurSkillCfg.animEvts[m_HitEvtIndex].buffs);

                    //播放音效
                    PlayerAudio(m_CurSkillCfg.animEvts[m_HitEvtIndex].audioClip);
                }
                m_HitEvtIndex++;
            }
            m_PreNormTime = normTime;
            return false;
        }
        else
        {
            return true;
        }
    }

    //=============================Interface

    public void AnimSpeedChange(float changeRate)
    {
        m_Cur_Anim_Speed = m_Ori_Anim_Speed * changeRate;
    }
    public bool IsAnimClip(string clipname)
    {
        return stateinfo.IsName(clipname);
    }

    public void PlayAnimation(string clipname)
    {
        if (!string.IsNullOrEmpty(clipname))
        {
            if (m_Animator != null)
            {
                if (!stateinfo.IsName(clipname))
                {
                    m_Animator.CrossFadeInFixedTime(clipname, 0.1f * timeScale);
                }
            }
            if (m_Animation != null)
            {
                m_Animation.CrossFade(clipname, 0.1f);
            }
        }
    }

    public void PlayerAudio(string audio_name,float volume = 1f)
    {
        if (string.IsNullOrEmpty(audio_name))
            return;
        AudioClip clip = ObjectsManager.LoadObject<AudioClip>(BundleBelong.audio, audio_name);
        if (clip == null)
            return;

        AudioSource source = GetComponent<AudioSource>();
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.priority = 50;
            source.pitch = 1;
        }
        source.PlayOneShot(clip, volume);
    }

    void OnHit(List<ActorBase> target, int damage, AttackAttachOneState attachState, string[] buffIDs)
    {
        for (int i = 0, count = target.Count; i < count; i++)
        {
            target[i].HurtDamage(damage);
            target[i].HurtAttachOne(attachState);

            if (buffIDs != null)
            {
                for (int j = 0; j < buffIDs.Length; j++)
                {
                    target[i].HurtBuffOrDeBuff(buffIDs[j]);
                }
            }
        }
    }
    #region DELETED

    void AnimaAddEvent()
    {
        AnimationEvent _event = new AnimationEvent();
        for (int i = 0; i < m_CurSkillCfg.animEvts.Length; i++)
        {
            _event.time = state.length * m_CurSkillCfg.animEvts[i].hitTime;
            _event.functionName = "onskill";
            _event.intParameter = i;
            state.clip.AddEvent(_event);
        }
    }

    public void onSkillHit(int index)
    {
        if (m_CurSkillCfg.animEvts[index].resetAnim)
        {
            PlayAnimation(SkillManager.idle);
        }
        else
            Debug.Log("Hit " + state.normalizedTime);
    }

    #endregion
}
