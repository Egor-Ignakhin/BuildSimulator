using UnityEngine;

sealed class ChankVisible : MonoBehaviour
{
    private CapsuleCollider _playerCollider;
    private GameObject _activerLods, _activerBlocks;
    private void Start()
    {
        this._playerCollider = FindObjectOfType<SeePlayerChanks>().Collider;
        _activerLods = transform.GetChild(0).gameObject;
        _activerBlocks = transform.GetChild(1).gameObject;
        GetComponent<BoxCollider>().isTrigger = true;
        gameObject.layer = 2;//ignore raycast
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == _playerCollider)
        {
            _activerBlocks.SetActive(true);
            _activerLods.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == _playerCollider)
        {
            _activerBlocks.SetActive(false);
            _activerLods.SetActive(false);
        }
    }
}
