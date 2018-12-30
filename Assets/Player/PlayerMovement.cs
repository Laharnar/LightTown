using UnityEngine;

namespace Fantasy
{
    public class PlayerMovement : Movement
    {
        public float cameraSpeed = 0.05f;
        public Vector3 cameraOffset = new Vector3(0, 2, -4);

        private float camerDist;
        private float rotY;

        private void Awake()
        {
            camerDist = cameraOffset.magnitude;
        }

        /*private void Update()
        {
            if (!charState.canMove)
            {
                if (!charState.isAttacking && !charState.isIncantating)
                {
                    anim.Play("idle");
                }
                return;
            }

            // read move input 
            float moveX = 0;
            bool a = Input.GetKey(KeyCode.A);
            bool d = Input.GetKey(KeyCode.D);
            if (a && !d)
            {
                moveX = -1;
            }
            else if (d && !a)
            {
                moveX = 1;
            }
            float moveY = 0;
            bool w = Input.GetKey(KeyCode.W);
            bool s = Input.GetKey(KeyCode.S);
            if (w && !s)
            {
                moveY = 1;
            }
            else if (s && !w)
            {
                moveY = -1;
            }

            if (moveX != 0 || moveY != 0)
            {
                Vector3 moveDir = new Vector3(moveX, 0, moveY).normalized;
                moveDir = Camera.main.transform.TransformDirection(moveDir);
                moveDir.y = 0;
                moveDir.Normalize();
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir, Vector3.up), 0.1f);
                transform.Translate(moveDir * Time.deltaTime * speed, Space.World);
                anim.Play("walk");
            }
            else if (!charState.isAttacking && !charState.isIncantating)
            {
                anim.Play("idle");
            }
        }*/

        private void LateUpdate()
        {
            Transform camera = Camera.main.transform;

            float rotX = Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed;
            rotY = Mathf.Clamp(rotY - Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed, -80, 60);

            camera.RotateAround(transform.position, Vector3.up, rotX);

            float dist = Vector3.Distance(camera.position, transform.position);
            Vector3 toCamera = camera.position - transform.position;
            toCamera.y = 0;
            toCamera.Normalize();
            Vector3 rotCamOffset = Quaternion.Euler(rotY, 0, 0) * cameraOffset;
            Vector3 camEndPoint = toCamera * -rotCamOffset.z + Vector3.Cross(toCamera, Vector3.up) * rotCamOffset.x;
            camEndPoint.y = rotCamOffset.y;
            camera.position = Vector3.Lerp(camera.position, camEndPoint + transform.position, cameraSpeed);
            camera.LookAt(transform);
        }
    }
}
