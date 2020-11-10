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
    internal override void Detonation()
    {
        _objectDown.Explosives.Remove(this);

        _myAud.clip = DetonationClip;
        _myAud.Play();
        _myAud.transform.SetParent(_objectDown.transform);
        _detonationEffect.SetActive(true);
        _flyingEffect.SetActive(false);
        Destroy(_myAud.gameObject, _myAud.clip.length - 1);

        FindNearestObjects();
    }

    protected override void FindNearestObjects()
    {
       FoundObjects = _objectDown.GetNearestObject(transform.position, Raduis);
        for (int i = 0; i < FoundObjects.Count; i++)
        {
            if (FoundBlock =  FoundObjects[i] as BaseBlock)
                FoundBlock.Destroy(Power);
            else if (FoundExplosiveObject =  FoundObjects[i] as ExplosiveObject)
                FoundExplosiveObject.Detonation();
        }

        Launcher.ChangeRocketLength(this);
        Destroy(gameObject);
}
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}