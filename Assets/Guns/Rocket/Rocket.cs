using System.Threading.Tasks;
using UnityEngine;

namespace Guns
{
    public sealed class Rocket : ExplosiveObject
    {
        [SerializeField] private AudioSource _myAud;
        internal AudioClip DetonationClip { get; set; }
        [SerializeField] private GameObject _detonationEffect;
        [SerializeField] private GameObject _flyingEffect;
        internal RocketLauncher Launcher { get; set; }
        internal int LifeTime { get; set; } = 0;

        internal override byte Type => 255;

        private void Awake()
        {
            Raduis = 2.2f;
            RaduisExplosion = 4;
            Power = 2.75f;
        }
        protected override void Start()
        {
            base.Start();
            _myRb = GetComponent<Rigidbody>();
            Falling();
        }

        private Rigidbody _myRb;
        private async void Falling()
        {
            while (true)
            {
                if (_myRb)
                {
                    _myRb.AddForce(-transform.forward*0.15f, ForceMode.Impulse);
                    await Task.Delay(100);
                }
                else break;
            }
        }


        private void OnCollisionEnter(Collision collision) => Detonation();
        internal override void Detonation()
        {
            _objectDown.Explosives.Remove(this);

            _myAud.clip = DetonationClip;
            _myAud.Play();
            _myAud.transform.SetParent(_objectDown.transform);
            _detonationEffect.SetActive(true);
            _flyingEffect.SetActive(false);
            Destroy(_myAud.gameObject, _myAud.clip.length - 1);

            FindNearestObjects();
        }

        protected override void FindNearestObjects()
        {
            Launcher.ChangeRocketLength(this);
            base.FindNearestObjects();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}