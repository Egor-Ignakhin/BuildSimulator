using UnityEngine;
namespace Dunamites
{
    public sealed class DunamiteClon : ExplosiveObject
    {
        private DunamiteManager _manager;
        private AudioSource _childAud;
        private AudioClip _boom, _tick;
        internal BoxCollider MyBoxColl { get; private set; }

        internal override byte Type => 3;

        private float timerToExplosion;
        internal float TimerToExplosion
        {
            get => timerToExplosion;
            set
            {
                timerToExplosion = value;
                if (value < 0)
                    value = 0;

                _textTimer.text = value >= 10 ? "00 : " + System.Math.Round(value, 1) : "00 : 0" + System.Math.Round(value, 1);
            }
        }

        private TMPro.TextMeshPro _textTimer;

        private void Awake()
        {
            Raduis = 4;
            RaduisExplosion = 8;
            Power = 4;            
        }
        protected override void Start()
        {
            base.Start();
            _manager = FindObjectOfType<DunamiteManager>();
            _manager.AddInList(this);

            _childAud = transform.GetChild(1).GetComponent<AudioSource>();
            MyBoxColl = transform.GetComponent<BoxCollider>();
            _textTimer = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
            _textTimer.color = new Color(0, 0.5f, 0);
            TimerToExplosion = 0;
            GetComponent<MeshRenderer>().sharedMaterial.color= Color.white;


            _tick = _manager._timerTickClip;
            _boom = _manager._boomClip;
            gameObject.AddComponent<DunamiteInteract>().MyDunamite = this;
            Destroy(GetComponent<BaseBlock>());         
        }
        internal bool IsManagerStart { get; set; }

        internal override void Detonation()
        {
            if(IsManagerStart == false)
                TimerToExplosion = 0;
            IsManagerStart = false;

            _childAud.clip = _tick;
            sec = TimerToExplosion;
            InvokeRepeating(nameof(CheckFinishAudio), 0.1f, 0.1f);
        }

        protected override void FindNearestObjects() => base.FindNearestObjects();


        protected override void OnDestroy()
        {
            base.OnDestroy();
            _manager.RemoveInList(this);
        }
        float sec;
        private void CheckFinishAudio()
        {           
            if (TimerToExplosion > 0)
            {                
                if (TimerToExplosion < sec || TimerToExplosion < 1)
                {
                    _childAud.Play();
                    
                    sec--;
                }

                TimerToExplosion -= 0.1f;
                return;
            }
            FindNearestObjects();
            transform.GetChild(0).gameObject.SetActive(false);
            _childAud.transform.SetParent(_objectDown.transform);
            _childAud.transform.GetChild(0).gameObject.SetActive(true);
            _childAud.clip = _boom;
            _childAud.Play();
            _childAud.enabled = true;
            GetComponent<Renderer>().enabled = false;
            Destroy(_childAud.gameObject, _childAud.clip.length + 1);
            Destroy(gameObject, _childAud.clip.length + 1);
            CancelInvoke();
        }
    }
}