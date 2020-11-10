using UnityEngine;

public sealed class SaveObject : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<SaveObjectsManager>().Objects.Add(this);
    }
    private void OnDestroy()
    {
        if (FindObjectOfType<SaveObjectsManager>())
        {
            FindObjectOfType<SaveObjectsManager>().Objects.Remove(this);
        }
    }
}