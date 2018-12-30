using UnityEngine;

namespace Fantasy
{
    public class Bullet : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }
}