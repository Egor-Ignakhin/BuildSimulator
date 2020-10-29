using UnityEngine;

public sealed class CameraRotate : MonoBehaviour,ILooking
{
	public int Sensitivity { get; set; }
	public float HeadMinY { get; set; }
	public float HeadMaxY { get; set; }

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