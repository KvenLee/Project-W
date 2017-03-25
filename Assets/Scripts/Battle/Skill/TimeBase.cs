using UnityEngine;

public class TimeBase : MonoBehaviour
{
    private int frame = 45;

    /// <summary>
    /// 重力或浮力
    /// </summary>
    protected float force = 9.8f;
    /// <summary>
    /// Time.scaleTime
    /// </summary>
    protected float timeScale = 1;
    /// <summary>
    /// Time.time
    /// </summary>
    protected float m_Time;
    protected float _timer;
    /// <summary>
    /// Time.deltaTime
    /// </summary>
    protected float m_DeltaTime;
    protected float m_DeltaScaleTime
    {
        get
        {
            return m_DeltaTime * timeScale;
        }
    }
    /// <summary>
    /// new WaitForSeconds(Time.deltaTime)
    /// </summary>
    protected float m_DeltaWaitTime
    {
        get
        {
            return m_DeltaTime / timeScale;
        }
    }

    protected virtual void Awake()
    {
        m_DeltaTime = 1f / frame;
        m_Time = 0f;
        _timer = 0f;
    }

	// Update is called once per frame
	protected virtual void Update ()
    {
        m_Time += m_DeltaTime * timeScale * Time.timeScale;
    }

    public virtual void SetTimeScale(float timescale)
    {
        if(timeScale < 0)
            timeScale = 0;

        timeScale = timescale;
    }
}
