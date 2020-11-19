using UnityEngine;

public sealed class CameraMove : MonoBehaviour
{
    public float Speed { get; private set; }
    private Vector3 _transfer;

    private void Awake()
    {
        Camera.main.orthographic = false;
    }

    private void OnEnable()
    {
        float value = Settings.AdvancedSettings.FlyingSpeed;
        if (value == 0)
            this.Speed = 5;
        else
            this.Speed = Settings.AdvancedSettings.FlyingSpeed == 1 ? 10 : 20;
    }

    private void Update()
    {
        if (! GameMenu.ActiveGameMenu)
        {
            Vector3 Speed = new Vector3(0, this.Speed, 0);

            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += Speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.position -= Speed * Time.deltaTime;
            }

            // Ускорение при нажатии клавиши Shift
            if (Input.GetKeyDown(KeyCode.LeftShift))
                this.Speed *= 3.5f;
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                this.Speed /= 3.5f;

            // перемещение камеры
            _transfer = transform.forward * Input.GetAxis("Vertical");
            _transfer += transform.right * Input.GetAxis("Horizontal");
            transform.position += _transfer * this.Speed * Time.deltaTime;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
