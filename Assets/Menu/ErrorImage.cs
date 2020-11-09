using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets;
public sealed class ErrorImage : Singleton<ErrorImage>
{
    private Image _myImage;
    private TextMeshProUGUI TextError;
    Color color = new Color(0, 0, 0, -0.0072375f);
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _myImage = gameObject.AddComponent<Image>();
        _myImage.enabled = false;

        transform.SetParent(GameObject.Find("Canvas").transform);
        GetComponent<RectTransform>().localPosition = Vector2.zero;
        GetComponent<RectTransform>().localScale = new Vector2(5, 3);

        TextError = new GameObject("TextErorr").AddComponent<TextMeshProUGUI>();
        TextError.transform.SetParent(transform);
        TextError.transform.localPosition = Vector2.zero;
        TextError.transform.localScale *= 1.5f;
        TextError.enabled = false;
        TextError.alignment = TextAlignmentOptions.Center;
        TextError.alignment  = TextAlignmentOptions.Midline;
    }
    private void FixedUpdate()
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
            _myImage.enabled = false;
        }
    }
    public void OnEnableColor(string titleEror)
    {
        TextError.enabled = true;
        TextError.text = titleEror;
        _myImage.enabled = true;

        _myImage.color = Color.red;
        TextError.color = Color.white;
    }
}
