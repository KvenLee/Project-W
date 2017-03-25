using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCfg : MonoBehaviour
{
    public const float mapDistance = 10.24f;

    public List<Sprite> containsSprites = new List<Sprite>();

    public Sprite GetSprite(string spritename)
    {
        return containsSprites.Find(delegate(Sprite temp)
        {
            return temp.name.Equals(spritename);
        });
    }

    public bool IsContains(string spritename)
    {
        return GetSprite(spritename) != null;
    }
    public Sprite this[int index]
    {
        get
        {
            if(containsSprites != null && containsSprites.Count > index)
            {
                return containsSprites[index];
            }
            return null;
        }
    }

    public int Count
    {
        get
        {
            return containsSprites.Count;
        }
    }
}
