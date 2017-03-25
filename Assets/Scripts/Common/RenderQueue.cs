using UnityEngine;
using System.Collections;

public class RenderQueue : MonoBehaviour
{
    public int renderQueue = 5;

    // Use this for initialization
    void Start()
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        for (int i = renders.Length - 1; i >= 0; i--)
        {
            renders[i].sharedMaterial.renderQueue += renderQueue;
        }
    }
}
