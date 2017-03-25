using System.Collections.Generic;

public class SkillManager
{
#region normal animations
    public static string idle_shang = "idle_shang";
    public static string idle_xia = "idle_xia";
    public static string idle_zuo = "idle_zuo";
    public static string idle_you = "idle_you";
    public static string idle_zuoshang = "idle_zuoshang";
    public static string idle_zuoxia = "idle_zuoxia";
    public static string idle_youshang = "idle_youshang";
    public static string idle_youxia = "idle_youxia";

    public static string run_shang = "run_shang";
    public static string run_xia = "run_xia";
    public static string run_zuo = "run_zuo";
    public static string run_you = "run_you";
    public static string run_zuoshang = "run_zuoshang";
    public static string run_zuoxia = "run_zuoxia";
    public static string run_youshang = "run_youshang";
    public static string run_youxia = "run_youxia";

    public static string collect_shang = "collect_shang";
    public static string collect_xia = "collect_xia";
    public static string collect_zuo = "collect_zuo";
    public static string collect_you = "collect_you";
    public static string collect_zuoshang = "collect_zuoshang";
    public static string collect_zuoxia = "collect_zuoxia";
    public static string collect_youshang = "collect_youshang";
    public static string collect_youxia = "collect_youxia";

    public static string mine_shang = "mine_shang";
    public static string mine_xia = "mine_xia";
    public static string mine_zuo = "mine_zuo";
    public static string mine_you = "mine_you";
    public static string mine_zuoshang = "mine_zuoshang";
    public static string mine_zuoxia = "mine_zuoxia";
    public static string mine_youshang = "mine_youshang";
    public static string mine_youxia = "mine_youxia";
#endregion

#region battle animations
    public static string idle = "idle";
    public static string run = "run";
    public static string hurt = "hurt";
    public static string miss = "miss";
    public static string dead = "dead";
    public static string win = "win";
    public static string specialIdle = "specialIdle";

    public static string floatingUp = "floatingUp";//"jifei";
    public static string floatingTop = "floatingTop";//"jifei";
    public static string floatingDown = "floatingDown";//"jifei";

    public static string hitFall = "hitFall";//"jidao";
    public static string falling = "falling";//"jidao";
    public static string fall = "fall";//"jidao";
    public static string getup = "getup";//"jidao";

    public static string attack1 = "attack1";
    public static string attack2 = "attack2";
    public static string attack3 = "attack3";

    public static string skill1 = "skill1";
    public static string skill2 = "skill2";
    public static string skill3 = "skill3";
    public static string skill4 = "skill4";
    public static string skill5 = "skill5";
#endregion

    public static int maskSortingOrder = 10;
    public static int outSortingOrder = 20;

    public static Dictionary <string,int> animTransIdDic = new Dictionary<string, int>
    {
        {idle,0},{run,1},{hurt,2},{miss,3},{dead,4},{specialIdle,14},
        {attack1,5},{attack2,6},{attack3,7},
        {skill1,8},{skill2,9},{skill3,10},{skill4,11},{skill5,12},
        {win,13},
        {floatingUp,120}, {floatingTop,121}, {floatingDown,122}, {hitFall,123}, {falling,124}, {fall,125}, {getup,126},

        {idle_shang,100}, {idle_xia,101}, {idle_zuo,102}, {idle_you,103}, {idle_zuoshang,104}, {idle_zuoxia,105}, {idle_youshang,106}, {idle_youxia,107},
        {run_shang,110}, {run_xia,111}, {run_zuo,112}, {run_you,113}, {run_zuoshang,114}, {run_zuoxia,115}, {run_youshang,116}, {run_youxia,117},
    };
}
