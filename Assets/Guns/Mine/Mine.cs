using System.Threading.Tasks;
using UnityEngine;
namespace Guns
{

    public sealed class Mine : ExplosiveObject
    {
        private AudioSource _myAud;
        private Rigidbody _myRb;
        private float _sensivity = 1;

        internal override byte Type => 5;

        private void Awake()
        {
            Raduis = 2;
            RaduisExplosion = 3.75f;
            Power = 4;
        }
        protected override void Start()
        {
            base.Start();
            _myAud = transform.GetChild(0).GetComponent<AudioSource>();
            _myAud.clip = FindObjectOfType<MinesManager>().DetonationClip;
            _myRb = gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<RetentionObject>();
            transform.SetParent(_objectDown.transform);
            Destroy(GetComponent<BaseBlock>());
            GetComponent<MeshCollider>().isTrigger = false;
            SleppForPlaint();
        }
        protected override void FindNearestObjects() => base.FindNearestObjects();

        private async void SleppForPlaint()
        {
            await Task.Delay(500);
            _sensivity = 0.0005f;
        }
        internal override void Detonation()
        {
            _objectDown.Explosives.Remove(this);

            _myAud.gameObject.SetActive(true);
            _myAud.Play();
            _myAud.transform.SetParent(_objectDown.transform);
            Destroy(_myAud.gameObject, _myAud.clip.length + 2);
            FindNearestObjects();
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (_myRb.velocity.magnitude >= _sensivity)
                Detonation();
        }
        protected override void OnDestroy() => base.OnDestroy();
    }
}