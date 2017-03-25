using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Cover2D : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        collider.isTrigger = true;
        DebugDrawPolygon(TransformPoints(collider.points, transform), Color.blue);
    }
    void DebugDrawPolygon(Vector2[] points, Color color)
    {
        for(int i = 0; i < points.Length; i++)
            Debug.DrawLine(points[i], points[(i + 1) % points.Length], color);
    }
    Vector2[] TransformPoints(Vector2[] points, Transform t)
    {
        for(int i = 0; i < points.Length; i++)
            points[i] = t.TransformPoint(points[i]);
        return points;
    }
#endif
}
