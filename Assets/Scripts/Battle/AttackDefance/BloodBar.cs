using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBar : MonoBehaviour
{
    public SpriteRenderer bloodRender;

    [Range(0,1)]
    public float fillAmount = 1f;

    Vector3 m_BloodLength = Vector3.zero;
    float m_ShowTime = 2f;
    float _timer;
    void Awake()
    {
        m_BloodLength = bloodRender.transform.localScale;
    }

    public void SetBloodBar(float amount)
    {
        fillAmount = amount;
        if (bloodRender != null)
        {
            if (float.Equals(fillAmount, 1f) == false)
            {
                gameObject.SetActive(true);
                _timer = Time.time;
            }
            if (m_BloodLength == Vector3.zero)
            {
                m_BloodLength = bloodRender.transform.localScale;
            }
            Vector3 scale = m_BloodLength; scale.x = m_BloodLength.x * fillAmount;
            bloodRender.transform.localScale = scale;
        }
    }
    void Update()
    {
        if (Time.time - _timer > m_ShowTime)
        {
            gameObject.SetActive(false);
        }
    }

    void OnValidate()
    {
        SetBloodBar(fillAmount);
    }
}
