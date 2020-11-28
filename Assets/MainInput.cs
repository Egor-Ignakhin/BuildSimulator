using UnityEngine;
using Assets;
public sealed class MainInput : Singleton<MainInput>
{
    internal delegate void MouseSrollMax();
    internal static event MouseSrollMax mouseSrollMax;
    internal delegate void MouseSrollMin();
    internal static event MouseSrollMin mouseSrollMin;

    internal delegate void InputTab();
    internal static event InputTab inputTab;

    internal delegate void Input_I();
    internal static event Input_I input_I;

    internal delegate void Input_Escape();
    internal static event Input_Escape input_Escape;

    internal delegate void Input_GetP();
    internal static event Input_GetP _input_GetP;
    internal delegate void Input_UpP();
    internal static event Input_UpP _input_UpP;

    internal delegate void Input_DownAnyKey();
    internal static event Input_DownAnyKey input_DownAnyKey;

    internal delegate void Input_DownG();
    internal static event Input_DownG input_DownG;

    internal delegate void Input_DownEnter();
    internal static event Input_DownEnter input_DownEnter;

    internal delegate void Input_MouseButtonDown0();
    internal static event Input_MouseButtonDown0 input_MouseButtonDown0;

    internal delegate void Input_MouseButtonUp();
    internal static event Input_MouseButtonUp input_MouseButtoUp0;

    private void Awake() => DontDestroyOnLoad(gameObject);

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0) mouseSrollMax?.Invoke();
        else if (Input.mouseScrollDelta.y < 0) mouseSrollMin?.Invoke();

        if (Input.GetKeyDown(KeyCode.Tab)) inputTab?.Invoke();

        if (Input.GetKeyDown(KeyCode.I)) input_I?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape)) input_Escape?.Invoke();

        if (Input.GetKey(KeyCode.P)) _input_GetP?.Invoke();
        else _input_UpP?.Invoke();

        if (Input.anyKeyDown) input_DownAnyKey?.Invoke();

        if (Input.GetKeyDown(KeyCode.G)) input_DownG?.Invoke();

        if (Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.KeypadEnter)) input_DownEnter?.Invoke();

        if (Input.GetMouseButtonDown(0)) input_MouseButtonDown0?.Invoke();

        if (Input.GetMouseButtonUp(0)) input_MouseButtoUp0?.Invoke();
    }
}
