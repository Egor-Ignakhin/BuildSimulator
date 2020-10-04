using UnityEngine;

public class BaseBlock : MonoBehaviour
{
   [SerializeField] private Color color;

   [Range(0,3)] public int Type;
    private bool _isDestroy;

  
    private void Start()
    {
        GetComponent<Renderer>().material.color = color;
    }

    public void Destroy(float num = 0)
    {if (_isDestroy)
            return;


        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<BoxCollider>();
        gameObject.GetComponent<BoxCollider>().enabled = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<BoxCollider>().isTrigger = false;
        }
        _isDestroy = true;
    }
}
