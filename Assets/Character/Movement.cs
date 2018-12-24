using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fantasy
{
    public abstract class Movement : MonoBehaviour
    {
        //public float speed = 1.0f;
        public float rotSpeed = 1.0f;

        private Animator anim;
        private CharacterState charState;

        private void Start()
        {
            anim = GetComponent<Animator>();
            charState = GetComponent<CharacterState>();
        }
        private void Update()
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
                Debug.DrawRay(transform.position, moveDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.FromToRotation(transform.forward, moveDir), 0.1f);
                anim.Play("walk");
            }
            else if (!charState.isAttacking && !charState.isIncantating)
            {
                anim.Play("idle");
            }
        }
    }
}
