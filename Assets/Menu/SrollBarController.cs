using UnityEngine;
using UnityEngine.UI;

public sealed class SrollBarController : MonoBehaviour
{
    [SerializeField] private RectTransform objects;
    [SerializeField] private Scrollbar _scrollbar;

    public void ChangeValue() => objects.localPosition = new Vector2(0, 45 * _scrollbar.value * objects.childCount);
    private void Update()
    {
        if (Input.mouseScrollDelta.y >= 0.05f)
        {
            if (_scrollbar.value > 0)
            {
                _scrollbar.value -= 0.05f;
                ChangeValue();
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (_scrollbar.value <= 1)
            {
                _scrollbar.value += 0.05f;
                ChangeValue();
            }
        }
    }
}
