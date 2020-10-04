using UnityEngine;

public class CameraRotate : MonoBehaviour
{
	public float sensitivity = 5f; // чувствительность мыши
	public float headMinY = -90f; // ограничение угла для головы
	public float headMaxY = 90f;

	private Vector3 direction;
	private float rotationY;


    private	void Update()
	{
		if (!GameMenu.ActiveGameMenu)
		{
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			// управление головой (камерой)
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
			rotationY += Input.GetAxis("Mouse Y") * sensitivity;
			rotationY = Mathf.Clamp(rotationY, headMinY, headMaxY);
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

			// вектор направления движения
			direction = new Vector3(h, 0, v);
			direction = transform.TransformDirection(direction);
			direction = new Vector3(direction.x, 0, direction.z);
		}
	}
}