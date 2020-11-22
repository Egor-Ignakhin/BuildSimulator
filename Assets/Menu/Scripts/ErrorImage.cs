using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets;
public sealed class ErrorImage : Singleton<ErrorImage>
{
    private Image _myImage;
    private TextMeshProUGUI TextError;
    Color color = new Color(0, 0, 0, -0.0072375f);
    bool _isDontDestroy;
    private void Awake()
    {
        if (!_isDontDestroy)
            DontDestroyOnLoad(gameObject);
        _isDontDestroy = true;
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
        TextError.alignment = TextAlignmentOptions.Midline;
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
    public void OnEnableColor(string titleEror, bool isImportantInf = false)
    {
        if (isImportantInf)
        {
            color = new Color(0, 0, 0, -0.0022375f);
            TextError.color = Color.red;
            _myImage.color = Color.white;
        }
        else
        {
            color = new Color(0, 0, 0, -0.0072375f);
            TextError.color = Color.white;
            _myImage.color = Color.red;
        }
        if (!TextError)
            Awake();


        this.enabled = true;
        TextError.enabled = true;
        TextError.text = titleEror;
        _myImage.enabled = true;
    }
}
