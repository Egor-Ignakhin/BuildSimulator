using System.Collections.Generic;
using UnityEngine;
namespace Dunamites
{
    public sealed class DunamiteManager : MonoBehaviour
    {
        internal delegate void ChangeList();
        internal static event ChangeList changeList;

        internal List<DunamiteClon> Dunamites = new List<DunamiteClon>();
        [SerializeField] internal AudioClip _boomClip;
        [SerializeField] internal AudioClip _timerTickClip;

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
                Dunamites[i]._isManagerStart = true;
                Dunamites[i].Detonation();
            }
            Dunamites.Clear();
            changeList?.Invoke();
        }
    }
}
