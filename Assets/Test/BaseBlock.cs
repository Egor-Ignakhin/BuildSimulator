using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    [Range(0, 2)] public int Type;
    private bool _isDestroy;

    public void OnEnable()
    {
        myRend = GetComponent<Renderer>();
    }
    private void Start()
    {
        ChangeColor(0);
    }

    public void Destroy()
    {
        if (_isDestroy)
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
    private Renderer myRend;
    public void ChangeColor(byte type)
    {
        switch (type)
        {
            case 0:
                if(Type != 2)// if block not is glass
                myRend.material.color = new Color(1, 1, 1);
                else
                {
                    myRend.material.color = new Color(1, 1, 1, 0.5f);
                }
                break;
            case 1:
                myRend.material.color = new Color(0, 1, 0, 0.5f);
                break;
            case 2:
                myRend.material.color = new Color(1, 0, 0, 0.5f);
                break;
            case 3:
                myRend.material.color = new Color(1, 0.75f, 0, 1);
                break;

            default:
                break;
        }
    }
}
