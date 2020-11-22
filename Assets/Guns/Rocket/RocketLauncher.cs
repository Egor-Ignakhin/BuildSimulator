using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Guns
{
    public sealed class RocketLauncher : MonoBehaviour
    {
        private readonly List<Rocket> _bullets = new List<Rocket>();
        internal int Rockets { get; set; }
        [SerializeField] private GameObject _spawnRocket;
        private Camera _cam;
        private ObjectDown _objectDown;
        [SerializeField] private AudioClip _rocketFlying;
        [SerializeField] private AudioClip _rocketDetonation;
        private InventoryAndItems.Inventory _inventory;

        private readonly int _maxLifeRocket = 10;
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

        internal void ChangeRocketLength(Rocket bullet) => _bullets.Remove(bullet);

        private Rigidbody _lastRocket;
        private async void StartRocket()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);//получаем луч направления
            GameObject newRocket = Instantiate(_spawnRocket, _spawnRocket.transform.position, _spawnRocket.transform.rotation, _objectDown.transform);// создаём ракету
            newRocket.SetActive(true);

            Rocket newBullet = newRocket.GetComponent<Rocket>();// кеш
            newBullet.DetonationClip = _rocketDetonation;// устанавливаемый клип полёта
            newBullet.Launcher = this;
            _lastRocket = newRocket.AddComponent<Rigidbody>();// кеш
            _lastRocket.useGravity = false;

            float multiply = 30;

            _lastRocket.AddTorque(Vector3.Cross(newRocket.transform.forward, ray.direction) * 5);// реализуем физический поврот ракеты со временем
            _lastRocket.AddForce(ray.direction * multiply, ForceMode.Impulse);// выстрел
            _bullets.Add(newBullet);


            await Task.Delay(100);
            newRocket.GetComponent<MeshCollider>().isTrigger = false;// включение коллайдера, что бы игрок не спотыкался
        }
        private void BulletChecker()
        {
            for (int i = 0; i < _bullets.Count; i++)// если ракета уже пролета максимально возможное время  
            {
                if (_bullets[i].LifeTime++ > _maxLifeRocket) // детонируем
                    _bullets[i].Detonation();
            }
        }

        private void OnDisable() => MainInput.input_MouseButtonDown0 -= this.Fire;
    }
}