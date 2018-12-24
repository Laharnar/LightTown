using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fantasy
{
    public class InventoryPickupMessageReceiver : MonoBehaviour
    {
        public Inventory sendTo;

        public void Notifiy(Item item)
        {
            sendTo.AddItem(item);
        }
    }
}
