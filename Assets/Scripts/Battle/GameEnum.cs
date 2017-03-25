//------------------------------------------------------------------------------
// <GameEnum>
//
//     游戏的自定义Enum类型
//
// </GameEnum>
//------------------------------------------------------------------------------

public enum BattleCmd
{
    MSGID_BATTLEREADY = 1,
    MSGID_BATTLEROUND = 2,
}

/// <summary>
/// 技能类型
/// </summary>
public enum SkillType
{
    /// <summary>
    /// 普通攻击
    /// </summary>
    NORMAL,
    /// <summary>
    /// 被动技能
    /// </summary>
    PASSIVE,
    /// <summary>
    /// 追打技能
    /// </summary>
    CHASE,
    /// <summary>
    /// 奥义(大招)
    /// </summary>
    ESOTERIC ,
}

/// <summary>
/// 目标选取类似
/// </summary>
public enum TargetGetterType
{
    /// <summary>
    /// 单攻
    /// </summary>
    Single,
    /// <summary>
    /// 群攻
    /// </summary>
    Group,
    Floating,
}
public enum HitPosEnum
{
    Target,     //根据目标决定
    Center,     //中心点
}

/// <summary>
/// 攻击附加状态
/// </summary>
public enum AttackAttachOneState
{
    /// <summary>
    /// 无状态附加攻击
    /// </summary>
    Normal,
    /// <summary>
    /// 浮空状态攻击
    /// </summary>
    HitFloating,
    /// <summary>
    /// 击飞攻击
    /// </summary>
    HitFly,
    /// <summary>
    /// 击退状态攻击
    /// </summary>
    HitBack,
    /// <summary>
    /// 击落状态攻击
    /// </summary>
    HitFall,
    /// <summary>
    /// 击倒状态攻击
    /// </summary>
    HitDownGround,
}

public enum RoleOneState
{
    /// <summary>
    /// 正常状态
    /// </summary>
    Normal,
    /// <summary>
    /// 非正常状态
    /// </summary>
    NoNormal,
    /// <summary>
    /// 击飞-上升
    /// </summary>
    FloatingUp,
    /// <summary>
    /// 击飞-最高点
    /// </summary>
    FloatingTop,
    /// <summary>
    /// 击飞-浮空并后退
    /// </summary>
    Fly,
    /// <summary>
    /// 击飞-自由下落
    /// </summary>
    FlyDownFree,
    /// <summary>
    /// 击飞-强制下落
    /// </summary>
    FlyDownForce,
    /// <summary>
    /// 倒地/击倒
    /// </summary>
    FallGround,
    /// <summary>
    /// 起立状态
    /// </summary>
    GetUp,
    /// <summary>
    /// 击退-后退
    /// </summary>
    GoBack,
    /// <summary>
    /// 击退-后退结束
    /// </summary>
    GoBackOver,
    /// <summary>
    /// 击退-还原
    /// </summary>
    GoAhead,
}
/// <summary>
/// 战斗相机状态枚举
/// </summary>
public enum BatCamEnum
{
    Free,
    Follow,
    Shake,
}

public class GameEnum
{
}

