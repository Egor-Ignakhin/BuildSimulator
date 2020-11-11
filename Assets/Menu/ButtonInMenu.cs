using UnityEngine;
using UnityEngine.UI;
namespace MainMenu
{
    public class ButtonInMenu : MonoBehaviour
    {
        private MenuManager manager;
        public GameObject ActiveObject;
        public GameObject[] OtherActiveObjects;
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
}