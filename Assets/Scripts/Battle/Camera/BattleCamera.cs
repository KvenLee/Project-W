using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : FlyAction
{
    public BatCamEnum batCamEnum = BatCamEnum.Free;
    private int maxShakeCount = 5;
    private Vector3 m_OriPos;
    private Vector3 m_EndPos;
    private int m_ShakeCount;

    private static BattleCamera m_Instance;
    public static BattleCamera Instance
    {
        get
        {
            return m_Instance;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        m_Instance = this;
    }

    void Start()
    {
        m_OriPos = transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        switch (batCamEnum)
        {
            case BatCamEnum.Follow: break;
            case BatCamEnum.Shake:
                int ra = m_ShakeCount % 2 == 0 ? 1 : -1;
                if(m_ShakeCount == maxShakeCount)
                {
                    FlyDirect(m_Time, transform.position, m_OriPos, 30f, delegate()
                    {
                        transform.position = m_OriPos;
                        batCamEnum = BatCamEnum.Follow;
                    });
                }
                else
                {
                    m_EndPos = m_OriPos + new Vector3(1f * ra, 0f, 0);
                    FlyDirect(m_Time, transform.position, m_EndPos, 30f, delegate()
                    {
                        transform.position = m_EndPos;
                        m_EndPos = m_OriPos + new Vector3(1f, 0f, 0);
                        m_ShakeCount++;
                    });
                    m_ShakeCount++;
                }
                break;
        }
    }

    public void Shake()
    {
        m_EndPos = m_OriPos + new Vector3(1f, 1f, 0);
        batCamEnum = BatCamEnum.Shake;
        m_ShakeCount = 0;
    }
}
