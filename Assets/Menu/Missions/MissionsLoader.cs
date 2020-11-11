using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public sealed class MissionsLoader : MonoBehaviour
    {
        private const int _countMissions = 3;
        private string[] _nameScenes = new string[_countMissions];
        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                EventTrigger ev = transform.GetChild(i).gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry click = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerClick
                };

                click.callback.AddListener((data) => { LoadMission(ev.name); });
                ev.triggers.Add(click);
            }
            _nameScenes[0] = "OneMission";
            _nameScenes[1] = "SecondMission";
            _nameScenes[2] = "ThreeMission";
        }
        public void LoadMission(string name)//загружает миссии исходя из названий дочернего объекта
        {
            MenuManager._playerSource.Play();
            int.TryParse(name, out int numScene);
            Debug.Log(name);
            SceneManager.LoadScene(_nameScenes[numScene]);
        }
    }
}