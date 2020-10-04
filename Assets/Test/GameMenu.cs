using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject QuestionsTrello,Activer;// лист управления и лист меню

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
                break;

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&! QuestionsTrello.activeInHierarchy)
        {
            Activer.SetActive(!Activer.activeSelf);
        }
        if (Activer.activeInHierarchy|| QuestionsTrello.activeInHierarchy)
        {
            ActiveGameMenu = true;
            Cursor.visible = true;
        }
        else
        {
            ActiveGameMenu = false;
            Cursor.visible = false;
        }

        if (QuestionsTrello.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuestionsTrello.SetActive(false);
                Activer.SetActive(true);
            }
        }
        if (ActiveGameMenu)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
