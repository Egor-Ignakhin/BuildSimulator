using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public sealed class WorldLabel : MonoBehaviour
    {
        internal WorldLoader Loader { get; set; }
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


            EventTrigger ev = GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => { Click(); });
            ev.triggers.Add(entry);

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<Button>())
                    transform.GetChild(i).GetComponent<Button>().onClick.AddListener(Delete);
            }
        }
        private void Click() => Loader.LoadWorld(Title);

        private void Delete() => Loader.DeleteWorld(Title);
    }
}