using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dunamites
{
    public sealed class DunamiteClon : MonoBehaviour
    {
        private DunamiteManager _manager;
        private AudioSource _childAud;
        internal ObjectDown _objectDown { get; set; }
        internal BoxCollider MyBoxColl { get; private set; }

        private float timerToExplosion;
        internal float TimerToExplosion
        {
            get => timerToExplosion;
            set
            {
                timerToExplosion = value;
                if (value < 0)
                    value = 0;

                _textTimer.text = value > 10? "00 : " + System.Math.Round(value,1): "00 : 0" + System.Math.Round(value, 1);
            }
        }

        private TMPro.TextMeshPro _textTimer;

        private void Start()
        {
            _objectDown = FindObjectOfType<ObjectDown>();
            _manager = FindObjectOfType<DunamiteManager>();
            _manager.AddInList(this);

            _childAud = transform.GetChild(1).GetComponent<AudioSource>();
            MyBoxColl = transform.parent.GetComponent<BoxCollider>();
            _textTimer = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
            _textTimer.color = new Color(0, 0.5f, 0);
            TimerToExplosion = 0;
        }

        internal void Detonation(AudioClip clip)
        {
            _childAud.clip = clip;
         
            StartCoroutine(nameof(CheckFinishAudio));            
        }

        private void Find()
        {
            List<BaseBlock> objects = _objectDown.GetNearestObject(transform.parent.position,4);

            for (int i = 0; i < objects.Count; i++)
                objects[i].Destroy(4);
        }


        private void OnDestroy() => _manager.RemoveInList(this);

        IEnumerator CheckFinishAudio()
        {
            while(TimerToExplosion > 0)
            {
                TimerToExplosion -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(1).SetParent(_objectDown.transform);
            _childAud.Play();
            Find();
            GetComponent<Renderer>().enabled = false;
            transform.parent.GetComponent<Renderer>().enabled = false;
            while (_childAud.isPlaying)
            yield return new WaitForSeconds(_childAud.clip.length);
            Destroy(_childAud.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }
}