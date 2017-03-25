using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodText : FlyAction
{
    public TextMesh textMesh;

    private Transform m_Target;
    private Vector3 beginpos;
    private Vector3 endpos;
    public void SetBloodText(int bloodCount, Transform Target)
    {
        m_Target = Target;
        if(textMesh != null)
        {
            textMesh.text = string.Format("<color=#ff0000>{0}</color>", bloodCount);
        }
        transform.position = m_Target.position + new Vector3(0, 1f, 0);
        endpos = m_Target.position + new Vector3(0, 1.8f, 0);
    }
    protected override void Update()
    {
        base.Update();
        FlyLerp(m_DeltaScaleTime, transform.position, endpos, 1f, delegate()
            {
                transform.position = endpos;
                m_Target = null;
                gameObject.SetActive(false);
            });
    }
}
