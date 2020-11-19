using System.Collections.Generic;
using UnityEngine;

public sealed class RocketLauncher : MonoBehaviour
{
    private readonly List<Transform> _bullets = new List<Transform>();
    internal int Rockets { get; set; }
    [SerializeField] private GameObject _spawnRocket;
    [SerializeField] private Transform _instatiatePlace;
    private Camera _cam;
    private ObjectDown _objectDown;
    [SerializeField] private AudioClip _rocketFlying;
    [SerializeField] private AudioClip _rocketDetonation;
    private InventoryAndItems.Inventory _inventory;
    private void OnEnable()
    {
        MainInput.input_MouseButtonDown0 += this.Fire;
        _objectDown = ObjectDown.Instance;
        _inventory = InventoryAndItems.Inventory.Instance;
    }
    private void Start()
    {
        _cam = Camera.main;
        InvokeRepeating(nameof(BulletChecker), 1, 1);
    }

    private void Fire()
    {
        if (Rockets > 0)
        {
            StartRocket();
            _inventory.GetItem(13, 1);//"Покупка ракеты"
            Rockets--;
        }
    }

    internal void ChangeRocketLength(Rocket bullet) => _bullets.Remove(bullet.transform);

    private void StartRocket()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        GameObject newRocket = Instantiate(_spawnRocket, _instatiatePlace.position, _instatiatePlace.rotation);
        newRocket.SetActive(true);

        Rocket newBullet = newRocket.GetComponent<Rocket>();
        newBullet.DetonationClip = _rocketDetonation;
        newBullet.Launcher = this;
        newRocket.AddComponent<Rigidbody>().useGravity = false;

        float multiply = 50;

        newRocket.GetComponent<Rigidbody>().AddForce(ray.direction * multiply, ForceMode.Impulse);
        _bullets.Add(newRocket.transform);
    }
    private void BulletChecker()
    {
        for (int i = 0; i < _bullets.Count; i++)
        {
            if (Vector3.Distance(_bullets[i].position, _bullets[i].GetComponent<Rocket>().StartPosition) > 300)
                _bullets[i].GetComponent<Rocket>().Detonation();
        }
    }

    private void OnDisable() => MainInput.input_MouseButtonDown0 -= this.Fire;
}