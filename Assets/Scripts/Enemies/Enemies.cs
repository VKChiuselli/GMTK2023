using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : Unit
{
    public override void AILogic()
    {
        var prePosition = transform.position;
        List<GameObject> objs = GetObjectsInRange(MaxMovement * 2, false, false);
        if (TargetClosest<Hero>(objs))
        {
            Instantiate(GameManager.Inst.SurprisedEffect, prePosition + Vector3.up, Quaternion.identity);
            return;
        }
    }

    public override void HoverInfo()
    {
        GetObjectsInRange(MaxMovement * 2, true, false);
        base.HoverInfo();
    }
}
