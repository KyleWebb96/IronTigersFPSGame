using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    [Header("----- Gun Stats -----")]
    public float shootRate;
    public int shootDist;
    public int shootDamage;
    public int ammoCount;

    [Header("----- Gun Components -----")]
    public GameObject gunModel;
    public GameObject hitEffect;
    public GameObject muzzleFlash;
    public AudioClip gunSound;
}
