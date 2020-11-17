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
    private void OnEnable() => MainInput.input_MouseButtonDown0 += this.Fire;

    private ExplosiveObject _lastExplosion;
    private RetentionObject _lastRb;
    private void Fire()
    {
        if (Ammo > 0)
        {
            _inventory.GetItem(15, 1);//"Покупка патрона"
            _myAud.clip = FireClip;
            _myAud.Play();
            GameObject effect = Instantiate(_FireEffect, _FireEffect.transform.position, _FireEffect.transform.rotation);
            effect.SetActive(true);
            effect.transform.SetParent(transform);

            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (_lastExplosion = hit.transform.GetComponent<ExplosiveObject>())
                    Delay((int)(Vector3.Distance(hit.point, transform.position) * 2.5f),true);
                else if (_lastRb = hit.transform.GetComponent<RetentionObject>())
                {
                    Delay((int)(Vector3.Distance(hit.point, transform.position) * 2.5f),false);
                }

            }
            Destroy(effect, 0.1f);
           Ammo--;
        }
    }

    private async void Delay(int delay, bool isExplosion)
    {
        await Task.Delay(delay);
        if (isExplosion)
            _lastExplosion.Detonation();
        else
            _lastRb._myRb.AddForce(transform.forward * Damage, ForceMode.Impulse);
    }


    private void OnDisable() => MainInput.input_MouseButtonDown0 -= this.Fire;
}
