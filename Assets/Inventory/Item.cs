using UnityEngine;

namespace Fantasy
{
    public class Item : MonoBehaviour
    {
        new public string name;
        public int width;
        public int height;
        public int maxStack;
        public int stack;
        public Sprite sprite;

        private void Awake()
        {
            base.name = name;
        }

        private void OnTriggerEnter(Collider other)
        {
            InventoryPickupMessageReceiver inv = other.GetComponent<InventoryPickupMessageReceiver>();
            if (inv != null)
            {
                inv.Notifiy(this);
            }
        }
    }
}
