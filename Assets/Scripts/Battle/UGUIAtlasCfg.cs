using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUIAtlasCfg : MonoBehaviour
{
    public Dictionary<string, Sprite> atlasCfg = new Dictionary<string, Sprite>();

    public Sprite this[string _name]
    {
        get
        {
            Sprite sprite = null;
            if(atlasCfg.TryGetValue(_name, out sprite))
            {
                return sprite;
            }
            return null;
        }
    }
}
