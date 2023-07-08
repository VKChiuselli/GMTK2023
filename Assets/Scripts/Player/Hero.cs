using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class Hero : Unit
{

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
