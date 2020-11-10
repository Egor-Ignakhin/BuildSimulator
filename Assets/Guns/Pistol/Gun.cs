using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    internal int Damage { get; set; }
    internal int Ammo { get; set; }
    public AudioClip FireClip;
}
