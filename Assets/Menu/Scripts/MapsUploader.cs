using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    sealed class MapsUploader : MonoBehaviour
    {
        private RectTransform _labelPref;// префаб кнопки, загружается из ресурсов

        private GameObject _importList;// объект для импорта
        private GameObject _exportList;// сам лист с мирами для экспорта

        // листы кнопок
        private readonly List<RectTransform> _labelsImport = new List<RectTransform>();
        private readonly List<RectTransform> _labelsExport = new List<RectTransform>();

        [SerializeField] private RectTransform _allExportTitles;
        [SerializeField] private GameObject _applyExport;

        [Space(5)]

        [SerializeField] private RectTransform _allImportTitles;
        [SerializeField] private GameObject _ApplyChangeExistingWorld;


        private string _lastPossibleWorld;
        private void Awake()
        {
            _labelPref = Resources.Load<GameObject>("Prefabs\\UploadLabel").GetComponent<RectTransform>();
            _exportList = _allExportTitles.parent.gameObject;
            _importList = _allImportTitles.parent.gameObject;
            Load();
        }

        private void Load()// метод загружает миры для экспорта(из сохранений)
        {
            string savePath = Directory.GetCurrentDirectory() + "\\Saves";
            if (!Directory.Exists(savePath)) // если папка с сохранениями не существует
            {
                Debug.Log("None Worlds");
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo($"{savePath}\\");

            List<string> allWorlds = new List<string>();
            FileInfo[] dinfo = directoryInfo.GetFiles();

            for (int k = 0; k < dinfo.Length; k++) // загружаем все доступные для переноса миры (доступность определяется наличием сохранённых объектов в мире)
            {
                if (Path.GetExtension(dinfo[k].Name) == ".txt")
                {
                    allWorlds.Add(dinfo[k].Name);
                    allWorlds[k] = allWorlds[k].Remove(allWorlds[k].LastIndexOf('.'));
                }
            }

            int labelCount = 0;
            for (int i = 0; i < allWorlds.Count; i++) // создаём кноки для миров
            {
                if (File.Exists($"{Directory.GetCurrentDirectory()}\\Saves\\obj\\{allWorlds[i]}.txt"))
                {
                    if (_labelsExport.Count < allWorlds.Count)
                    {
                        CreateField(true);
                        _labelsExport[labelCount++].GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = allWorlds[i];
                    }
                }
            }
        }


        private void ClickExport(string title)
        {
            _applyExport.SetActive(true);
            _lastPossibleWorld = title;

            _applyExport.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Export '{_lastPossibleWorld}' ?";
        }
        private void ClickImport(string title)
        {
            _lastPossibleWorld = title;
            if (File.Exists(Directory.GetCurrentDirectory() + "\\Saves\\" + _lastPossibleWorld + ".txt"))
            {
                _ApplyChangeExistingWorld.SetActive(true);
            }
            else
            {
                ImportWorld();
                ErrorImage.Instance.OnEnableColor("World was loaded");
            }
        }
        private readonly string _separator = "//";
        public void ExportWorld()// метод сливает два сохранения в файл
        {
            string path = Directory.GetCurrentDirectory() + "\\Saves";
            List<string> exportedSave = new List<string>();

            string[] _1 = File.ReadAllLines($"{path}\\{_lastPossibleWorld}.txt");
            for (int i = 0; i < _1.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(_1[i]))
                    exportedSave.Add(_1[i]);
            }

            exportedSave.Add(_separator);// разделение между сохранением персонажа и мира

            string[] _2 = File.ReadAllLines($"{path}\\obj\\{_lastPossibleWorld}.txt");
            for (int i = 0; i < _2.Length; i++)
                exportedSave.Add(_2[i]);


            string[] save = new string[exportedSave.Count];
            for (int i = 0; i < exportedSave.Count; i++)
                save[i] = exportedSave[i];

            File.WriteAllLines($"{Directory.GetCurrentDirectory()}\\Worlds\\ {_lastPossibleWorld}.bs", save);
            Debug.Log($"Export world  - {_lastPossibleWorld}");
            _applyExport.SetActive(false);

            ErrorImage.Instance.OnEnableColor($"World was saved in {Directory.GetCurrentDirectory()}\\Worlds\\{_lastPossibleWorld}.bs",true);
        }

        public void UpLoad() => _exportList.SetActive(true);

        public void DownLoad()// метод создаёт кнопки для импорта
        {
            _importList.SetActive(true);
            string path = Directory.GetCurrentDirectory() + "//Worlds";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            List<string> allWorlds = new List<string>();
            for (int i = 0; i < directoryInfo.GetFiles().Length; i++)
            {
                if (Path.GetExtension(directoryInfo.GetFiles()[i].Name) == ".bs")
                {
                    allWorlds.Add(directoryInfo.GetFiles()[i].Name);
                    allWorlds[i] = allWorlds[i].Remove(allWorlds[i].LastIndexOf('.'));
                }
            }

            for (int i = 0; i < allWorlds.Count; i++)
            {
                if (_labelsImport.Count < allWorlds.Count)
                    CreateField(false);
                _labelsImport[i].GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = allWorlds[i];
            }

        }
        private void ImportWorld()
        {
            string[] title = File.ReadAllLines($"{Directory.GetCurrentDirectory()}\\Worlds\\{_lastPossibleWorld}.bs");

            List<string> load = new List<string>();
            for (int i = 0; i < title.Length; i++)
            {
                if (title[i] != _separator)
                    load.Add(title[i]);
                else
                {
                    string[] personSave = new string[load.Count];

                    for (int k = 0; k < load.Count; k++)
                        personSave[k] = load[k];
                    load.Clear();

                    File.WriteAllLines($"{Directory.GetCurrentDirectory()}\\Saves\\{_lastPossibleWorld}.txt", personSave);// запись сохранения игрока
                }
            }
            string[] objSave = new string[load.Count];

            for (int k = 0; k < load.Count; k++)
                objSave[k] = load[k];

            File.WriteAllLines($"{Directory.GetCurrentDirectory()}\\Saves\\obj\\{_lastPossibleWorld}.txt", objSave);// запись сохранения игрока


            ErrorImage.Instance.OnEnableColor("World was loaded");
            _ApplyChangeExistingWorld.SetActive(false);
        }


        private void CreateField(bool isUpload)
        {
            RectTransform newLabel = Instantiate(_labelPref, Vector2.zero, Quaternion.identity, isUpload ? _allExportTitles : _allImportTitles);

            newLabel.localPosition = new Vector2(0, 200);
            newLabel.localScale = new Vector2(9, 0.9f);
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };

            EventTrigger.Entry enter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };

            EventTrigger.Entry exit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };

            if (isUpload)
            {
                if (_labelsExport.Count > 0)
                {
                    newLabel.localScale = _labelsExport[_labelsExport.Count - 1].localScale;
                    newLabel.localPosition = new Vector2(_labelsExport[_labelsExport.Count - 1].localPosition.x, _labelsExport[_labelsExport.Count - 1].localPosition.y - 2 * 52.5f);
                }

                entry.callback.AddListener((data) => { ClickExport(newLabel.GetChild(0).GetComponent<TextMeshProUGUI>().text); });
                _labelsExport.Add(newLabel);
            }
            else
            {
                if (_labelsImport.Count > 0)
                {
                    newLabel.localScale = _labelsExport[_labelsExport.Count - 1].localScale;
                    newLabel.localPosition = new Vector2(_labelsImport[_labelsImport.Count - 1].localPosition.x, _labelsImport[_labelsImport.Count - 1].localPosition.y - 2 * 52.5f);
                }

                entry.callback.AddListener((data) => { ClickImport(newLabel.GetChild(0).GetComponent<TextMeshProUGUI>().text); });
                _labelsImport.Add(newLabel);
            }

            enter.callback.AddListener((data) => { newLabel.GetComponent<Image>().color = Color.yellow; });
            exit.callback.AddListener((data) => { newLabel.GetComponent<Image>().color = Color.white; });

            newLabel.GetComponent<EventTrigger>().triggers.Add(entry);
            newLabel.GetComponent<EventTrigger>().triggers.Add(enter);
            newLabel.GetComponent<EventTrigger>().triggers.Add(exit);
        }
    }
}