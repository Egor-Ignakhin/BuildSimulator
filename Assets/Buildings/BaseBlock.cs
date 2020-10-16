using UnityEngine;

public sealed class BaseBlock : MonoBehaviour
{
    [Range(0, 2)] public int Type;
    public bool _isDestroy { get; private set; }
    private Renderer myRend;
    public ObjectDown _obDown;
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

        sbyte force = (sbyte)(Random.Range(-4, 4) * 15);
        gameObject.AddComponent<Rigidbody>().velocity = new Vector3(force, force, force);
        GetComponent<BoxCollider>().isTrigger = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<BoxCollider>().isTrigger = false;
        }
        _isDestroy = true;
    }
    

    public void ChangeColor(byte type)
    {
        switch (type)
        {
            case 0:
                if(Type != 2)// if block not is glass
                myRend.material.color = Color.white;
                else
                    myRend.material.color = new TransparentColors(0).color;
                break;
            case 1:
                myRend.material.color = new TransparentColors(1).color;
                break;
            case 2:
                myRend.material.color = new TransparentColors(2).color;
                break;
            case 3:
                myRend.material.color = new TransparentColors(3).color;
                break;

            default:
                break;
        }
    }
    private void OnDestroy()
    {
        if (_obDown)
            _obDown.Objects.Remove(this);
    }
}
