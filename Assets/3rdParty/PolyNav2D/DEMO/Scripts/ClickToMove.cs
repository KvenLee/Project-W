using UnityEngine;
using System.Collections.Generic;

//example
[RequireComponent(typeof(PolyNavAgent))]
public class ClickToMove : MonoBehaviour
{
    private PolyNavAgent _agent;
    public PolyNavAgent agent
    {
        get
        {
            if(!_agent)
                _agent = GetComponent<PolyNavAgent>();
            return _agent;
        }
    }

    void Update()
    {
        if(CGameRoot.Instance.IsPointOnUI)
        {
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        Vector2 click_pos = Input.mousePosition;
#else
        Vector2 click_pos =  Input.touchCount > 0 ? Input.GetTouch(0).position :Vector2.zero;
#endif

        if(Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(click_pos);
            agent.SetDestination(pos);
        }
    }
}