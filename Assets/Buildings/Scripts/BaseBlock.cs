using UnityEngine;

public sealed class BaseBlock : MonoBehaviour
{
    public bool IsBlock = true;
    [Range(0, 3)] public int Type;
    private Renderer myRend;
    internal Rigidbody _myRb { get; private set; }
    internal ObjectDown _obDown { get; set; }

    public void OnEnable() => myRend = GetComponent<Renderer>();
    private void Start()
    {
        ChangeColor(0);
        if (IsBlock)
        {
            _obDown.Objects.Add(this);//добавление в список ВзрывОбъектов

            gameObject.AddComponent<SaveObject>();
            GetComponent<SaveObject>().enabled = true;
        }
    }

    public void Destroy(float power)
    {
        if (!IsBlock)
            return;

        power *= Random.Range(10,16);
        int multiply = Random.Range(1, 3);
        power = multiply == 1 ? power : power * -1;

        if (!_myRb)
        {
            _myRb = gameObject.AddComponent<Rigidbody>();
            _obDown.ExplosivedObjects.Add(this);
        }
        _myRb.velocity = new Vector3(power, power, power);
    }

    public void ChangeColor(byte type)
    {
        switch (type)
        {
            case 0:
                if (Type != 2)// if block not is glass
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
        {
            if (IsBlock)
            {
                _obDown.Objects.Remove(this);
                _obDown.ExplosivedObjects.Remove(this);
            }
        }
    }
}