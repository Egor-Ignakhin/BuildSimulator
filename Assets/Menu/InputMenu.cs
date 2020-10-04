using UnityEngine;
using UnityEngine.SceneManagement;

public class InputMenu : MonoBehaviour
{
    private int _buttons = 0;

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        switch (_buttons)
        {
            case 1: Application.Quit();
                break;

            case 2:
                SceneManager.LoadScene("Test");
                break;

            default: break;
        }
    }

    public void Click(int num)
    {
        _buttons = num;
    }
}
