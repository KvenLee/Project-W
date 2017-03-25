/// <summary>
/// 战斗相关的配置
/// </summary>
public class BattleCfg
{
    #region 浮空参数
    /// <summary>
    /// 浮空高度(小)
    /// </summary>
    public const float HEIGHT_FLOATING_LIGHT = 1f;
    /// <summary>
    /// 浮空高度(大)
    /// </summary>
    public const float HEIGHT_FLOATING_HEAVY = 2f;

    /// <summary>
    /// 浮空时间
    /// </summary>
    public const float TIME_FLOATING = 0.1f;

    /// <summary>
    /// 上升速度（小）
    /// </summary>
    public const float SPEED_UP_LIGHT = 3f;
    /// <summary>
    /// 上升速度（大）
    /// </summary>
    public const float SPEED_UP_HEAVY = 5f;

    /// <summary>
    /// 下落速度（轻）
    /// </summary>
    public const float SPEED_DOWN_LIGHT = 3f;
    /// <summary>
    /// 下落速度（重）
    /// </summary>
    public const float SPEED_DOWN_HEAVY = 6f;
    #endregion

    #region 击退参数
    /// <summary>
    /// 击退距离(小)
    /// </summary>
    public const float DISTANCE_BACK_LIGHT = 0.5f;
    /// <summary>
    /// 击退距离(大)
    /// </summary>
    public const float DISTANCE_BACK_HEAVY = 1f;

    /// <summary>
    /// 击退速度
    /// </summary>
    public const float SPEED_BACK = 6f;
    /// <summary>
    /// 击退返回速度
    /// </summary>
    public const float SPEED_BACK_RESET = 2f;
    #endregion

    #region 反弹参数 注：反弹参数部分公用了上升和下落的速度参数
    /// <summary>
    /// 弹起高度(轻)
    /// </summary>
    public const float HEIGHT_BOUNCE_LIGHT = 0.5f;
    /// <summary>
    /// 弹起高度(重)
    /// </summary>
    public const float HEIGHT_BOUNCE_HEAVY = 1f;
    #endregion


    /// <summary>
    /// 打击相距距离---近程攻击
    /// </summary>
    public const float HIT_DISTANCE = 1f;
}
