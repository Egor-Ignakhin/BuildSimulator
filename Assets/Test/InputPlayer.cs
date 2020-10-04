using UnityEngine;

public class InputPlayer : MonoBehaviour
{
    [SerializeField] private GameObject AnimCircle,AnimBuild;
    private BuildHouse _bH;

   private void Start()
    {
        _bH = GetComponent<BuildHouse>();
    }

    private void Update()
    {
        if (_bH._isDestroy)
        {
            AnimCircle.SetActive(true);
        }
        else
        {
            AnimCircle.SetActive(false);
        }
        if (_bH._isBuild)
        {
            AnimBuild.SetActive(true);

        }
        else
        {
            AnimBuild.SetActive(false);
        }
    }
}
