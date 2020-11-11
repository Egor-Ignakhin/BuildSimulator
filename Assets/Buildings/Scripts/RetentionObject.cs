using UnityEngine;

public sealed class RetentionObject : Interacteble// навесив этот класс на объект, будет возможно его перемещать
{
    private Rigidbody _myRb;

    private void Start() => _myRb = GetComponent<Rigidbody>();

    public override void Interact(InputPlayer inputPlayer)
    {
        inputPlayer.HelpingText.text = "Hold [" + inputPlayer._getItemKey + ']';

        if (Input.GetKey(inputPlayer._getItemKey))        
            inputPlayer.HoldObject(_myRb);        
        else
        {
            inputPlayer.CanHolding = true;
            inputPlayer.IsStartHold = true;
            _myRb.useGravity = true;
        }
    }
}