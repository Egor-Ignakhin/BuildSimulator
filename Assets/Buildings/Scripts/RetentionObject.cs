using UnityEngine;

public sealed class RetentionObject : Interacteble// навесив этот класс на объект, будет возможно его перемещать
{
    private Rigidbody _myRb;
    private float _force;

    private void Start() => _myRb = GetComponent<Rigidbody>();

    public override void Interact(InputPlayer inputPlayer)
    {
        inputPlayer.HelpingText.text = "Hold [" + inputPlayer._getItemKey + ']';

        inputPlayer._holdSlider.localScale = new Vector2(_force * 0.1f, 1);//сила показывается слайдером
        inputPlayer.HoldSliderParent.SetActive(_force > 0.005f);// включаем если сила больше чем чуть чуть

        if (Input.GetMouseButton(2))
        {
            if (_force < 10)
                _force += Time.deltaTime;
        }
        else
        {
            if (_force > 0)
                _force -= Time.deltaTime;
        }

        if (Input.GetKey(inputPlayer._getItemKey))        
            inputPlayer.HoldObject(_myRb);        
        else
        {
            inputPlayer.CanHolding = true;
            inputPlayer.IsStartHold = true;
            _myRb.useGravity = true;
            _myRb.AddForce(inputPlayer.transform.forward * _force * 100);
            _force = 0;
            inputPlayer._holdSlider.localScale = new Vector2(_force * 0.1f, 1);
            inputPlayer.HoldSliderParent.SetActive(false);
        }
    }
}