using UnityEngine;

namespace Dunamites
{
    public sealed class DunamiteInteract : Interacteble
    {
        internal DunamiteClon MyDunamite { get; set; }
        public override void Interact(InputPlayer inputPlayer)
        {
            inputPlayer.HelpingText.text = "Change timer [" + inputPlayer._getItemKey + ']';
            if (Input.GetKeyDown(inputPlayer._getItemKey))
            {
                inputPlayer.DunamiteFieldCs.gameObject.SetActive(true);
                inputPlayer.DunamiteFieldCs.GetDunamite(MyDunamite);
            }
        }
    }
}