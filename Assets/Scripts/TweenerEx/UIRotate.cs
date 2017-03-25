using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotate : MonoBehaviour
{
    public float rotate_speed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotate_speed * 100 * Time.deltaTime);
    }
}
