using InventoryAndItems;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Pistol : Gun
{
    private Camera _cam;
    private AudioSource _myAud;
    [SerializeField] private Transform _spawnPlace;
    [SerializeField] private GameObject _FireEffect;
    private Inventory _inventory;
    private void Awake()
    {
        _cam = Camera.main;
        _myAud = GetComponent<AudioSource>();
        _inventory = Inventory.Instance;
        Damage = 1;
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
            _inventory.GetItem(15, 1);//"Покупка патрона"
            _myAud.clip = FireClip;
            _myAud.Play();
            GameObject effect = Instantiate(_FireEffect, _FireEffect.transform.position, _FireEffect.transform.rotation);
            effect.SetActive(true);

            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (_lastFlameBarrel = hit.transform.GetComponent<FlameBarrel>())
                {
                    Delay((int)(Vector3.Distance(hit.point, transform.position) * 2.5f));
                }

            }
            Destroy(effect, 0.1f);
           Ammo--;
        }
    }

    private async void Delay(int delay)
    {
        await Task.Delay(delay);

        _lastFlameBarrel.Detonation();
    }


    private void OnDisable()
    {
        MainInput.input_MouseButtonDown0 -= this.Fire;
    }
}
