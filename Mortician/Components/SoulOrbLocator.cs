using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using Morris.Modules;

public class SoulOrbLocator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject[] soulOrbs;

    public void ActivateSphere(int i)
    {
        soulOrbs[i].SetActive(true);

        EffectData effectData = new EffectData()
        {
            origin = soulOrbs[i].transform.position
        };

        //Util.PlaySound("Play_mage_m1_impact", soulOrbs[i]);
        EffectManager.SpawnEffect(Assets.SoulOrbActivatedEffect, effectData, true);
    }

    public void DeactivateSphere(int i)
    {
        soulOrbs[i].SetActive(false);
    }

    public Vector3 GetPosition(int i)
    {
        return soulOrbs[i].transform.position;
    }
}
