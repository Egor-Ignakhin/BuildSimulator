using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Pistol : Gun
{
    private Camera _cam;
    private AudioSource _myAud;
    [SerializeField] private Transform _spawnPlace;
    [SerializeField] private GameObject _FireEffect;
    private void Awake()
    {
        _cam = Camera.main;
        _myAud = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        MainInput.input_MouseButtonDown0 += this.Fire;  
    }
    private FlameBarrel _lastFlameBarrel;
    private void Fire()
    {
        if (Ammo > 0)
        {
            _myAud.clip = FireClip;
            _myAud.Play();
            GameObject effect = Instantiate(_FireEffect, _FireEffect.transform.position, _FireEffect.transform.rotation);
            effect.SetActive(true);

            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (_lastFlameBarrel = hit.transform.GetComponent<FlameBarrel>())
                {
                    _lastFlameBarrel.Detonation();
                }

            }
            Destroy(effect, 0.1f);
            DateTime a = DateTime.Now;
           Ammo--;
        }
    }
    private void OnDisable()
    {
        MainInput.input_MouseButtonDown0 -= this.Fire;
    }
}
