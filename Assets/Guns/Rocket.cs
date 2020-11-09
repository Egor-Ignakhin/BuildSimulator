using System.Collections.Generic;
using UnityEngine;

public sealed class Rocket : ExplosiveObject
{   
    [SerializeField] private AudioSource _myAud;
    internal AudioClip DetonationClip { get; set; }
    [SerializeField] private GameObject _detonationEffect;
    [SerializeField] private GameObject _flyingEffect;
    internal RocketLauncher Launcher { get; set; }
    internal Vector3 StartPosition { get; private set; }
    private void Awake()
    {
        Raduis = 2.2f;
        Power = 2.75f;
    }
    protected override void Start()
    {
        base.Start();
        StartPosition = transform.position; 
    }
    private void OnCollisionEnter(Collision collision) => Detonation();
    internal void Detonation()
    {
        _myAud.clip = DetonationClip;
        _myAud.Play();
        _myAud.transform.SetParent(_objectDown.transform);
        _detonationEffect.SetActive(true);
        _flyingEffect.SetActive(false);
        Destroy(_myAud.gameObject, _myAud.clip.length - 1);

        List<Transform> objects = _objectDown.GetNearestObject(transform.position, Raduis);
        BaseBlock block;
        Dunamites.DunamiteClon dunamite;
        Rocket rocket;
        FlameBarrel barrel;
        for (int i = 0; i < objects.Count; i++)
        {
            if (block = objects[i].GetComponent<BaseBlock>())
            {
                    block.Destroy(Power);
            }
            else if (dunamite = objects[i].GetComponent<Dunamites.DunamiteClon>())
            {
                dunamite.TimerToExplosion = 0;
                dunamite.Detonation();
            }
            else if (rocket = objects[i].GetComponent<Rocket>())
            {
                if(rocket != this)
                {
                    _objectDown.Explosives.Remove(rocket);
                    rocket.Detonation();
                }

            }
            else if (barrel = objects[i].GetComponent<FlameBarrel>())
            {
                barrel.Detonation();
            }
        }

        Launcher.ChangeRocketLength(this);
        Destroy(gameObject);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}