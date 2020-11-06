using System.Collections.Generic;
using UnityEngine;

public sealed class Bullet : MonoBehaviour
{
    internal ObjectDown _objectDown { get; set; }
    [SerializeField] private AudioSource _myAud;
    internal AudioClip DetonationClip { get; set; }
    [SerializeField] private GameObject _detonationEffect;
    [SerializeField] private GameObject _flyingEffect;
    internal RocketLauncher Launcher { get; set; }
    internal Vector3 StartPosition { get; private set; }

    private void Start() => StartPosition = transform.position;
    private void OnCollisionEnter(Collision collision) => Detonation();
    internal void Detonation()
    {
        _myAud.clip = DetonationClip;
        _myAud.Play();
        _myAud.transform.SetParent(_objectDown.transform);
        _detonationEffect.SetActive(true);
        _flyingEffect.SetActive(false);
        Destroy(_myAud.gameObject, _myAud.clip.length - 1);

        List<BaseBlock> objects = _objectDown.GetNearestObject(transform.position, 2.2f);

        for (int i = 0; i < objects.Count; i++)
            objects[i].Destroy(2.75f);

        Launcher.ChangeRocketLength(this);
        Destroy(gameObject);
    }
}