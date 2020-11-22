using UnityEngine;
using UnityEngine.UI;

sealed class ScrollBarController : MonoBehaviour
{
    [SerializeField] private RectTransform objects;
    private Scrollbar _scrollbar;
    private MainInput _input;
    [SerializeField] private float MaxY; // за выход за границу кнопка выключается
    [SerializeField] private float MinY; // тоже самое

    private void Start()
    {
        _scrollbar = GetComponent<Scrollbar>();
        _scrollbar.onValueChanged.AddListener((float val) => ChangeValue());

        _input = MainInput.Instance;
        MainInput.mouseSrollMax += this.UpScroll;
        MainInput.mouseSrollMin += this.DownScroll;

        CheckPositions();
    }
    private void ChangeValue()
    {
        objects.localPosition = new Vector2(0, 45 * _scrollbar.value * objects.childCount);
        CheckPositions();
    }
    private void UpScroll()
    {
        if (_scrollbar.value > 0)
            _scrollbar.value -= 0.05f;
    }
    private void DownScroll()
    {
        if (_scrollbar.value <= 1)
            _scrollbar.value += 0.05f;
    }

    private void CheckPositions()
    {
        if (MaxY != 0)
        {
            for (int i = 0; i < objects.childCount; i++)
            {
                objects.GetChild(i).gameObject.SetActive(!(objects.GetChild(i).position.y > MaxY));
                if (MinY != 0)
                {
                    if (objects.GetChild(i).position.y < MinY)
                        objects.GetChild(i).gameObject.SetActive(objects.GetChild(i).position.y > MinY);
                }
            }
        }
    }

    private void OnDestroy()
    {
        MainInput.mouseSrollMax -= this.UpScroll;
        MainInput.mouseSrollMin -= this.DownScroll;
    }
}