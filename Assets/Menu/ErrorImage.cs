using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ErrorImage : MonoBehaviour
{
    internal static ErrorImage Singleton { get; private set; }
    public string Text { get; set; }
    private Image _myImage;
    public TextMeshProUGUI TextError;
    public  string TitleError { get; set; }
    Color color = new Color(0, 0, 0, -0.00092375f);

    private void Awake()
    {
        Singleton = this;
    }
    private void Update()
    {
        if (TextError.color.a > 0.005f)
        {
            _myImage.color += color;
            TextError.color += color;
        }
        else 
        {
            TextError.enabled = false;
            this.enabled = false; 
        }
    }
    public void OnEnableColor()
    {
        TextError.enabled = true;
        TextError.text = TitleError;

        _myImage = GetComponent<Image>();
        _myImage.color = Color.red;
        TextError.color = Color.white;
    }
}
