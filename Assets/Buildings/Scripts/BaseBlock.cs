using UnityEngine;

public sealed class BaseBlock : MonoBehaviour
{
    public bool IsBlock = true;
    [Range(0, 7)] public int Type;
    private Renderer myRend;
    internal Rigidbody MyRb { get; private set; }
    internal ObjectDown ObDown { get; set; }

    public void OnEnable() => myRend = GetComponent<Renderer>();
    private void Start()
    {
        if (Type != 2)// if block not is glass
            myRend.sharedMaterial.color = Color.white;
        else
            myRend.sharedMaterial.color = new TransparentColors(0).color;


        if (IsBlock)
            ObDown.Objects.Add(this);//добавление в список ВзрывОбъектов
    }
    public void Destroy(float power)
    {
        if (!IsBlock)
            return;

        power *= Random.Range(10, 16);
        int multiply = Random.Range(1, 3);
        power = multiply == 1 ? power : power * -1;

        //в зависимости от типа блока нужно определять сопротивление
        if (Type == 0)//кирпич
            power *= 0.74f;
        else if (Type == 1)//дерево
            power *= 1;
        else if (Type == 2)//стекло
            power *= 1.43f;
        else if (Type == 6)//камень
            power *= 0.55f;
        else if (Type == 7)// песок
            power *= 0.89f;

        if (!MyRb)// если объект не был взорван
        {
            gameObject.isStatic = false;
            MyRb = gameObject.AddComponent<Rigidbody>();
            ObDown.ExplosivedObjects.Add(this);
            gameObject.AddComponent<RetentionObject>();
        }
        MyRb.velocity = new Vector3(power, power, power);
    }

    private void OnDestroy()
    {
        if (!ObDown)
            return;
        if (IsBlock)
        {
            ObDown.Objects.Remove(this);
            ObDown.ExplosivedObjects.Remove(this);
        }
    }
}