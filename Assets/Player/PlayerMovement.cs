using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public float rotSpeed = 1.0f;
    public Transform camera;

	void Update ()
    {
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float moveY = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        Vector3 move = new Vector3(moveX, 0, moveY);

        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        transform.Translate(cameraForward * moveY + cameraRight * moveX);

        float rotY = Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed;
        float rotZ = -Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed;

        camera.RotateAround(transform.position, transform.up, rotY);
        camera.RotateAround(transform.position, camera.right, rotZ);
    }
}
