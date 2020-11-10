using UnityEngine;

public sealed class FpsCount : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    private void Start() => InvokeRepeating(nameof(UpdateCounter), 1, 1);
    private void UpdateCounter() => text.text = System.Math.Round(1 / Time.deltaTime).ToString();
}
