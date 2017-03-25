using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform follower;
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if(follower !=null)
        {
            transform.position = Vector2.Lerp(transform.position, follower.position, speed * Time.deltaTime);
        }
    }
}
