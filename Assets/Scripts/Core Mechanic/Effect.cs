using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float Duration;
    public GameObject DeathObject = null;

    public void Start()
    {
        if (GetComponent<SFX>())
            GetComponent<SFX>().PlayFirstEffect();
        Invoke(nameof(DestroyObj), Duration);
    }

    void DestroyObj()
    {
        if (DeathObject != null)
            Instantiate(DeathObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
