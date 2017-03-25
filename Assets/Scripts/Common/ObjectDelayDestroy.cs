using UnityEngine;
using System.Collections;

/*************************
 *  author: Andrea.Chow
 *  date: 
 *  class function: 特效从0到1 延迟消失
 * 
 *************************/

public class ObjectDelayDestroy : MonoBehaviour 
{
    public float delayTime = 0.2f;

	void OnEnable()
    {
        Invoke("Do", delayTime);
	}

    void Do()
    {
        gameObject.SetActive(false);
    }
}
