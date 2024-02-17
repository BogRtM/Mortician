using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulOrbLocator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject[] soulOrbs;

    public void ActivateSphere(int i)
    {
        soulOrbs[i].SetActive(true);
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
