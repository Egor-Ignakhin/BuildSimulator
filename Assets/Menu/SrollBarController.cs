using UnityEngine;
using UnityEngine.UI;

public sealed class SrollBarController : MonoBehaviour
{
    [SerializeField] private RectTransform objects;
    [SerializeField] private Scrollbar _scrollbar;
    private MainInput _input;

    private void Start()
    {
        _input = MainInput.Instance;
        MainInput.mouseSrollMax += this.UpScroll;
        MainInput.mouseSrollMin += this.DownScroll;
    }
    public void ChangeValue() => objects.localPosition = new Vector2(0, 45 * _scrollbar.value * objects.childCount);
    private void UpScroll()
    {
        if (_scrollbar.value > 0)
        {
            _scrollbar.value -= 0.05f;
            ChangeValue();
        }
    }
    private void DownScroll()
    {
        if (_scrollbar.value <= 1)
        {
            _scrollbar.value += 0.05f;
            ChangeValue();
        }
    }
    private void OnDestroy()
    {
        MainInput.mouseSrollMax -= this.UpScroll;
        MainInput.mouseSrollMin -= this.DownScroll;
    }
}
