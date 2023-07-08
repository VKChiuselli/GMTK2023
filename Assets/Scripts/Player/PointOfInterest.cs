using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PointOfInterest : MonoBehaviour
{
    protected bool _inHeroVision = false;
    protected bool _inShadow = false;
    public bool IsVisibleToHero => _inHeroVision && !_inShadow;

    public virtual void FlagInShadow(bool isInShadow)
    {
        _inShadow = isInShadow;
    }

    public virtual void FlagVisible(bool isVisible)
    {
        _inHeroVision = isVisible;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
    
}
