using UnityEngine;

namespace Fantasy
{
    public class RootMotion : MonoBehaviour
    {
        public Vector3 translation;
        public Vector3 rotation;

        private void Update()
        {
            transform.Translate(translation, Space.Self);
            transform.Rotate(rotation, Space.Self);
        }
    }
}
