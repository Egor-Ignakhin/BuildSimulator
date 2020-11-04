using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dunamites
{
    public sealed class DunamiteClon : MonoBehaviour
    {
        private DunamiteManager _manager;
        private AudioSource _parentAud;
        internal ObjectDown _objectDown { get; set; }
        internal BoxCollider MyBoxColl { get; private set; }

        private float timerToExplosion;
        internal float TimerToExplosion
        {
            get => timerToExplosion;
            set
            {
                timerToExplosion = value;

                _textTimer.text = value > 10? "00 : " + System.Math.Round(value,1): "00 : 0" + System.Math.Round(value, 1);
            }
        }

        private TMPro.TextMeshPro _textTimer;

        private void Start()
        {
            _objectDown = FindObjectOfType<ObjectDown>();
            _manager = FindObjectOfType<DunamiteManager>();
            _manager.AddInList(this);

            _parentAud = transform.parent.GetComponent<AudioSource>();
            MyBoxColl = transform.parent.GetComponent<BoxCollider>();
            _textTimer = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
            _textTimer.color = new Color(0, 0.5f, 0);
            TimerToExplosion = 0;
        }

        internal void Detonation(AudioClip clip)
        {
            _parentAud.clip = clip;
         
            StartCoroutine(nameof(CheckFinishAudio));            
        }

        private void Find()
        {
            List<BaseBlock> objects = _objectDown.GetNearestObject(transform.parent.position);

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Destroy();
            }
        }


        private void OnDestroy()
        {
            _manager.RemoveInList(this);
        }

        IEnumerator CheckFinishAudio()
        {
            while(TimerToExplosion > 0)
            {
                TimerToExplosion -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            transform.GetChild(0).gameObject.SetActive(false);
            _parentAud.Play();
            Find();
            GetComponent<Renderer>().enabled = false;
            transform.parent.GetComponent<Renderer>().enabled = false;
            while (_parentAud.isPlaying)
            yield return new WaitForSeconds(_parentAud.clip.length);
            Destroy(transform.parent.gameObject);
        }
    }
}