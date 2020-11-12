using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FlameBarrel : ExplosiveObject
{
    private AudioSource _myAud;
    private Rigidbody _myRb;
    internal CapsuleCollider _myCollider { get; private set; }

    internal override byte Type => 4;
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
        gameObject.AddComponent<RetentionObject>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        float power = collision.relativeVelocity.magnitude;
        if (_myRb.velocity.magnitude >= 3||power > 5)
        {
            Detonation();
        }
    }
    internal override void Detonation()
    {
        _objectDown.Explosives.Remove(this);

        _myAud.gameObject.SetActive(true);
        _myAud.Play();
        _myAud.transform.SetParent(_objectDown.transform);
        Destroy(_myAud.gameObject, _myAud.clip.length - 1);
        FindNearestObjects();
    }

    protected override void FindNearestObjects() => base.FindNearestObjects();

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
