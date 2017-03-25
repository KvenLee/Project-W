using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour
{
    public float updateInterval = 0.5f;
    float lastInterval;
    int frames;
    float fps;
    GUIStyle style;

    // Use this for initialization
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        style = new GUIStyle();
        style.fontSize = 24;
        style.richText = true;
    }

    void Update()
    {
        ++frames;
        if(Time.realtimeSinceStartup > lastInterval + updateInterval)
        {
            fps = frames / (Time.realtimeSinceStartup - lastInterval);
            frames = 0;
            lastInterval = Time.realtimeSinceStartup;
        }
    }

    void OnGUI()
    {
        string text = "<color=\"red\">" + "FPS:" + fps.ToString("f2") + "</color>";
        GUI.Label(new Rect(0, 0, 200, 200), text, style);
    }
}
