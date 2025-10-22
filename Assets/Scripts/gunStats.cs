using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;

    [Range(1, 100)] public int shootDamage;
    [Range(15, 1000)] public int shootDist;
    [Range(0.1f, 3)] public float shootRate;
    public int ammoCur;
    [Range(5, 200)] public int ammoMax;
    //[Range(1, 10)] public int maxProjectiles;
    //[Range(1, 5)] public int maxChains;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSounds;
    [Range(0, 1)] public float shootSoundVol;


}

