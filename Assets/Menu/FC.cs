using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FC : MonoBehaviour
{
    private Transform _cam;
    private float _speed = 0.02f;
    private MeshRenderer _myRend;
    void Start()
    {
        _myRend = GetComponent<MeshRenderer>();
        _cam = Camera.main.transform;
        StartCoroutine(nameof(Rotator));       
    }
    private IEnumerator Rotator()
    {
        while (true)
        {
            if( _cam.position.y > 800)
            {
                _myRend.enabled = true;
                _speed = 0.02f;
            }
            else
            {
                _myRend.enabled = false;
                _speed = 5;
            }
            transform.LookAt(_cam);
            transform.eulerAngles += new Vector3(180, 0, 180);
            yield return new WaitForSeconds(_speed);
        }
    }
}
