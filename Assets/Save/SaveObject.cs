using UnityEngine;

public sealed class SaveObject : MonoBehaviour
{
    private SaveObjectsManager manager;
    private void Start()
    {
        manager = FindObjectOfType<SaveObjectsManager>();
        manager.Objects.Add(this);
    }
    private void OnDestroy() => manager.Objects.Remove(this);
}
