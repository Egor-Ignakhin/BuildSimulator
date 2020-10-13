using UnityEngine;

public class CameraRotate : MonoBehaviour
{
	public float Sensitivity { get; set; } = 0f; // чувствительность мыши
	public float HeadMinY { get; set; } = 0f; // ограничение угла для головы
	public float HeadMaxY { get; set; } = 0f;

	private Vector3 direction;
	private float rotationY;


	private void Update()
	{
		if (GameMenu.ActiveGameMenu)
			return;
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// управление головой (камерой)
		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Sensitivity;
		rotationY += Input.GetAxis("Mouse Y") * Sensitivity;
		rotationY = Mathf.Clamp(rotationY, HeadMinY, HeadMaxY);
		transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

		// вектор направления движения
		direction = new Vector3(h, 0, v);
		direction = transform.TransformDirection(direction);
		direction = new Vector3(direction.x, 0, direction.z);
	}
}