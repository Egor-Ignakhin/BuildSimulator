using System.Collections.Generic;
using UnityEngine;

public abstract class ExplosiveObject : MonoBehaviour//Классы, наследуемые от этого класса, имеют радиус и силу взрыва
{
    public float Raduis = 2.2f;//радиус для поиска ближайшиъ объектов
    public float RaduisExplosion = 2.2f;//радиус для поиска ближайших взрывОбъектов
    public float Power = 2.75f;//сила взрыва
    protected ObjectDown _objectDown;
    private Rigidbody _player;
    internal abstract byte Type { get; }//тип взрывчатки

    protected virtual void Start()
    {
        _objectDown = FindObjectOfType<ObjectDown>();
        _objectDown.Explosives.Add(this);
        _player = FindObjectOfType<FirstPersonController>().GetComponent<Rigidbody>();
    }

    protected List<MonoBehaviour> FoundObjects;
    protected BaseBlock FoundBlock;
    protected ExplosiveObject FoundExplosiveObject;

    protected virtual void FindNearestObjects()//поиск ближайших объектов и подрыв их
    {
        _objectDown.Explosives.Remove(this);
        FoundObjects = _objectDown.GetNearestObject(transform.position, Raduis,RaduisExplosion);
        for (int i = 0; i < FoundObjects.Count; i++)
        {
            if (FoundBlock = FoundObjects[i] as BaseBlock)
                FoundBlock.Destroy(Power);
            else if (FoundExplosiveObject = FoundObjects[i] as ExplosiveObject)
                FoundExplosiveObject.Detonation();
        }

        if (Vector3.Distance(transform.position, _player.transform.position) < Raduis * 3)
            _player.AddExplosionForce(10, transform.position, Raduis * 3, 0, ForceMode.Impulse);

        Destroy(gameObject);
    }
    internal abstract void Detonation();

    protected virtual void OnDestroy() => _objectDown.Explosives.Remove(this);
}