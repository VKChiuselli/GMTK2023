using System.Security.AccessControl;
using Assets.Scripts;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class Hero : Unit
{

    /*
         * Priorities------------
         * Spotlight
         * Chests
         * Keys
         * Doors
         * Enemies
         * Random
         */
    private LightRange _light;

    protected override void Start()
    {
        base.Start();
        _light = GetComponent<LightRange>();
    }

    public override void HoverInfo()
    {
        GetObjectsInRange(true, 1000, true);
        base.HoverInfo();
    }

    public override void AILogic()
    {
        var prevPosition = transform.position;
        List<GameObject> objs = GetObjectsInRange(false, 1000, true);
        // if they can move to you, game over
        if (TargetClosest<Player>(objs))
        {
            Instantiate(GameManager.Inst.SurprisedEffect, prevPosition + Vector3.up, Quaternion.identity);
            GameManager.Inst.Invoke(nameof(GameManager.Inst.GameOverHeroSawParent), 0.8f);
            return;
        }

        if (TargetClosest<Trap>(objs)) return;
        // Find closeset chest
        if (TargetClosest<Chest>(objs)) return;
        // Find keys
        if (TargetClosest<Key>(objs)) return;
        // Find Doors
        if (TargetClosest<Door>(objs)) return;
        // Find enemies
        if (TargetClosest<Enemies>(objs)) return;

    }

    public override void Death(Unit causeOfDeath)
    {
        base.Death(causeOfDeath);
        GameManager.Inst.GameOverHeroDeath();
    }


    public void PlayArrowSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(0);
    } 

    public void PlayAttackSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(1);
    } 

    public void PlayShieldBlockSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(2);
    } 

    public void PlayTakingDamageSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(3);
    } 

    public void PlayTorchSFX()
    {
        GetComponent<SFX>().PlayOneSpecific(4);
    } 



}
