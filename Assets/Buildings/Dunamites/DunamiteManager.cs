using System.Collections.Generic;
using UnityEngine;
namespace Dunamites
{
    public sealed class DunamiteManager : MonoBehaviour
    {
        internal delegate void ChangeList();
        internal static event ChangeList changeList;

        internal List<DunamiteClon> Dunamites = new List<DunamiteClon>();
        [SerializeField] private AudioClip _boomClip;
        private BuildHouse _bh;
        private void Start()
        {
            _bh = FindObjectOfType<BuildHouse>();
            BuildHouse.chMode += this.ChangeColliders;
        }

        internal void AddInList(DunamiteClon dunamite)
        {
            Dunamites.Add(dunamite);
            changeList?.Invoke();
        }
        internal void RemoveInList(DunamiteClon dunamite)
        {
            Dunamites.Remove(dunamite);
            changeList?.Invoke();
        }

        internal void Detonation()
        {
            for (int i = 0; i < Dunamites.Count; i++)
            {
                Dunamites[i].Detonation(_boomClip);
            }
            Dunamites.Clear();
            changeList?.Invoke();
        }
        private void ChangeColliders()
        {
            for (int i = 0; i < Dunamites.Count; i++)
                Dunamites[i].MyBoxColl.enabled = _bh.CanActiveCollier;
        }
        private void OnDestroy() => BuildHouse.chMode -= this.ChangeColliders;
    }
}
