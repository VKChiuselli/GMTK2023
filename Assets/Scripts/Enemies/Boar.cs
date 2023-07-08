using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Unit
{

    public void PlayDeathSFX()
    {
        GetComponent<SFX>().PlayFirstEffect();
    }
    public void PlayAttackSFX()
    {
        GetComponent<SFX>().PlaySecondEffect();
    }


}
