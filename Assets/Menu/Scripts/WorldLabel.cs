using UnityEngine;
using UnityEngine.EventSystems;
namespace MainMenu
{
    public sealed class WorldLabel : MonoBehaviour
    {
        internal WorldLoader _loader { get; set; }
        private string _title;
        internal string Title
        {
            get => _title;

            set
            {
                _title = value;
                tTltTxt.text = value;
            }
        }
        private TMPro.TextMeshProUGUI tTltTxt;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (tTltTxt = transform.GetChild(i).GetComponent<TMPro.TextMeshProUGUI>())
                    break;
            }


            EventTrigger ev = gameObject.GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => { Click(); });
            ev.triggers.Add(entry);
        }
        private void Click()
        {
            _loader.LoadWorld(this);
        }
    }
}