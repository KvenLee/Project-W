using UnityEngine;
using System.Collections;

public class UVOffset : MonoBehaviour
{
    public float xoffsetSpeed = 0f;
    public float yoffsetSpeed = 0f;
    Renderer render;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = render.material.GetTextureOffset("_MainTex") + new Vector2(xoffsetSpeed * Time.deltaTime, yoffsetSpeed * Time.deltaTime);
        render.material.SetTextureOffset("_MainTex", pos);
    }
}
