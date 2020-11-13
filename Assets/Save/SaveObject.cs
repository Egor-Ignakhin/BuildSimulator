using UnityEngine;

public sealed class SaveObject : MonoBehaviour
{
    private void Start()
    {
        if (!FindObjectOfType<ObjectDown>()._isMission)
            FindObjectOfType<SaveObjectsManager>().Objects.Add(this);
    }
    private void OnDestroy()
    {
        if (!FindObjectOfType<ObjectDown>()._isMission)
        {
            if (FindObjectOfType<SaveObjectsManager>())
            {
                FindObjectOfType<SaveObjectsManager>().Objects.Remove(this);
            }
        }
    }
}