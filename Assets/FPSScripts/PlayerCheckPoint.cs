using UnityEngine;

public sealed class PlayerCheckPoint : MonoBehaviour
{
    public Transform LastCheckPoint;
    private Collider _player;

    private void Start() => _player = FindObjectOfType<FirstPersonController>().GetComponent<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other == _player)
            _player.transform.position = LastCheckPoint == null ? new Vector3(0,1,0) : LastCheckPoint.position;
    }
}
