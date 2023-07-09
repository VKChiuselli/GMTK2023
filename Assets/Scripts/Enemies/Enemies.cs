using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : Unit
{
    public override void AILogic()
    {
        List<GameObject> objs = GetObjectsInRange(false, MaxMovement, false);
        if (TargetClosest<Hero>(objs)) return;
    }

    public override void HoverInfo()
    {
        GetObjectsInRange(true, MaxMovement * 2, false);
        base.HoverInfo();
    }
}
