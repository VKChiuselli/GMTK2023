using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemies
{
    FunBarManager funBarManager;

    protected override void Start()
    {
        funBarManager = FindObjectOfType<FunBarManager>();
    }

    public override void Death(Unit whoKilled)
    {
        if (whoKilled.GetType() == typeof(Hero))
        {
            if (funBarManager)
                funBarManager.ChangeFunBarCounter(15);
        }
        else
        {

        }
        PlayDeathSFX();
        return;
    }

    public void PlayDeathSFX()
    {
        GetComponent<SFX>().PlayFirstEffect();
    }
    public void PlayAttackSFX()
    {
        GetComponent<SFX>().PlaySecondEffect();
    }


}
