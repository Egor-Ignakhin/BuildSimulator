using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 transfer;

    private void Awake()
    {
        Camera.main.orthographic = false;
    }


    private void Update()
    {
        if (! GameMenu.ActiveGameMenu)
        {
            Vector3 Speed = new Vector3(0, speed, 0);

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
                speed *= 5;
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                speed /= 5;

            // перемещение камеры
            transfer = transform.forward * Input.GetAxis("Vertical");
            transfer += transform.right * Input.GetAxis("Horizontal");
            transform.position += transfer * speed * Time.deltaTime;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
