using System.Collections.Generic;
using UnityEngine;

public abstract class ExplosiveObject : MonoBehaviour//Классы, наследуемые от этого класса, имеют радиус и силу взрыва
{
    public float Raduis = 2.2f;
    public float Power = 2.75f;
    protected ObjectDown _objectDown;

    protected virtual void Start()
    {
        _objectDown = FindObjectOfType<ObjectDown>();
        _objectDown.Explosives.Add(this);
    }

    protected List<MonoBehaviour> FoundObjects;
    protected BaseBlock FoundBlock;
    protected ExplosiveObject FoundExplosiveObject;

    protected abstract void FindNearestObjects();
    internal abstract void Detonation();

    protected virtual void OnDestroy() => _objectDown.Explosives.Remove(this);
}