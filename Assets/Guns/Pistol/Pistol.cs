using InventoryAndItems;
using System.Threading.Tasks;
using UnityEngine;
namespace Guns
{
    public sealed class Pistol : Gun
    {
        private Camera _cam;// камера для аускания луча
        private AudioSource _myAud;
        [SerializeField] private Transform _spawnPlace;// место появления "пули"
        [SerializeField] private GameObject _FireEffect;

        [SerializeField] private GameObject _ConcreteDecalEffect;
        [SerializeField] private GameObject _DirtDecalEffect;
        [SerializeField] private GameObject _SoftBodydecalEffect;
        [SerializeField] private GameObject _WoodDecalEffect;
        private Inventory _inventory;
        private Animator _myAnimator;
        private void Awake()
        {
            _cam = Camera.main;
            _myAud = GetComponent<AudioSource>();
            _inventory = Inventory.Instance;
            Damage = 1;
            _myAnimator = GetComponent<Animator>();
        }
        private void OnEnable()
        {
            _myAnimator.SetBool("IsFire", false);
            MainInput.input_MouseButtonDown0 += this.Fire;
        } // подписка на событие клика нулевой кнопки

        private ExplosiveObject _lastExplosion;// кеш для оптимизации
        private RetentionObject _lastRb;// кеш для оптимизации
        private BaseBlock _lastBlock;
        private bool _canFire = true;
        private async void Fire()
        {
            if (!_canFire)
                return;
            if (Ammo > 0)
            {
                _canFire = false;
                _myAnimator.SetBool("IsFire", true);
                await Task.Delay(333);// простой, что бы подождать  момент выстрела
                if (!gameObject.activeInHierarchy)
                    return;
                _inventory.GetItem(15, 1);//"Покупка патрона"
                _myAud.clip = FireClip;
                _myAud.Play();
                GameObject effect = Instantiate(_FireEffect, _FireEffect.transform.position, _FireEffect.transform.rotation);// создание эффекта выстрела
                effect.SetActive(true);
                effect.transform.SetParent(transform);
                Destroy(effect, 0.12f);

                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (_lastExplosion = hit.transform.GetComponent<ExplosiveObject>())
                        Delay((int)(Vector3.Distance(hit.point, transform.position) * 2.5f), true);
                    else if (_lastRb = hit.transform.GetComponent<RetentionObject>())
                        Delay((int)(Vector3.Distance(hit.point, transform.position) * 2.5f), false);

                    await Task.Delay((int)(Vector3.Distance(hit.point, transform.position) * 2.5f));
                    if (!gameObject.activeInHierarchy)
                        return;
                    GameObject decalEffect = null;
                    if (_lastBlock = hit.transform.GetComponent<BaseBlock>())
                    {
                        if (_lastBlock.Type == 0)
                            decalEffect = Instantiate(_ConcreteDecalEffect, hit.point, hit.transform.rotation);// создание эффекта попадания
                        else if (_lastBlock.Type == 1)// wood
                            decalEffect = Instantiate(_WoodDecalEffect, hit.point, hit.transform.rotation);// создание эффекта попадания
                        else if (_lastBlock.Type == 2)// glass
                            decalEffect = Instantiate(_SoftBodydecalEffect, hit.point, hit.transform.rotation);// создание эффекта попадания
                        else
                            decalEffect = Instantiate(_DirtDecalEffect, hit.point, hit.transform.rotation);// создание эффекта попадания
                    }
                    if (hit.transform.GetComponent<FirstPersonController>())
                        decalEffect = Instantiate(_SoftBodydecalEffect, hit.point, hit.transform.rotation);// создание эффекта попадания
                    else if (decalEffect == null)
                        decalEffect = Instantiate(_DirtDecalEffect, hit.point, hit.transform.rotation);// создание эффекта попадания

                    decalEffect.SetActive(true);
                    decalEffect.transform.SetParent(hit.transform);
                    decalEffect.transform.forward = hit.normal;
                    decalEffect.GetComponent<ParticleSystem>().Play();
                    Destroy(decalEffect, decalEffect.GetComponent<ParticleSystem>().main.duration);
                    _myAnimator.SetBool("IsFire", false);
                }
                Ammo--;
                await Task.Delay(200);
                _canFire = true;
            }
            else
            {
                _myAud.clip = EmptyClick;
                _myAud.Play();
                _myAnimator.SetBool("IsFire", false);
            }
        }

        private async void Delay(int delay, bool isExplosion)
        {
            await Task.Delay(delay);
            if (!gameObject.activeInHierarchy)
                return;

            if (isExplosion)// взорвём объект, если можно
                _lastExplosion.Detonation();
            else
                _lastRb._myRb.AddForce(transform.forward * Damage, ForceMode.Impulse);
        }


        private void OnDisable()
        {
            _canFire = true;
            MainInput.input_MouseButtonDown0 -= this.Fire;
            _myAnimator.SetBool("IsFire", false);
        }
    }
}