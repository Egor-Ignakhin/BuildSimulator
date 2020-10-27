using UnityEngine;
using UnityEngine.UI;

public class ButtonInMenu : MonoBehaviour
{
    private MenuManager manager;
    public GameObject ActiveObject;
    private void Start()
    {
        manager = (MenuManager)FindObjectOfType(typeof(MenuManager));
        gameObject.AddComponent<Button>();
        Button myButton = GetComponent<Button>();

        myButton.onClick.AddListener(this.Click);
    }
    public void Click()
    {
        manager.Clickk(this);
    }
}
