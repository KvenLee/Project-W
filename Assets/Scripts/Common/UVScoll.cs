using UnityEngine;
using System.Collections;

public class UVScoll : MonoBehaviour
{
    Renderer render;
    public float scollSpeed = 5f;
    public int xCount = 4;
    public int yCount = 4;

    float offsetx = 0f;
    float offsety = 0f;
    Vector2 singleTexSize = Vector2.zero;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<Renderer>();
        singleTexSize = new Vector2(1.0f / xCount, 1.0f / yCount);
        render.material.mainTextureScale = singleTexSize;
    }

    // Update is called once per frame
    void Update()
    {
        float frame = Mathf.Floor(Time.time * scollSpeed);
        offsetx = frame / xCount;
        offsety = -(frame - frame % xCount) / yCount / xCount;
        render.material.SetTextureOffset("_MainTex", new Vector2(offsetx, offsety));
    }
}
