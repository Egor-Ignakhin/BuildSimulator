using UnityEngine;

sealed class ChankVisible : MonoBehaviour
{
    public CapsuleCollider _playerCollider;
    public GameObject _activer;
    private void Start()
    {
        this._playerCollider = FindObjectOfType<SeePlayerChanks>().Collider;
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name == "Plane")//only for dev
            {
                _activer = transform.GetChild(i).gameObject;
            }
        }
        GetComponent<BoxCollider>().isTrigger = true;
        gameObject.layer = 3;//ignore raycast
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other == _playerCollider)
        {
            _activer.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == _playerCollider)
        {
            _activer.SetActive(false);
        }
    }

}
