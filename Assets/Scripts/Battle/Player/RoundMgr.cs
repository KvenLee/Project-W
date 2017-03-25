using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 职能：
/// 1、决定哪个角色的回合
/// 2、决定哪个角色追打
/// 3、管理角色
/// </summary>
public class RoundMgr : TUSingleton<RoundMgr>
{
    public GameObject mask;

    public bool m_IsPuasing = false;
    public bool m_IsGameOver = false;
    private int turn = 0;
    public bool m_IsAutoBat = false;

    public ActorBase curPlayer;
    /// <summary>
    /// 回合类型
    /// 0：普攻回合
    /// 1：被动回合
    /// 2：奥义回合
    /// </summary>
    public int roundType;

    public Dictionary<Pb.TeamType, List<ActorBase>> m_TeamActor = new Dictionary<Pb.TeamType, List<ActorBase>>();
    private List<ActorBase> m_BatActorLst = new List<ActorBase>();

    public SpriteRenderer battleBg;
    public Transform actorRoot1;
    public Transform actorRoot2;
    public List<Vector3 > stationDic = new List<Vector3>();

    [ContextMenu("Test")]
    void SetPos()
    {
        stationDic.Clear();
        foreach(Transform  tr in actorRoot1)
        {
            stationDic.Add(tr.localPosition);
        }
    }

    public void InitTeam(ActorBase player)
    {
        List<ActorBase> teamType = null;
        if(m_TeamActor.TryGetValue(player.battleTeam, out teamType))
        {
            if(teamType.Contains(player))
                return;
            teamType.Add(player);
        }
        else
        {
            m_TeamActor.Add(player.battleTeam, new List<ActorBase>()
            {
                player
            });
        }
    }

    public void ActorDie(ActorBase actor)
    {
        List<ActorBase> teamType = null;
        if(m_TeamActor.TryGetValue(actor.battleTeam, out teamType))
        {
            int index1 = teamType.FindIndex(delegate(ActorBase temp)
            {
                return temp.m_HeroAttr.hero_id == actor.m_HeroAttr.hero_id;
            });
            if(index1 != -1)
            {
                teamType.RemoveAt(index1);
            }
        }
        int index = m_BatActorLst.FindIndex(delegate(ActorBase temp)
        {
            return temp.battleTeam == actor.battleTeam && temp.actorID == actor.actorID;
        });
        if(index != -1)
        {
            m_BatActorLst.RemoveAt(index);
        }
    }

    public IEnumerator JudgeLogicUpdate()
    {
        ShowText();
        while(true)
        {
            if(IsGameOver())
            {
                Debug.Log("Game Over!");
                yield return new WaitForSeconds(1f);
                PreBattleSys.Instance.BattleOver();
                yield break;
            }
            yield return null;

            if(m_BatActorLst.Count > 0)
            {
                while(PlayerAllReset() == false)
                    yield return null;

                yield return new WaitForSeconds(0.1f);
                if(!IsGameOver())
                {
                    GetRoundPlayer().Skill();
                }
            }
        }
    }

    bool IsGameOver()
    {
        if(m_IsGameOver == false)
        {
            m_IsGameOver = m_TeamActor[Pb.TeamType.CS_TEAM_BLUE].Count == 0 || m_TeamActor[Pb.TeamType.CS_TEAM_RED].Count == 0;
        }
        return m_IsGameOver;
    }

    ActorBase GetRoundPlayer()
    {
        //下一回合
        if(curPlayer == null)
        {
            curPlayer = m_BatActorLst[0];
            return curPlayer;
        }
        int index = m_BatActorLst.FindIndex(delegate(ActorBase temp)
        {
            return curPlayer == temp;
        });
        if(index == m_BatActorLst.Count - 1)
        {
            ShowText();
            curPlayer = m_BatActorLst[0];
        }
        else
        {
            curPlayer = m_BatActorLst[index + 1];
        }
        return curPlayer;
    }

    public List<ActorBase> GetActorByTeam(Pb.TeamType team)
    {
        List<ActorBase> bp;
        if(m_TeamActor.TryGetValue(team, out bp))
        {
            return bp;
        }
        return null;
    }


    //===================

    public void CacheBattlePlayers()
    {
        m_BatActorLst.Clear();
        foreach(KeyValuePair<Pb.TeamType, List<ActorBase>> kv in m_TeamActor)
        {
            m_BatActorLst.AddRange(kv.Value);
        }
        m_BatActorLst.Sort(CompareSort);
    }
    private int CompareSort(ActorBase x, ActorBase y)
    {
        //先按行动值大小，再按编队，最后按职业
        if(x.m_HeroAttr.moveValue == y.m_HeroAttr.moveValue)
        {
            if(x.battleTeam != y.battleTeam)
                return x.battleTeam.CompareTo(y.battleTeam);
            return y.m_HeroAttr.profession.CompareTo(x.m_HeroAttr.profession);
        }
        return y.m_HeroAttr.moveValue.CompareTo(x.m_HeroAttr.moveValue);
    }

    bool PlayerAllReset()
    {
        for(int i = 0, count = m_BatActorLst.Count; i < count; i++)
        {
            if(m_BatActorLst[i].IsActorReset == false)
                return false;
        }
        return true;
    }

    public void BattlePlayersPuase(bool pause)
    {
        if(!m_IsPuasing && !pause) return;
        m_IsPuasing = pause;

        for(int i = 0, count = m_BatActorLst.Count; i < count; i++)
        {
            m_BatActorLst[i].PlayerTimePause(pause);
        }
    }
    
    void ShowText()
    {
        if(BattleSys.Instance.CtrlUI.m_LabRound != null)
        {
            turn += 1;
            int round = turn / 3;
            roundType = turn % 3;
            switch(roundType)
            {
                case 0:
                    BattleSys.Instance.CtrlUI.m_LabRound.text = string.Format("第{0}回合", round);
                    break;
                case 1:
                    BattleSys.Instance.CtrlUI.m_LabRound.text = string.Format("被动{0}回合", round + 1);
                    break;
                case 2:
                    BattleSys.Instance.CtrlUI.m_LabRound.text = string.Format("奥义{0}回合", round + 1);
                    break;
            }
        }
        //turn>1表示开始战斗
        if(turn > 0)
            Notify.Event.fire((int)BattleCmd.MSGID_BATTLEROUND, null);
    }

    private int m_MaskCount;
    public void ShowMask(bool show)
    {
        m_MaskCount += show ? 1 : -1;
        mask.SetActive(m_MaskCount > 0);
        //BattleSys.Instance.CtrlUI.mask.SetActive(m_MaskCount > 0);
    }
}