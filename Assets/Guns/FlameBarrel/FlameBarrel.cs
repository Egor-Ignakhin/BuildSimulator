using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FlameBarrel : ExplosiveObject
{
    private AudioSource _myAud;
    private Rigidbody _myRb;
    internal CapsuleCollider _myCollider { get; private set; }
    private void Awake()
    {
        Raduis = 2.25f;
        Power = 3.5f;
    }
    protected override void Start()
    {
        base.Start();
        gameObject.layer = 10;
        _myCollider = GetComponent<CapsuleCollider>();
        _myAud = transform.GetChild(0).GetComponent<AudioSource>();
        _myRb = GetComponent<Rigidbody>();
        _myAud.clip = FindObjectOfType<BarrelsManager>().DetonationClip;
    }
    private void OnCollisionEnter(Collision collision)
    {
        float power = collision.relativeVelocity.magnitude;
        if (_myRb.velocity.magnitude >= 3||power > 5)
        {
            Detonation();
        }
    }
    internal void Detonation()
    {
        _myAud.gameObject.SetActive(true);
        _myAud.Play();
        _myAud.transform.SetParent(_objectDown.transform);
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
                rocket.Detonation();
            }
            else if (barrel = objects[i].GetComponent<FlameBarrel>())
            {
                if (barrel != this)
                {
                    _objectDown.Explosives.Remove(barrel);
                       barrel.Detonation();
                }
            }
        }
        Destroy(gameObject);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
