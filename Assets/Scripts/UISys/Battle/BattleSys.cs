using Pb;
using Res_Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSys : GameSys<BattleSys>
{
    //Data
    public int map_id;
    public CSTeam team;

    private string prefab = "UIBattle";
    public BattleUICtrl CtrlUI { get; private set; }

    private Dictionary<int ,BattleHeroUICtrl> batHeroUIDic = new Dictionary<int, BattleHeroUICtrl>();

    public override void SysEnter()
    {
        if(CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<BattleUICtrl>(prefab);
        }
        if(CtrlUI != null)
        {
            //EventDelegate.Add(CtrlUI.m_EvtClick1.onClick, HeroHeadClick);
            StartCoroutine(LoadScene());
        }
        base.SysEnter();
    }

    public override void SysUpdate()
    {
        base.SysUpdate();
        if(batHeroUIDic.Count > 0)
        {
            foreach(ActorBase ab in RoundMgr.Instance.m_TeamActor[Pb.TeamType.CS_TEAM_BLUE])
            {
                batHeroUIDic[ab.actorID].IsCanUse = ab.DetectEstSkillCD(SkillType.ESOTERIC);
            }
        }
    }
    public override void SysLeave()
    {
        if (CtrlUI != null)
        {
            CtrlUI.gameObject.SetActive(false);
            CtrlUI = null;
        }
        batHeroUIDic.Clear();
        base.SysLeave();
    }

    private void HeroHeadClick(int actorID)
    {
        bool use_ok = false;
        //int actorID = ob.GetComponent<BattleHeroUICtrl>().actor_id;

        foreach(ActorBase ab in RoundMgr.Instance.m_TeamActor[Pb.TeamType.CS_TEAM_BLUE])
        {
            if(ab.actorID == actorID)
            {
                use_ok = ab.UseEstoericSkill();
                break;
            }
        }
        if(use_ok)
        {
            //ob
        }
        //ob.GetComponent<ActorBase>().
    }

    public IEnumerator LoadScene()
    {
        //1、load sceneve
        GameObject go = ObjectsManager.CreateGameObject(BundleBelong.map, "BattleScene", false);

        yield return null;

        //2、load players and init RoundMgr
        SetDemmyHeroData();
        if(team != null)
        {
            team.team_player_blue.Sort(ActorSort);
            team.team_player_red.Sort(ActorSort);
            for(int i = 0; i < team.team_player_blue.Count; i++)
            {
                BuildBattleActor(team.team_player_blue[i], TeamType.CS_TEAM_BLUE, i + 1);
                yield return null;
            }
            for(int i = 0; i < team.team_player_red.Count; i++)
            {
                BuildBattleActor(team.team_player_red[i], TeamType.CS_TEAM_RED, i + 1);
                yield return null;
            }
        }
        RoundMgr.Instance.CacheBattlePlayers();

        //3、load prefabs(skills and buff)


        //4.load ui
        if(CtrlUI != null)
        {
            batHeroUIDic.Clear();
            CtrlUI.gridHelper.BindDataSource(team.team_player_blue.Count, delegate(Transform trans, int dataIndex)
            {
                BattleHeroUICtrl bhctrl =  trans.GetComponent<BattleHeroUICtrl>();
                if(bhctrl != null)
                {
                    int actorid = team.team_player_blue[dataIndex].player_id;
                    ResHeroInfo heroInfo = TableMgr.Instance.GetRecord<ResHeroInfo>(TableMgr.cIdxHero, actorid);
                    string icon = CUility.PbBytes2String(heroInfo.head_icon);
                    bhctrl.icon.sprite = ObjectsManager.LoadObject<Sprite>(BundleBelong.headicon, icon);
                    bhctrl.gridHelper.BindDataSource(3, null);
                    bhctrl.actor_id = actorid;
                    bhctrl.pressAction = HeroHeadClick;
                    bhctrl.IsCanUse = false;

                    batHeroUIDic.Add(actorid, bhctrl);
                }
            });
        }

        //5、battle begin

        yield return new WaitForSeconds(1f);
        StartCoroutine(RoundMgr.Instance.JudgeLogicUpdate());
        yield return null;
    }

    int ActorSort(CSTeamPlayer p1,CSTeamPlayer p2)
    {
        return p1.player_station.CompareTo(p2.player_station);
    }

    private void BuildBattleActor(CSTeamPlayer player, TeamType team, int station)
    {
        string source_id = CUility.PbBytes2String(player.player_source_name);
        GameObject actor = ObjectsManager.CreateGameObject(BundleBelong.prefab, "BattleActorRoot", false);
        if(actor != null)
        {
            actor.name = "BattleActor_" + source_id;
            actor.transform.SetParent(team == TeamType.CS_TEAM_BLUE ? RoundMgr.Instance.actorRoot1 : RoundMgr.Instance.actorRoot2);
            actor.transform.localPosition = RoundMgr.Instance.stationDic[station];
            actor.transform.localEulerAngles = Vector3.zero;
            actor.transform.localScale = Vector3.one;

            ActorBase ab = actor.GetComponent<ActorBase>();
            ab.actorID = player.player_id;
            ab.station = station;
            ab.battleTeam = team;

            for(int i=0; i < player.player_skills.Count; i++)
            {
                ab.own_skill_ids.Add(player.player_skills[i].skill_id);
            }
                
            GameObject model = ObjectsManager.CreateGameObject(BundleBelong.prefab, source_id, false);
            if(model != null)
            {
                model.transform.SetParent(ab.model);
                model.transform.localPosition = Vector3.zero;
                model.transform.localScale = Vector3.one;
                model.transform.localEulerAngles = Vector3.one;
                model.GetComponent<SpriteRenderer>().sortingOrder = team == TeamType.CS_TEAM_BLUE ? 10 + station : station;
                model.GetComponent<Animator>().runtimeAnimatorController = ObjectsManager.LoadObject<RuntimeAnimatorController>(BundleBelong.animation, source_id + "@battle");
            }
            GameObject bloodbar = ObjectsManager.CreateGameObject(BundleBelong.prefab, "bloodbar", false);
            if(bloodbar != null)
            {
                bloodbar.SetActive(false);
                bloodbar.transform.SetParent(ab.bloodTrans);
                bloodbar.transform.localPosition = Vector3.zero;
                bloodbar.transform.localEulerAngles = Vector3.zero;
                bloodbar.transform.localScale = Vector3.one;
                ab.bloodBar = bloodbar.GetComponent<BloodBar>();
                ab.bloodBar.SetBloodBar(1);
            }
            RoundMgr.Instance.InitTeam(ab);
        }
    }

    //===========

    private void SetDemmyHeroData()
    {
        team = new CSTeam();
        List<CSTeamPlayer> players = new List<CSTeamPlayer>();
        CRecordTable table = TableMgr.Instance.GetTable(TableMgr.cIdxHero);
        for (int i = 0; i < table.Count(); i++)
        {
            if (players.Count == 4)
                break;
            ResHeroInfo heroInfo = table.GetRecord(i) as ResHeroInfo;
            if (heroInfo.id == 20002) continue;

            CSTeamPlayer cp = new CSTeamPlayer();
            cp.player_id = (int)heroInfo.id;
            cp.player_source_name = heroInfo.source_name;
            cp.player_station = heroInfo.station;
            for (int j = 0; j < heroInfo.skillLst.Count; j++)
            {
                CSPlayerSkill skill = new CSPlayerSkill();
                skill.skill_id = heroInfo.skillLst[j].id;
                cp.player_skills.Add(skill);
            }
            players.Add(cp);
        }
        team.team_player_blue.Clear();
        team.team_player_blue.AddRange(players);

        players.Clear();
        for (int i = table.Count() - 1; i >= 0; i--)
        {
            if (players.Count == 4)
                break;
            ResHeroInfo heroInfo = table.GetRecord(i) as ResHeroInfo;
            if (heroInfo.id == 20002) continue;

            CSTeamPlayer cp = new CSTeamPlayer();
            cp.player_id = (int)heroInfo.id;
            cp.player_source_name = heroInfo.source_name;
            cp.player_station = heroInfo.station;
            for(int j = 0; j < heroInfo.skillLst.Count; j++)
            {
                CSPlayerSkill skill = new CSPlayerSkill();
                skill.skill_id = heroInfo.skillLst[j].id;
                cp.player_skills.Add(skill);
            }
            players.Add(cp);
        }
        team.team_player_red.Clear();
        team.team_player_red.AddRange(players);
    }
}
