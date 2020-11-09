using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject QuestionsTrello, Activer;// лист управления и лист меню
    private InventoryAndItems.Inventory _inventory;

    public delegate void ActiveMenu();// событие  определения положения
    public static event ActiveMenu ActiveMenuEvent;// событие  определения положения

    private static bool _activeGameMenu;
    public static bool ActiveGameMenu
    {
        get => _activeGameMenu;
        set
        {
            _activeGameMenu = value;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = value;
        }
    }
    [SerializeField] private Saver _saver;
    internal static bool _wasSaved { get; private set; } = false;
    private void Start()
    {
        _inventory = InventoryAndItems.Inventory.Instance;
        MainInput.input_I += this.SetVisible;
        MainInput.input_Escape += this.MenuEvent;
    }

    public void OnClick(int num)
    {
        switch (num)
        {
            case 0:
                _wasSaved = true;//если булева  == true то загрузить меню
                _saver.Save();
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
    private void SetVisible()
    {
        _inventory.TurnOffOn();
        ActiveGameMenu = _inventory.IsActive;
        Cursor.visible = _inventory.IsActive;
    }
    private void MenuEvent()
    {
        ActiveMenuEvent?.Invoke();
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
    }
    private void OnDestroy()
    {
        MainInput.input_I -= this.SetVisible;
        MainInput.input_Escape -= this.MenuEvent;
    }
}
