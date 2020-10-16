using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject QuestionsTrello, Activer;// лист управления и лист меню
    [SerializeField] private Inventory _inventory;

    public delegate void ActiveMenu();// событие  определения положения
    public static event ActiveMenu ActiveMenuEvent;// событие  определения положения

    public static bool ActiveGameMenu;
    public void OnClick(int num)
    {
        switch (num)
        {
            case 0:
                SceneManager.LoadScene(0);
                break;
            case 1:
                QuestionsTrello.SetActive(true);
                Activer.SetActive(false);
                break;
            case 2:
                Activer.SetActive(false);
                ActiveGameMenu = false;
                Cursor.visible = false;
                break;

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _inventory.TurnOffOn();
            ActiveGameMenu = _inventory.IsActive;
            Cursor.visible = _inventory.IsActive;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActiveMenuEvent();
            if (_inventory.IsActive == true)
            {
                _inventory.TurnOffOn();
                ActiveGameMenu = false;
                Cursor.visible = false;
                return;
            }
            if (QuestionsTrello.activeInHierarchy)
            {
                QuestionsTrello.SetActive(false);
                ActiveGameMenu = false;
                Cursor.visible = false;
            }
            if (Activer.activeInHierarchy)
            {
                Activer.SetActive(false);
                ActiveGameMenu = false;
                Cursor.visible = false;
            }
            else
            {
                Activer.SetActive(true);
                ActiveGameMenu = true;
                Cursor.visible = true;
            }
            return;
        }

        Cursor.lockState = ActiveGameMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
