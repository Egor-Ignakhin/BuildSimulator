using UnityEngine;

public sealed class Mine : ExplosiveObject
{
    private AudioSource _myAud;
    private Rigidbody _myRb;
    private void Awake()
    {
        Raduis = 1.75f;
        Power = 4;
    }
    protected override void Start()
    {
        base.Start();
        _myAud = transform.GetChild(1).GetComponent<AudioSource>();
        _myAud.clip = FindObjectOfType<MinesManager>().DetonationClip;
        _myRb = gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<RetentionObject>();
    }
    protected override void FindNearestObjects() => base.FindNearestObjects();

    internal override void Detonation()
    {
        _objectDown.Explosives.Remove(this);

        _myAud.gameObject.SetActive(true);
        _myAud.Play();
        _myAud.transform.SetParent(_objectDown.transform);
        Destroy(_myAud.gameObject, _myAud.clip.length - 1);
        FindNearestObjects();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_myRb.velocity.magnitude >= 0.0005f)
            Detonation();
    }
    protected override void OnDestroy() => base.OnDestroy();
}
