using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableEntity : PointOfInterest
{
    
    public virtual void InteractWith(Unit unit) //TODO: Make this abstract
    {
        Debug.Log("TODO");
    }
}
